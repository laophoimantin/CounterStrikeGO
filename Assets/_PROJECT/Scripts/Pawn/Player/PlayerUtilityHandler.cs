using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtilityHandler : MonoBehaviour, IUtilityEquipper
{
    private PlayerController _controller;
    private UtilityController _currentItem;
    
    private List<Node> _highlightedNodes = new();
    
    public bool HasItem => _currentItem != null;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        this.Subscribe<OnTurnChangedEvent>(HandleTurnChanged);
    }

    private void OnDisable()
    {
        this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChanged);
        ClearRangeHighlight(); 
    }
    
    private void HandleTurnChanged(OnTurnChangedEvent eventData)
    {
        if (eventData.NewTurn == TurnType.PlayerPlanning)
        {
            if (HasItem)
            {
                ShowRangeHighlight();
            }
        }
        else
        {
            ClearRangeHighlight();
        }
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
        ClearRangeHighlight();
        _controller.PlayerVisual.Wobble();
        _currentItem.Throw(targetNode, () => { onActionFinished?.Invoke(endsTurn); });
        UnequipItem();
    
        return true;
    }

    private void UnequipItem()
    {
        _currentItem = null;
        UpdateVisualState();
        ClearRangeHighlight();
    }

    private void ShowRangeHighlight()
    {
        ClearRangeHighlight();

        if (!HasItem) return;

        _highlightedNodes = NodeManager.Instance.GetNodesInRange(_controller.CurrentNode, _currentItem.ThrowRange);

        foreach (Node node in _highlightedNodes)
        {
            node.ToggleHighlight(true); 
        }
    }

    private void ClearRangeHighlight()
    {
        if (_highlightedNodes == null || _highlightedNodes.Count == 0) return;

        foreach (Node node in _highlightedNodes)
        {
            node.ToggleHighlight(false);
        }

        _highlightedNodes.Clear();
    }
    
    private void UpdateVisualState()
    {
        _controller.PlayerVisual.SetHoldingItemState(HasItem);
    }
}