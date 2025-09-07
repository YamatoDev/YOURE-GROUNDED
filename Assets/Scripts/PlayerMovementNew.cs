using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementNew : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float mouseSensitivity = 200f;
    [SerializeField] private GroundCheck groundCheck;

    [Header("Cameras")]
    [SerializeField] private Camera firstPersonCam;   // first-person camera
    [SerializeField] private Camera thirdPersonCam;   // third-person camera
    private Camera activeCamera;

    private Rigidbody rb;
    private float rotationY = 0f; // yaw
    private float rotationX = 0f; // pitch (only for first person)

    private Vector3 inputDirection; // store movement input

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // prevents tipping over
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
        HandleInput();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
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

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player left/right (yaw)
        rotationY += mouseX;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

        if (activeCamera == firstPersonCam)
        {
            // Apply pitch (up/down) ONLY to first-person cam
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            firstPersonCam.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        inputDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
    }

    private void HandleMovement()
    {
        Vector3 move = inputDirection * moveSpeed;
        Vector3 newVel = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = newVel;

        // Clamp horizontal speed
        float maxSpeed = 20f;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flatVel.x, rb.linearVelocity.y, flatVel.z);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck.isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
