using UnityEngine;

public class PlayerClamp : MonoBehaviour
{
    [Header("Bounds")]
    public float minX = 0f;
    public float maxX = 1000f;
    public float minZ = 0f;
    public float maxZ = 1000f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        // Clamp X and Z, leave Y alone
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}
