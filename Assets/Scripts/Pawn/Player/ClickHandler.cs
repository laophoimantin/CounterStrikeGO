using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn
{
    public class ClickHandler : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private float _dragThresholdPercent = 0.05f;
        
        private bool _dragging = false;
        private Vector2 _startPos;
        private Camera _cam;

        void Start()
        {
            _cam = Camera.main;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                TryStartDrag();
            
            if (Input.GetMouseButtonUp(0))
                TryEndDrag();
        }

        private void TryStartDrag()
        {
            if (!RaycastPlayer()) return;
            _dragging = true;
            _startPos = Input.mousePosition;
        }
        
        private void TryEndDrag()
        {
            if (!_dragging) return;

            _dragging = false;

            Vector2 endPos = Input.mousePosition;
            Vector2 delta = endPos - _startPos;
            
            float threshold = Screen.height * _dragThresholdPercent;
            if (delta.magnitude < threshold) return;

            Vector2 dir = delta.normalized;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x > 0) _player.TryMoveTo(Direction.East);
                else _player.TryMoveTo(Direction.West);
            }
            else
            {
                if (dir.y > 0) _player.TryMoveTo(Direction.North);
                else _player.TryMoveTo(Direction.South);
            }
        }
        
        private bool RaycastPlayer()
        {
            Ray r = _cam.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(r, out var hit) && hit.collider.CompareTag("Player");
        }

        
        
        
        // private void HandleClick()
        // {
        //     var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        //     if (Physics.Raycast(ray, out RaycastHit hit))
        //     {
        //         if (TurnManager.Instance.CurrentTurn != TurnType.PlayerPlanning) return;
        //
        //         if (hit.collider.TryGetComponent(out PlayerController player))
        //         {
        //             _selectedPlayer = player;
        //             return;
        //         }
        //
        //         if (hit.collider.TryGetComponent(out Node node))
        //         {
        //             if (_selectedPlayer != null)
        //             {
        //                 _selectedPlayer.TryMoveTo(node);
        //                 _selectedPlayer = null;
        //             }
        //         }
        //     }
        // }
    }
}