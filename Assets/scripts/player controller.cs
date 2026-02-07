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
        bool isMoving = (movementX != 0 || movementY != 0);
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
        
        // Camera follows player
        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position + new Vector3(-3, 3, 1);
            mainCamera.transform.LookAt(transform);
            //  mainCamera.transform.rotation = Quaternion.Euler(15, 5, 0);
        } else if (mainCamera == null){
            Debug.LogWarning("Main Camera not found!");
        }
    }
}