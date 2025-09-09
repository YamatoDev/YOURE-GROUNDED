using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpCooldown = 0.2f;
    private bool canJump = true;

    [Header("Gravity")]
    [SerializeField] private float gravityForce = -9.81f;

    [Header("Ground Check")]
    [SerializeField] private GroundCheck groundCheck;

    [Header("Look")]
    [SerializeField] private Transform playerBody;      // rotates horizontally
    [SerializeField] private Transform cameraTransform; // rotates vertically (first-person)
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float mouseSmoothTime = 0.05f;
    private Vector2 lookInput;
    private Vector2 currentMouseDelta;
    private Vector2 mouseDeltaVelocity;
    private float xRotation = 0f; // pitch

    [Header("Cameras")]
    [SerializeField] private Camera firstPersonCam;
    [SerializeField] private Camera thirdPersonCam;
    private Camera activeCamera;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent physics rotation
        Cursor.lockState = CursorLockMode.Locked;

        // Start with first-person active
        firstPersonCam.enabled = true;
        thirdPersonCam.enabled = false;
        activeCamera = firstPersonCam;
    }

    void Update()
    {
        HandleCameraSwitch();
        HandleLook();
    }

    void FixedUpdate()
    {
        HandleMovement();
        ApplyCustomGravity();
    }

    // --- Input System Callbacks ---
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && groundCheck.isGrounded && canJump)
        {
            Jump();
        }
    }

    // --- Movement ---
    private void HandleMovement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        Vector3 worldMove = transform.TransformDirection(inputDir) * moveSpeed;
        rb.linearVelocity = new Vector3(worldMove.x, rb.linearVelocity.y, worldMove.z);

        float maxSpeed = 20f;
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset Y for consistent jumps
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        StartCoroutine(JumpCooldown());
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    // --- Gravity ---
    private void ApplyCustomGravity()
    {
        if (!groundCheck.isGrounded)
        {
            rb.AddForce(Vector3.up * gravityForce, ForceMode.Acceleration);
        }
    }

    // --- Camera / Look ---
    private void HandleLook()
    {
        // Smooth mouse movement
        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta,
            lookInput,
            ref mouseDeltaVelocity,
            mouseSmoothTime
        );

        float mouseX = currentMouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = currentMouseDelta.y * mouseSensitivity * Time.deltaTime;

        // Pitch (up/down)
        xRotation -= mouseY;

        if (activeCamera == firstPersonCam)
        {
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else if (activeCamera == thirdPersonCam)
        {
            xRotation = Mathf.Clamp(xRotation, -30f, 45f); // not too much
            cameraTransform.parent.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Yaw (always rotates the player body)
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // --- Camera Switching ---
    private void HandleCameraSwitch()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (activeCamera == firstPersonCam)
            {
                firstPersonCam.enabled = false;
                thirdPersonCam.enabled = true;
                activeCamera = thirdPersonCam;
            }
            else
            {
                thirdPersonCam.enabled = false;
                firstPersonCam.enabled = true;
                activeCamera = firstPersonCam;
            }
        }
    }
}
