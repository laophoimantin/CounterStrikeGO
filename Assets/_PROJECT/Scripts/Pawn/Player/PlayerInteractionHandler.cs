using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    public static bool IsPlayerInteracting { get; private set; }

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
        {
            _cam = Camera.main;
        }

        IsPlayerInteracting = false;
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
            {
                TryStartDrag();
                if (_dragging)
                {
                    IsPlayerInteracting = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryEndDrag();
                IsPlayerInteracting = false;
            }
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
                //_player.TryUseUtility(clickedNode);
                _player.Input_TryUseUtility(clickedNode);
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

        Vector2 dragDir = delta.normalized;

        
        Vector3 playerWorldPos = _player.transform.position;
        Vector2 playerScreenPos = _cam.WorldToScreenPoint(playerWorldPos);

        Vector2 screenNorth = ((Vector2)_cam.WorldToScreenPoint(playerWorldPos + Vector3.forward) - playerScreenPos).normalized;
        Vector2 screenSouth = ((Vector2)_cam.WorldToScreenPoint(playerWorldPos + Vector3.back) - playerScreenPos).normalized;
        Vector2 screenEast  = ((Vector2)_cam.WorldToScreenPoint(playerWorldPos + Vector3.right) - playerScreenPos).normalized;
        Vector2 screenWest  = ((Vector2)_cam.WorldToScreenPoint(playerWorldPos + Vector3.left) - playerScreenPos).normalized;

        float dotNorth = Vector2.Dot(dragDir, screenNorth);
        float dotSouth = Vector2.Dot(dragDir, screenSouth);
        float dotEast  = Vector2.Dot(dragDir, screenEast);
        float dotWest  = Vector2.Dot(dragDir, screenWest);

        float maxDot = Mathf.Max(dotNorth, Mathf.Max(dotSouth, Mathf.Max(dotEast, dotWest)));

        if (maxDot == dotNorth) 
            _player.TryMoveTo(Direction.North);
        else if (maxDot == dotSouth) 
            _player.TryMoveTo(Direction.South);
        else if (maxDot == dotEast) 
            _player.TryMoveTo(Direction.East);
        else if (maxDot == dotWest) 
            _player.TryMoveTo(Direction.West);
    }
    // private void TryEndDrag()
    // {
    //     if (!_dragging) return;
    //
    //     _dragging = false;
    //
    //     _player.OnDropped();
    //
    //     Vector2 endPos = Input.mousePosition;
    //     Vector2 delta = endPos - _startPos;
    //
    //     float threshold = Screen.height * _dragThresholdPercent;
    //
    //     if (delta.magnitude < threshold) return;
    //
    //     Vector2 dir = delta.normalized;
    //
    //     if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
    //     {
    //         if (dir.x > 0) _player.TryMoveTo(Direction.East);
    //         else _player.TryMoveTo(Direction.West);
    //     }
    //     else
    //     {
    //         if (dir.y > 0) _player.TryMoveTo(Direction.North);
    //         else _player.TryMoveTo(Direction.South);
    //     }
    // }

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