using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{

    private Rigidbody rb; 
    private float movementX;
    private float movementY;
    void Start()
    {
      rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();; 
        movementX = movementVector.x; 
        movementY = movementVector.y; 
        
    }

    void FixedUpdate()
    {
        
        transform.Translate(movementX * Time.deltaTime * 4f, 0, movementY * Time.deltaTime * 4f);
    }

    
    
}