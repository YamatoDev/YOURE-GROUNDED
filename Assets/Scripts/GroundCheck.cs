using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private float checkDistance = 0.2f; 
    [SerializeField] private LayerMask groundMask;

    [SerializeField] public bool isGrounded = false;

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, checkDistance, groundMask);
        Debug.DrawRay(transform.position, Vector3.down * checkDistance, isGrounded ? Color.green : Color.red);
    }
}
