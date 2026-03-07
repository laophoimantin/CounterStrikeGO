using System;
using UnityEngine;
using Pawn;
using Core;

namespace Grid
{
    [CreateAssetMenu(fileName = "ObjectivePickupFeature", menuName = "Grid/Node Feature/Objective Pickup Feature")]
    public class ObjectivePickupFeature : BaseNodeFeature
    {
        [SerializeField] private GameObject _objectiveItemPrefab;
        private GameObject _objectiveItem;
        private bool _isCollected;

        public override void Initialize(Node owner)
        {
            base.Initialize(owner);

            if (_objectiveItemPrefab == null)
            {
                Debug.LogError("Objective Item is null!");
                return;
            }

            _objectiveItem = Instantiate(_objectiveItemPrefab, owner.transform, false);
            _objectiveItem.transform.localPosition = Vector3.zero;
        }

        public override void OnEnter(GridUnit unit)
        {
            if (_isCollected) return;
            if (unit is not PlayerController) return;
            Debug.Log("Objective Pickup!");
            _isCollected = true;
            GameManager.Instance.OnPlayerPickedUpObjective();
            HideItem();
        }

        private void HideItem()
        {
            _objectiveItem.SetActive(false);
        }
    }
}