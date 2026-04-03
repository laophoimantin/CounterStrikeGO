using System;
using UnityEngine;

public class BoardInspectCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cam;
    private Transform _target;

    private float _distance;
    private float _pitch;
    private float _yaw;
    
    [Header("Camera Settings")]
    [SerializeField] private float _orbitSpeed = 5f;
    [SerializeField] private float _snapBackSpeed = 5f;


    private float _currentPitch;
    private float _currentYaw;

    public void ApplySettings(CameraSetupData data)
    {
        _distance = data.Distance;
        _pitch = data.Pitch;
        _yaw = data.Yaw;

        _currentPitch = _pitch;
        _currentYaw = _yaw;
    }
    
    private void LateUpdate()
    {
        HandleCameraOrbit();
    }

    private void HandleCameraOrbit()
    {
        if (_target == null || GameManager.Instance.IsGameOver) return;

        if (Input.GetMouseButton(0) && !PlayerInteractionHandler.IsPlayerInteracting)
        {
            _currentYaw += Input.GetAxis("Mouse X") * _orbitSpeed;
            _currentPitch -= Input.GetAxis("Mouse Y") * _orbitSpeed;

            _currentPitch = Mathf.Clamp(_currentPitch, 10f, 85f);
        }
        else
        {
            _currentYaw = Mathf.Lerp(_currentYaw, _yaw, Time.deltaTime * _snapBackSpeed);
            _currentPitch = Mathf.Lerp(_currentPitch, _pitch, Time.deltaTime * _snapBackSpeed);
        }

        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
        Vector3 position = _target.position - (rotation * Vector3.forward * _distance);

        _cam.position = position;
        _cam.LookAt(_target);
    }

    public void AssignFocusPoint(Transform target)
    {
        _target = target;
    }
}

[Serializable]
public struct CameraSetupData
{
    public float Distance;
    public float Pitch;
    public float Yaw;
}