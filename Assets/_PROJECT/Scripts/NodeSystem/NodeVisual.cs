using UnityEngine;

/// <summary>
/// Toggling specific overlays
/// </summary>
public class NodeVisual : MonoBehaviour
{
    [Header("Visual Objects")]
    [SerializeField] private GameObject _baseRenderer;
    [SerializeField] private GameObject _exitOverlayRenderer;
    [SerializeField] private GameObject _overlayRenderer;

    public void SetupVisual(BaseNodeFeature feature, bool isObstacle)
    {
        _baseRenderer.SetActive(!isObstacle);
        ToggleAttackRangeHighlight(false);
        if (feature is ExitFeature)
        {
            _exitOverlayRenderer.SetActive(true);
        }
        else
        {
            _exitOverlayRenderer.SetActive(false);
        }
    }

    public void ToggleAttackRangeHighlight(bool isOn)
    {
        _overlayRenderer.SetActive(isOn);
    }
}