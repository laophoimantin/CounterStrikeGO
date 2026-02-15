using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UtilityVisual : GridUnitVisual
{
    [SerializeField] private Transform _utilityModel;

    void Start()
    {
        if (_utilityModel != null) 
            _utilityModel.gameObject.SetActive(false);
        if (_pawnModel != null)
            _pawnModel.gameObject.SetActive(true);
    }
    public void SwitchToFlyingMode(Vector3 startPos)
    {
        _pawnModel.gameObject.SetActive(false);
        _utilityModel.gameObject.SetActive(true);
        _utilityModel.transform.position = startPos + Vector3.up * 2; 
    }

    public IEnumerator AnimateThrow(Vector3 targetPos, float duration)
    {
        yield return _utilityModel.transform
            .DOJump(targetPos, 2.0f, 1, duration) 
            .SetEase(Ease.Linear)
            .WaitForCompletion();
    }
}
