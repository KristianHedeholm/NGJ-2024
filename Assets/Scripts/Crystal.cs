using UnityEngine;

public class Crystal : MonoBehaviour
{
    public void SetTargetAngel(float angel)
    {
        transform.rotation = Quaternion.AngleAxis(angel, Vector3.forward);
    }
}
