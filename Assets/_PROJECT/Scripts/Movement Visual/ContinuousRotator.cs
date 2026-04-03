using UnityEngine;

public class ContinuousRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 90f;
    
    [SerializeField] private Vector3 _rotationAxis = Vector3.up; 

    void Update()
    {
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }
}