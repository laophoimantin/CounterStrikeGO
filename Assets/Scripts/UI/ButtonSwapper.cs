using DG.Tweening;
using UnityEngine;
using System;

public class ButtonSwapper : MonoBehaviour
{
    [Serializable] 
    public class SwapItem
    {
        public string Name;
        public RectTransform Container;
        public GameObject InGameObj; 
        public GameObject MenuObj;   
    }

    [Header("Configuration")]
    [SerializeField] private SwapItem[] _items; 

    [Header("Settings")]
    [SerializeField] private float _duration = 0.25f;

    public void Swap(bool showMenuVersion)
    {
        foreach (var item in _items)
        {
            // Safety Check
            if (item.Container == null) continue;

            item.Container.DOKill();

            Sequence seq = DOTween.Sequence();

            seq.Append(item.Container.DORotate(new Vector3(0, 90, 0), _duration / 2)
                .SetEase(Ease.InBack));

            seq.AppendCallback(() =>
            {
                if (item.InGameObj) item.InGameObj.SetActive(!showMenuVersion);
                if (item.MenuObj) item.MenuObj.SetActive(showMenuVersion);

                item.Container.localEulerAngles = new Vector3(0, -90, 0);
            });

            seq.Append(item.Container.DORotate(Vector3.zero, _duration / 2)
                .SetEase(Ease.OutBack));
        }
    }
}