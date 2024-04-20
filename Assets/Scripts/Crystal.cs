using UnityEngine;

public class Crystal : MonoBehaviour
{

    
    [SerializeField]
    [Range(0.0f, 180.0f)]
    private float _maxAngle;

    [SerializeField]
    [Range(0.0f, -180.0f)]
    private float _minAngle;

    [SerializeField]
    private float _speed;

    private float _inputValue;

    private void Awake()
    {
        _inputValue = 0.0f;
    }

    public void UpdateMovement(float inputValue)
    {
        _inputValue = inputValue;
    }

    public void LeftCrystal()
    {
        _inputValue = 0.0f;
    }

    private void Update()
    {
        if(_inputValue == 0.0f)
        {
            return;
        }

        var rotationValue = transform.rotation.z;
        var addedRotation = _inputValue * Time.deltaTime * _speed;
        var totalRotation = rotationValue + addedRotation;
        if(_minAngle < totalRotation && totalRotation < _maxAngle)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, totalRotation);
        }
    }
}
