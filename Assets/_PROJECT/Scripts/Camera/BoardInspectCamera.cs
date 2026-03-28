using Pawn;
using UnityEngine;

public class BoardInspectCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cam;
    [SerializeField] private Transform _target;

    [Header("Default Camera Settings")]
    [SerializeField] private float _distance = 10f;
    [SerializeField] private float _defaultPitch = 45f;
    [SerializeField] private float _defaultYaw = 0f;

    [Header("Camera Settings")]
    [SerializeField] private float _orbitSpeed = 5f;
    [SerializeField] private float _snapBackSpeed = 5f;


    private float _currentPitch;
    private float _currentYaw;

    private void Start()
    {
        _currentPitch = _defaultPitch;
        _currentYaw = _defaultYaw;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        if (Input.GetMouseButton(0) && !ClickHandler.IsPlayerInteracting)
        {
            _currentYaw += Input.GetAxis("Mouse X") * _orbitSpeed;
            _currentPitch -= Input.GetAxis("Mouse Y") * _orbitSpeed;

            _currentPitch = Mathf.Clamp(_currentPitch, 10f, 85f);
        }
        else
        {
            _currentYaw = Mathf.Lerp(_currentYaw, _defaultYaw, Time.deltaTime * _snapBackSpeed);
            _currentPitch = Mathf.Lerp(_currentPitch, _defaultPitch, Time.deltaTime * _snapBackSpeed);
        }

        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
        Vector3 position = _target.position - (rotation * Vector3.forward * _distance);

        _cam.position = position;
        _cam.LookAt(_target);
    }
}