using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
    }

    void LateUpdate()
    {
        if (_cam == null) return;

        Vector3 direction = _cam.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
