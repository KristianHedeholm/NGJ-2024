using UnityEngine;

public class Crystal : MonoBehaviour
{
    public void SetTargetAngle(float angle)
    {
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
