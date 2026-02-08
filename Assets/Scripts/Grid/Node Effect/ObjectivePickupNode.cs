using System;
using UnityEngine;
using Pawn;
using Core;

namespace Grid
{
    public class ObjectivePickupNode : MonoBehaviour, INodeEffect
    {
        [SerializeField] private GameObject _objectiveItemPrefab;
        private GameObject _objectiveItem;
        
        private Node _node;

        public void Initialize(Node owner)
        {
            _node = owner;
            if (_objectiveItemPrefab == null)
            {
                Debug.LogError("Objective Item is null!");
                return;
            }
            _objectiveItem = Instantiate(_objectiveItemPrefab, owner.transform, false);
            _objectiveItem.transform.localPosition = Vector3.zero;
        }

        public void OnEnter(GridUnit unit)
        {
            if (unit is PlayerController)
            {
                GameManager.Instance.OnPlayerPickedUpObjective();
                HideItem();
                this.enabled = false;
                Debug.Log("Objective Collected!");
            }
        }

        private void HideItem()
        {
            // Hide bomb
            _objectiveItem.SetActive(false);
        }
    }
}