using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody rb;
    private Camera mainCamera;
    private Animator animator;

    // Movement input
    private float movementX;
    private float movementY;

    // Camera settings
    public float moveSpeed = 4f;

    public float cameraDistance = 4f;
    public float cameraRotateSpeed = 90f;
    public float cameraSmoothSpeed = 10f;

    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float cameraYaw;
    private float cameraPitch = 20f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();

        cameraYaw = transform.eulerAngles.y;
    }

    void OnMove(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Arrow keys control camera
        float yawInput = 0f;
        float pitchInput = 0f;

        if (Keyboard.current.leftArrowKey.isPressed)
            yawInput = -1f;
        else if (Keyboard.current.rightArrowKey.isPressed)
            yawInput = 1f;

        if (Keyboard.current.upArrowKey.isPressed)
            pitchInput = -1f;
        else if (Keyboard.current.downArrowKey.isPressed)
            pitchInput = 1f;

        cameraYaw += yawInput * cameraRotateSpeed * Time.deltaTime;
        cameraPitch += pitchInput * cameraRotateSpeed * Time.deltaTime;

        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
    }

    void FixedUpdate()
    {
        if (mainCamera == null) return;

        // Movement relative to camera
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 move =
            cameraForward * movementY +
            cameraRight * movementX;

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);

        // Rotate character toward movement
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }

        animator.SetBool("IsRunning", move.sqrMagnitude > 0.001f);
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);

        Vector3 desiredPosition =
            transform.position +
            rotation * new Vector3(0, 0, -cameraDistance);

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            desiredPosition,
            Time.deltaTime * cameraSmoothSpeed
        );

        mainCamera.transform.LookAt(
            transform.position + Vector3.up * 1.2f
        );
    }
}
