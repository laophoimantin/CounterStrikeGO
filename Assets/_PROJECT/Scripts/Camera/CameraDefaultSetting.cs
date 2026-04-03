using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultCameraSettings", menuName = "Camera/Default Camera Settings")]
public class CameraDefaultSetting : ScriptableObject
{
    [Header("Default Camera Settings")]
    [SerializeField] private float _distance = 10f;
    [SerializeField] private float _defaultPitch = 45f;
    [SerializeField] private float _defaultYaw = 0f;
    
    public float Distance => _distance;
    public float Pitch => _defaultPitch;
    public float Yaw => _defaultYaw;    
}
