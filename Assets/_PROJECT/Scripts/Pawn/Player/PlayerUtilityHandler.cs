using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtilityHandler : MonoBehaviour, IUtilityEquipper
{
    private PlayerController _controller;
    private UtilityController _currentItem;

    public bool HasItem => _currentItem != null;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    public void EquipUtility(UtilityController newItem)
    {
        _currentItem = newItem;
        UpdateVisualState();
    }

    public bool TryUseUtility(Node targetNode, Action<bool> onActionFinished)
    {
        if (!HasItem) return false;

        List<Node> validNodes = NodeManager.Instance.GetNodesInRange(_controller.CurrentNode, _currentItem.ThrowRange);

        if (!validNodes.Contains(targetNode))
        {
            return false; 
        }

        bool endsTurn = _currentItem.EndsTurn;
        _controller.PlayerVisual.Wobble();
        _currentItem.Throw(targetNode, () => { onActionFinished?.Invoke(endsTurn); });
        UnequipItem();
    
        return true;
    }

    private void UnequipItem()
    {
        _currentItem = null;
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        _controller.PlayerVisual.SetHoldingItemState(HasItem);
    }
}