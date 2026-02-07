using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb; 
    private float movementX;
    private float movementY;
    private Camera mainCamera;
    private bool moved;
    private Animator animator;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    void OnMove(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x; 
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        transform.Translate(movementX * Time.deltaTime * 4f, 0, movementY * Time.deltaTime * 4f);
        
        // Animation control
        bool isMoving = movementX != 0 || movementY != 0;
        animator.SetBool("IsRunning", isMoving);

        // Camera follows player
        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position + new Vector3(0, 3, -1);
            // Point de visée légèrement devant et en hauteur du personnage
            Vector3 lookAtPoint = transform.position + transform.forward * 0.8f + Vector3.up * 0.9f;
            mainCamera.transform.LookAt(lookAtPoint);
        } else if (mainCamera == null){
            Debug.LogWarning("Main Camera not found!");
        }
    }
}