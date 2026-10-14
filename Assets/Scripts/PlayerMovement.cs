using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Camera")]
    public Transform cameraTarget;
    public float mouseSensitivity = 150f;

    [Header("Look Limits")]
    public float minLookAngle = -80f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public float groundCheckDistance = 1.1f;
    public LayerMask groundLayer;

    [Header("Y Stability")]
    public float yStabilityStrength = 15f;
    public float maxYCorrection = 0.15f;

    private Rigidbody rb;

    private float horizontal;
    private float vertical;

    private float yaw;
    private float pitch;

    private bool isJumping;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Player cannot physically rotate.
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        // Smooth Rigidbody physics.
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Start camera rotation from player's current rotation.
        yaw = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // =========================
        // MOVEMENT INPUT
        // =========================

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");


        // =========================
        // MOUSE LOOK
        // =========================

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity * Time.deltaTime;

        pitch -= mouseY * mouseSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(
            pitch,
            minLookAngle,
            maxLookAngle
        );


        // =========================
        // JUMP
        // =========================

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );

            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        // =========================
        // MOVEMENT
        // =========================

        Quaternion yawRotation = Quaternion.Euler(
            0f,
            yaw,
            0f
        );

        Vector3 forward =
            yawRotation * Vector3.forward;

        Vector3 right =
            yawRotation * Vector3.right;

        Vector3 movement =
            right * horizontal +
            forward * vertical;

        movement.Normalize();

        Vector3 velocity = rb.linearVelocity;

        velocity.x = movement.x * moveSpeed;
        velocity.z = movement.z * moveSpeed;


        // =========================
        // Y STABILITY
        // =========================

        if (IsGrounded() && !isJumping)
        {
            // Only correct VERY small vertical velocity.
            // This prevents tiny physics vibrations.
            if (Mathf.Abs(velocity.y) < maxYCorrection)
            {
                velocity.y = Mathf.Lerp(
                    velocity.y,
                    0f,
                    yStabilityStrength * Time.fixedDeltaTime
                );
            }
        }


        rb.linearVelocity = velocity;


        // Once the player is airborne,
        // allow normal physics again.
        if (!IsGrounded())
        {
            isJumping = false;
        }
    }

    private void LateUpdate()
    {
        if (cameraTarget == null)
            return;

        // CameraTarget only controls vertical look.
        cameraTarget.localRotation =
            Quaternion.Euler(
                pitch,
                0f,
                0f
            );
    }

    public float GetYaw()
    {
        return yaw;
    }

    public float GetPitch()
    {
        return pitch;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }
}