using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Characters.Player
{
    public class ClickHandler : MonoBehaviour
    {
        
        #region Private Fields
        
        private Camera _mainCamera;
        private PlayerController _selectedPlayer;
        
        #endregion

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                HandleClick();
        }

        private void HandleClick()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (TurnManager.Instance.CurrentTurn != TurnType.PlayerPlanning) return;

                if (hit.collider.TryGetComponent(out PlayerController player))
                {
                    _selectedPlayer = player;
                    _selectedPlayer.HighlightAvailableNodes(true);
                    return;
                }

                if (hit.collider.TryGetComponent(out OldNode node))
                {
                    if (_selectedPlayer != null)
                    {
                        _selectedPlayer.TryMoveTo(node);
                        _selectedPlayer.HighlightAvailableNodes(false);
                        _selectedPlayer = null;
                    }
                }
            }
        }
    }
}