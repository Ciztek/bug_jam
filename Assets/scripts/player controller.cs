using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Components
    private Camera mainCamera;
    private Animator animator;

    // Input
    private Vector2 movementInput;
    private Vector2 lookInput;
    private bool jumpInput;

    // Movement settings
    public float moveSpeed = 4f;
    public float rotationSpeed = 10f;

    // Camera settings
    public float cameraDistance = 4f;
    public float cameraRotateSpeed = 90f; // Degrees per second
    public float cameraSmoothSpeed = 10f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float cameraYaw = 0f;
    private float cameraPitch = 20f;

    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        cameraYaw = transform.eulerAngles.y;
    }

    // Move input (WASD / ZQSD)
    void OnMove(InputValue value) => movementInput = value.Get<Vector2>();

    // Look input (mouse or arrow keys)
    void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

    // Jump input (spacebar)
    void OnJump(InputValue value) => jumpInput = value.isPressed;

    void Update()
    {
        if (mainCamera == null) return;

        HandleCameraRotation();
        HandleMovement();
        HandleJump();
        HandleAnimation();
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        UpdateCameraPosition();
    }

    private void HandleCameraRotation()
    {
        // Update yaw and pitch based on input
        cameraYaw += lookInput.x * cameraRotateSpeed * Time.deltaTime;
        cameraPitch -= lookInput.y * cameraRotateSpeed * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
    }

    private void HandleMovement()
    {
        // Camera-relative movement
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = mainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 move = forward * movementInput.y + right * movementInput.x;

        // Normalize diagonal movement
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        // Apply movement
        transform.position += move * moveSpeed * Time.deltaTime;

        // Rotate character toward movement
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (jumpInput && IsGrounded())
        {
            transform.position += Vector3.up * 0.5f;
            jumpInput = false; // Reset jump input to prevent continuous jumping
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f);
    }

    private void HandleAnimation()
    {
        animator.SetBool("IsRunning", movementInput.sqrMagnitude > 0.001f);
        animator.SetBool("IsJumping", jumpInput);
    }

    private void UpdateCameraPosition()
    {
        // Orbiting camera
        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
        Vector3 desiredPosition = transform.position - rotation * Vector3.forward * cameraDistance + Vector3.up * 1.2f;

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, desiredPosition, cameraSmoothSpeed * Time.deltaTime);
        mainCamera.transform.LookAt(transform.position + Vector3.up * 1.2f);
    }
}
