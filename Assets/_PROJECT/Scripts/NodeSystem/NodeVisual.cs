using UnityEngine;

public class NodeVisual : MonoBehaviour
{
    [Header("Renderers")]
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