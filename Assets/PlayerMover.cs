using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravityForce = -9.81f;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    [Header("Rotation")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float mouseSmoothTime = 0.05f; // lower = snappier, higher = smoother
    private Vector2 mouseDelta;
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    private float xRotation = 0f;

    [Header("Ground Check")]
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private float jumpCooldown = 0.2f;
    private bool canJump = true;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent unwanted physics rotation
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
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
        mouseDelta = context.ReadValue<Vector2>();
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
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection) * moveSpeed;

        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
    }

    private void ApplyCustomGravity()
    {
        if (!groundCheck.isGrounded)
        {
            rb.AddForce(Vector3.up * gravityForce, ForceMode.Acceleration);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // reset Y velocity
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        StartCoroutine(JumpCooldown());
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    // --- Camera ---
    private void HandleLook()
    {
        // Smooth the mouse delta
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, mouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        float mouseX = currentMouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = currentMouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
