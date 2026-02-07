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
    [Header("Movement")]
    public float maxSpeed = 4f;
    public float acceleration = 12f;
    public float deceleration = 16f;
    public float rotationSpeed = 10f;

    private Vector3 currentVelocity;

    // Camera settings
    [Header("Camera")]
    public float cameraDistance = 4f;
    public float cameraRotateSpeed = 90f;
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

    void OnMove(InputValue value) => movementInput = value.Get<Vector2>();
    void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    void OnJump(InputValue value) => jumpInput = value.isPressed;

    void OnSprint(InputValue value) => maxSpeed = value.isPressed ? 20f : 4f;

    void Update()
    {
        if (!mainCamera) return;

        HandleCameraRotation();
        HandleMovement();
        HandleJump();
        HandleAnimation();
    }

    void LateUpdate()
    {
        if (!mainCamera) return;
        UpdateCameraPosition();
    }

    private void HandleCameraRotation()
    {
        cameraYaw += lookInput.x * cameraRotateSpeed * Time.deltaTime;
        cameraPitch -= lookInput.y * cameraRotateSpeed * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
    }

    private void HandleMovement()
    {
        // Camera-relative directions
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = mainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 inputDirection = forward * movementInput.y + right * movementInput.x;
        inputDirection = inputDirection.sqrMagnitude > 1f ? inputDirection.normalized : inputDirection;

        Vector3 targetVelocity = inputDirection * maxSpeed;

        // Accelerate or decelerate
        float accelRate = inputDirection.sqrMagnitude > 0.01f ? acceleration : deceleration;
        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            accelRate * Time.deltaTime
        );

        transform.position += currentVelocity * Time.deltaTime;

        // Rotate toward movement direction
        if (currentVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void HandleJump()
    {
        if (jumpInput && IsGrounded())
        {
            currentVelocity.y = 15f; // Simple jump impulse
            jumpInput = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f);
    }

    private void HandleAnimation()
    {
        // animator.SetFloat("Speed", currentVelocity.magnitude / maxSpeed);
        // animator.SetBool("IsGrounded", IsGrounded());
        animator.SetBool("IsRunning", movementInput.sqrMagnitude > 0.001f);
        animator.SetBool("IsJumping", jumpInput);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        Vector3 desiredPosition =
            transform.position
            - rotation * Vector3.forward * cameraDistance
            + Vector3.up * 1.2f;

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            desiredPosition,
            cameraSmoothSpeed * Time.deltaTime
        );

        mainCamera.transform.LookAt(transform.position + Vector3.up * 1.2f);
    }
}
