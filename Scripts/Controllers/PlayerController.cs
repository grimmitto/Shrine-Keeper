using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float rotationSmoothTime = 0.1f;
    public float lookSensitivity = 1.0f;
    public float minPitch = -40f;
    public float maxPitch = 70f;

    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    [Header("Input References")]
    public PlayerInput playerInput;
    private InputAction moveAction;
    private bool inputLocked;

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private float turnSmoothVelocity;
    private Vector2 moveInput;
    private bool isRunning;
    private bool jumpQueued;
    private bool isFreeLooking;

    [Header("Camera Zoom")]
    public Transform cameraChild;
    public float zoomSpeed = 5f;
    public float minZoom = 1.0f;
    public float maxZoom = 6.0f;
    public float defaultZoom = 3.0f;
    private float targetZoom;

    // Track yaw from previous frame
    private float lastYaw;
    public bool loadedFromSave = false;


    // ---------------- Animator Hashes ----------------

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int TurnHash = Animator.StringToHash("TurnAmount");
    private static readonly int TurnMoveHash = Animator.StringToHash("TurnWhileMoving");
    private static readonly int LookYawHash = Animator.StringToHash("LookYaw");
    private static readonly int LookPitchHash = Animator.StringToHash("LookPitch");

    // ------------------ Input Events ------------------

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (inputLocked) return;

        if (ctx.performed || ctx.started)
            moveInput = ctx.ReadValue<Vector2>();
        else if (ctx.canceled)
            moveInput = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (inputLocked) return;
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext ctx) => isRunning = ctx.ReadValueAsButton();

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) jumpQueued = true;
    }

    public void OnFreeLook(InputAction.CallbackContext ctx)
    {
        isFreeLooking = ctx.ReadValueAsButton();
    }

    public void OnZoom(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<Vector2>().y;
        if (!isFreeLooking || Mathf.Abs(scroll) < 0.01f) return;

        targetZoom -= scroll * zoomSpeed * Time.deltaTime;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    public void SetInputLock(bool locked)
    {
        inputLocked = locked;

        if (locked)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
        }
    }

    // ------------------ Core Unity Loop ------------------

    void Start()
    {
        if (loadedFromSave)
            return;

        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (!playerInput) playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];

        if (cameraChild)
        {
            float startZ = -cameraChild.localPosition.z;
            targetZoom = startZ;
            defaultZoom = startZ;
        }

        // Initialize yaw tracking
        yaw = transform.eulerAngles.y;
        lastYaw = yaw;

        if (cameraTransform)
        {
            Vector3 camEuler = cameraTransform.rotation.eulerAngles;
            pitch = camEuler.x;
        }
    }

    public void ApplyLoadedRotation(float yawValue, float pitchValue)
    {
        loadedFromSave = true;
        yaw = yawValue;
        pitch = pitchValue;
    }


    void Update()
    {
        if (inputLocked) return;

        // IMPORTANT FIX:
        // Record yaw *before* anything rotates this frame
        lastYaw = transform.eulerAngles.y;

        moveInput = moveAction.ReadValue<Vector2>();

        HandleMovement();
        ApplyGravityAndJump();
    }

    void LateUpdate()
    {
        if (inputLocked) return;

        HandleLook();
        HandleZoom();
        UpdateAnimatorParameters();
    }

    // ------------------ Movement ------------------

    void HandleMovement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            // Camera-relative movement
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;

            // Rotate toward movement
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    void ApplyGravityAndJump()
    {
        bool grounded = controller.isGrounded;

        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        if (grounded && jumpQueued)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ------------------ Animator Updates ------------------

    void UpdateAnimatorParameters()
    {
        float inputMag = moveInput.magnitude;
        bool moving = inputMag > 0.1f;
        bool grounded = controller.isGrounded;

        animator.SetBool(IsMovingHash, moving);
        animator.SetBool(IsRunningHash, isRunning);

        float currentSpeed = moving ? (isRunning ? runSpeed : walkSpeed) : 0f;
        animator.SetFloat(SpeedHash, currentSpeed);

        animator.SetFloat(DirectionHash, moveInput.y);
        animator.SetBool(IsGroundedHash, grounded);
        animator.SetFloat(VerticalVelHash, velocity.y);

        // ----- FIXED TURNING -----

        float currentYaw = transform.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(lastYaw, currentYaw);

        float normalizedTurn = Mathf.Clamp(deltaYaw / 45f, -1f, 1f);

        bool idle = !moving && grounded;
        bool turningInPlace = idle && !isFreeLooking && Mathf.Abs(normalizedTurn) > 0.01f;


        float turnWhileMoving = moving ? normalizedTurn : 0f;
        animator.SetFloat(TurnMoveHash, turnWhileMoving);

        // ----- HEAD LOOK -----
        animator.SetFloat(LookYawHash, Mathf.Clamp(lookInput.x, -1f, 1f));
        animator.SetFloat(LookPitchHash, Mathf.Clamp(-lookInput.y, -1f, 1f));

        animator.SetFloat(TurnHash, animator.GetBool(IsMovingHash) ? Mathf.Clamp(lookInput.x, -1f, 1f) : 0f);

    }

    // ------------------ Camera ------------------

    void HandleLook()
    {
        if (!cameraTransform) return;

        yaw += lookInput.x * lookSensitivity;
        pitch -= lookInput.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (isFreeLooking)
        {
            cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            cameraTransform.rotation = Quaternion.Euler(pitch, transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }

    void HandleZoom()
    {
        if (!cameraChild) return;

        Vector3 localPos = cameraChild.localPosition;

        if (isFreeLooking)
        {
            localPos.z = Mathf.Lerp(localPos.z, -targetZoom, Time.deltaTime * 8f);
        }
        else
        {
            localPos.z = Mathf.Lerp(localPos.z, -defaultZoom, Time.deltaTime * 6f);
        }

        cameraChild.localPosition = localPos;
    }
}
