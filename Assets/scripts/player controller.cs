using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // ===== COMPONENTS =====
    private Camera mainCamera;
    private Animator animator;

    // ===== INPUT =====
    private Vector2 movementInput;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool sprintInput;

    // ===== MOVEMENT =====
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7.5f;
    public float acceleration = 12f;
    public float deceleration = 16f;
    public float rotationSpeed = 10f;
    public float jumpForce = 15f;

    private Vector3 currentVelocity;

    // ===== IDLE / SPRINT LOGIC =====
    [Header("Idle")]
    public float idleDelay = 0.25f;
    private float idleTimer;
    private bool isIdle;

    // ===== CAMERA =====
    [Header("Camera")]
    public float cameraDistance = 4f;
    public float cameraRotateSpeed = 90f;
    public float cameraSmoothSpeed = 10f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float cameraYaw;
    private float cameraPitch = 20f;

    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        cameraYaw = transform.eulerAngles.y;
    }

    // ===== INPUT CALLBACKS =====
    void OnMove(InputValue value)   => movementInput = value.Get<Vector2>();
    void OnLook(InputValue value)   => lookInput = value.Get<Vector2>();
    void OnJump(InputValue value)   => jumpInput = value.isPressed;
    void OnSprint(InputValue value) => sprintInput = value.isPressed;

    void Update()
    {
        if (!mainCamera) return;

        UpdateIdleState();
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

    // ===== IDLE =====
    private void UpdateIdleState()
    {
        if (movementInput.sqrMagnitude < 0.01f)
            idleTimer += Time.deltaTime;
        else
            idleTimer = 0f;

        isIdle =
            idleTimer >= idleDelay &&
            new Vector3(currentVelocity.x, 0f, currentVelocity.z).sqrMagnitude < 0.05f;
    }

    // ===== CAMERA =====
    private void HandleCameraRotation()
    {
        cameraYaw += lookInput.x * cameraRotateSpeed * Time.deltaTime;
        cameraPitch -= lookInput.y * cameraRotateSpeed * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
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

    // ===== MOVEMENT =====
    private void HandleMovement()
    {
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = mainCamera.transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 inputDirection =
            forward * movementInput.y +
            right * movementInput.x;

        if (inputDirection.sqrMagnitude > 1f)
            inputDirection.Normalize();

        float targetSpeed =
            sprintInput && !isIdle ? sprintSpeed : walkSpeed;

        Vector3 targetVelocity = inputDirection * targetSpeed;

        float accelRate =
            inputDirection.sqrMagnitude > 0.01f
                ? acceleration
                : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            accelRate * Time.deltaTime
        );

        transform.position += currentVelocity * Time.deltaTime;

        if (inputDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(inputDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // ===== JUMP =====
    private void HandleJump()
    {
        if (jumpInput && IsGrounded())
        {
            currentVelocity.y = jumpForce;
            jumpInput = false;
        }

        // simple gravity
        if (!IsGrounded())
            currentVelocity.y += Physics.gravity.y * Time.deltaTime;
        else if (currentVelocity.y < 0f)
            currentVelocity.y = 0f;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            0.2f
        );
    }

    // ===== ANIMATION =====
    private void HandleAnimation()
    {
        float planarSpeed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;
        
        // Calculate normalized speed on a 0-2 scale:
        // 0 = idle
        // 0-1 = walk to run
        // 1-2 = run to sprint
        float normalizedSpeed;
        
        if (planarSpeed < 0.01f)
        {
            normalizedSpeed = 0f;
        }
        else if (sprintInput && !isIdle)
        {
            // Sprinting: map from walkSpeed to sprintSpeed → 1.0 to 2.0
            normalizedSpeed = 1f + Mathf.Clamp01((planarSpeed - walkSpeed) / (sprintSpeed - walkSpeed));
        }
        else
        {
            // Walking/Running: map from 0 to walkSpeed → 0.0 to 1.0
            normalizedSpeed = Mathf.Clamp01(planarSpeed / walkSpeed);
        }

        animator.SetFloat("Speed", normalizedSpeed, 0.1f, Time.deltaTime);
        animator.SetBool("IsJumping", !IsGrounded());
    }
}
