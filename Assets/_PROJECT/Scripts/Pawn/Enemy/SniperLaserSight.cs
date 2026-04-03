using UnityEngine;

public class SniperLaserSight : MonoBehaviour
{
    [SerializeField] private EnemyController _sniper;
    
    [SerializeField] private Transform _laserPivot;

    private void OnEnable()
    {
        this.Subscribe<OnSniperTargetDetectedEvent>(HandleLaserUpdate);
    }

    private void OnDisable()
    {
        this.Unsubscribe<OnSniperTargetDetectedEvent>(HandleLaserUpdate);
    }

    private void HandleLaserUpdate(OnSniperTargetDetectedEvent evt)
    {
        if (evt.Sniper != _sniper) return;

        if (_sniper.IsDead || evt.TargetNode == null)
        {
            _laserPivot.gameObject.SetActive(false);
            return;
        }

        _laserPivot.gameObject.SetActive(true);

        Vector3 startPos = _sniper.CurrentNode.transform.position;
        Vector3 endPos = evt.TargetNode.transform.position;

        float distance = Vector3.Distance(startPos, endPos);

        Vector3 newScale = _laserPivot.localScale;
        newScale.z = distance;
        _laserPivot.localScale = newScale;
    }
}