using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D => _rigidbody2D;

    [SerializeField]
    private Rigidbody2D _rigidbody2D;

    public float springStrength;
    public float springDamp;
}
