using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NodeVisual _nodeVisual;

    [Header("Node Features")]
    [SerializeField] private BaseNodeFeature _feature;

    public Action OnOccupancyChanged;

    // ------------------------------------------------------------
    [Header("Core")]
    [SerializeField] private int _xValue;
    [SerializeField] private int _yValue;
    [SerializeField] private float _size;
    public int XValue => _xValue;
    public int YValue => _yValue;

    public Vector3 WorldPos => transform.position;
    // ------------------------------------------------------------
    [SerializeField] private bool _isObstacle;
    public bool IsObstacle => _isObstacle;
    // ------------------------------------------------------------
    [Header("Neighbours")]
    [SerializeField] private Node _north;
    [SerializeField] private Node _south;
    [SerializeField] private Node _east;
    [SerializeField] private Node _west;

    public Node NorthNode => _north;
    public Node SouthNode => _south;
    public Node EastNode => _east;
    public Node WestNode => _west;

    // ------------------------------------------------------------
    [Header("Occupancy")]
    private List<GridOccupant> _units = new();

    // ------------------------------------------------------------
    [Header("Utility")]
    [SerializeField] private IPickupable _item;
    private bool HasItem => _item != null;

    private BaseZone _activeBaseZone;

    [Header("Debug")]
    [SerializeField] private TextMeshPro _textMesh;


    private void Awake()
    {
        if (_feature != null)
        {
            _feature.Initialize(this);
        }

        _nodeVisual.SetupVisual(_feature, _isObstacle);
    }

    public void Initialize(int x, int y, float size)
    {
        _xValue = x;
        _yValue = y;
        _size = size;

        name = $"({x}, {y})";
        _textMesh.text = name;
    }


    public Vector2Int Get2DCoordinate()
    {
        Vector2Int coordinate = new Vector2Int(_xValue, _yValue);
        return coordinate;
    }

    public void Start()
    {
        _textMesh.gameObject.SetActive(false);
    }

    // Unit Management =================================================================================================
    public void AddUnit(GridOccupant unit)
    {
        if (!_units.Contains(unit))
        {
            _units.Add(unit);
            OnOccupancyChanged?.Invoke();
        }
    }

    public void RemoveUnit(GridOccupant unit)
    {
        _units.Remove(unit);
        OnOccupancyChanged?.Invoke();
    }

    public IReadOnlyList<GridOccupant> GetAllOccupants()
    {
        return _units;
    }

    // Neighbors =======================================================================================================
    public Node GetNodeInDirection(Direction dir)
    {
        return dir switch
        {
            Direction.North => _north,
            Direction.South => _south,
            Direction.East => _east,
            Direction.West => _west,
            _ => null
        };
    }

    public void AssignNeighbour(Node other, Direction dir)
    {
        if (other == null) return;

        switch (dir)
        {
            case Direction.North:
                _north = other;
                other._south = this;
                break;

            case Direction.South:
                _south = other;
                other._north = this;
                break;

            case Direction.East:
                _east = other;
                other._west = this;
                break;

            case Direction.West:
                _west = other;
                other._east = this;
                break;
        }

        _isObstacle = false;
    }

    public void RemoveNeighbour(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                if (_north != null)
                {
                    _north._south = null;
                    _north = null;
                }

                break;

            case Direction.South:
                if (_south != null)
                {
                    _south._north = null;
                    _south = null;
                }

                break;

            case Direction.East:
                if (_east != null)
                {
                    _east._west = null;
                    _east = null;
                }

                break;

            case Direction.West:
                if (_west != null)
                {
                    _west._east = null;
                    _west = null;
                }

                break;
        }
    }

    public IEnumerable<Node> GetNeighbour()
    {
        if (_north != null) yield return _north;
        if (_south != null) yield return _south;
        if (_east != null) yield return _east;
        if (_west != null) yield return _west;
    }

    public void TriggerEnter(PawnUnit pawnUnit)
    {
        if (_feature != null)
        {
            _feature.OnEnter(pawnUnit);
        }

        if (HasItem)
        {
            _item.OnPickUpBy(pawnUnit);
        }
    }

    // Utility =========================================================================================================
    public void AddItem(IPickupable utility)
    {
        _item = utility;
    }

    public void RemoveItem()
    {
        _item = null;
    }

    // Zone ============================================================================================================
    public void AddZone(BaseZone newBaseZone)
    {
        var oldZone = _activeBaseZone;
        _activeBaseZone = newBaseZone;
        oldZone?.Expire();
    }

    public void RemoveZone(BaseZone oldZone)
    {
        if (_activeBaseZone == oldZone)
            _activeBaseZone = null;
    }

    public bool IsWalkable()
    {
        if (_isObstacle) return false;

        if (_activeBaseZone != null && !_activeBaseZone.IsWalkable())
            return false;

        return true;
    }

    public bool IsHideable()
    {
        if (_activeBaseZone != null && _activeBaseZone.IsHideable())
            return true;

        return false;
    }

    public void ToggleHighlight(bool isOn)
    {
        if (_nodeVisual != null && !_isObstacle)
        {
            _nodeVisual.ToggleAttackRangeHighlight(isOn);
        }
    }


#if UNITY_EDITOR
    public void IsolateNode()
    {
        _isObstacle = true;
    }

    public void ReLinkNode()
    {
        UnityEditor.Undo.RecordObject(this, "Relink Node");
        _isObstacle = false;

        Node[] allNodes = FindObjectsOfType<Node>();
        Debug.Log(allNodes.Length);
        foreach (Node n in allNodes)
        {
            if (n == this) continue;

            UnityEditor.Undo.RecordObject(n, "Relink Neighbor");

            if (n.XValue == _xValue && n.YValue == _yValue + 1)
            {
                _north = n;
                n._south = this;
                n._isObstacle = false;
            }

            else if (n.XValue == _xValue && n.YValue == _yValue - 1)
            {
                _south = n;
                n._north = this;
                n._isObstacle = false;
            }

            else if (n.XValue == _xValue + 1 && n.YValue == _yValue)
            {
                _east = n;
                n._west = this;
                n._isObstacle = false;
            }

            else if (n.XValue == _xValue - 1 && n.YValue == _yValue)
            {
                _west = n;
                n._east = this;
                n._isObstacle = false;
            }

            UnityEditor.EditorUtility.SetDirty(n);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[Tool] Node ({XValue}, {YValue}) linked successfully");
    }
 
    // Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float size = _size;
        Vector3 half = new Vector3(size * 0.5f, 0, size * 0.5f);
        Vector3 center = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 bottomLeft = center - half;
        Gizmos.DrawLine(bottomLeft, bottomLeft + Vector3.right * size);
        Gizmos.DrawLine(bottomLeft, bottomLeft + Vector3.forward * size);
        Gizmos.DrawLine(bottomLeft + Vector3.right * size,
            bottomLeft + Vector3.right * size + Vector3.forward * size);
        Gizmos.DrawLine(bottomLeft + Vector3.forward * size,
            bottomLeft + Vector3.forward * size + Vector3.right * size);

        if (_isObstacle)
        {
            Gizmos.color = Color.red;
        }

        Gizmos.color = Color.cyan;
        if (_north) DrawConnection(_north);
        if (_east) DrawConnection(_east);
    }

    private void DrawConnection(Node other)
    {
        Gizmos.DrawLine(transform.position, other.transform.position);
    }
#endif
}