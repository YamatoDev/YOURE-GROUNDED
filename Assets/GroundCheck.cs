using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isGrounded = false;

    void Update()
    {
        Debug.Log(isGrounded);
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Player is grounded!");
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Player is on the air!");
            isGrounded = false;
        }
    }
}
