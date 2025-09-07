using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public bool isGrounded;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer; // Assign this in the Inspector

    void Update()
    {
        // Cast a ray downward from the object's position
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundLayer);

        // Optional: visualize the ray in the editor
        Debug.DrawRay(transform.position, Vector3.down * groundDistance, isGrounded ? Color.green : Color.red);
    }
}
