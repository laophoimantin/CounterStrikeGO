using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UtilityVisual : GridUnitVisual
{
    [SerializeField] private Transform _utilityModel;
    
    [SerializeField] private float _throwHeight = 2;
    [SerializeField] private float _throwDuration = 1;

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

    public void HideUtitlityModel()
    {
        _pawnModel.gameObject.SetActive(false);
    }

    public IEnumerator AnimateThrow(Vector3 targetPos)
    {
        yield return _utilityModel.transform
            .DOJump(targetPos, _throwHeight, 1, _throwDuration)
            .SetEase(Ease.Linear)
            .WaitForCompletion();
    }
}