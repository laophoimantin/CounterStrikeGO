using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn
{
    public class ClickHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController _player;
        private Camera _cam;


        [Header("Settings")]
        [SerializeField] private LayerMask _playerLayer;
        [SerializeField] private LayerMask _nodeLayer;
        [SerializeField] private float _dragThresholdPercent = 0.05f;

        private bool _dragging = false;
        private Vector2 _startPos;

        void Start()
        {
            if (_cam == null)
                _cam = Camera.main;
        }

        private void Update()
        {
            if (_player.HasUtility)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    HandleUtilityClick();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                    TryStartDrag();

                if (Input.GetMouseButtonUp(0))
                    TryEndDrag();
            }
        }

        private void HandleUtilityClick()
        {
            Ray r = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out var hit, Mathf.Infinity, _nodeLayer))
            {
                Node clickedNode = hit.collider.GetComponent<Node>();
            
                if (clickedNode != null)
                {
                    _player.TryUseUtility(clickedNode);
                }
            }
        }
        
        
        private void TryStartDrag()
        {
            if (!RaycastPlayer()) return;
            _dragging = true;
            _startPos = Input.mousePosition;

            _player.OnPickedUp();
        }

        private void TryEndDrag()
        {
            if (!_dragging) return;

            _dragging = false;

            _player.OnDropped();

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
            if (Physics.Raycast(r, out var hit))
            {
                return hit.collider.CompareTag("Player") || hit.transform.root.CompareTag("Player");
            }

            return false;
        }
    }
}