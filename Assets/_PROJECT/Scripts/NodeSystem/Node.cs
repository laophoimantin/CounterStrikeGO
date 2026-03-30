using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Node Features")]
    [SerializeField] private BaseNodeFeature _feature;

    // ------------------------------------------------------------
    [Header("Core")]
    [SerializeField] private int _xValue;
    [SerializeField] private int _yValue;
    [SerializeField] private float _size;

    public Vector3 WorldPos => transform.position;
    // ------------------------------------------------------------
    [SerializeField] private bool _isObstacle;
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
    [Header("Crowd Control")]
    [SerializeField] private float _crowdRadius = 0.35f;

    // ------------------------------------------------------------
    [Header("Utility")]
    [SerializeField] private UtilityController _utilityItem;
    public bool HasUtilityItem => _utilityItem != null;
    private BaseZone _activeBaseZone;

    [Header("Debug")]
    [SerializeField] private TextMeshPro _textMesh;


    private void Awake()
    {
        if (_feature != null)
        {
            _feature.Initialize(this);
        }
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

    // Unit Management
    public void AddUnit(GridOccupant unit)
    {
        if (!_units.Contains(unit))
        {
            _units.Add(unit);
            RearrangeUnits();
        }
    }

    public void RemoveUnit(GridOccupant unit)
    {
        _units.Remove(unit);
        RearrangeUnits();
    }

    public bool HasUnit()
    {
        return _units.Count > 0;
    }

    public bool HasUnitsOfType<T>() where T : PawnUnit
    {
        return _units.Any(u => u is T);
    }

    public IEnumerable<GridOccupant> GetAllUnits()
    {
        return _units;
    }

    public IEnumerable<T> GetUnitsByType<T>() where T : PawnUnit
    {
        return _units.OfType<T>();
    }

    public T GetUnitByType<T>() where T : PawnUnit
    {
        foreach (var unit in _units)
        {
            if (unit is T typedUnit)
            {
                return typedUnit;
            }
        }

        return null;
    }

    private void RearrangeUnits()
    {
        var activeUnits = _units.Where(o => o.OccupiesSpace).ToList();

        //int count = _units.Count;
        int count = activeUnits.Count;
        if (count == 0) return;

        List<Vector3> slots = new List<Vector3>();

        if (count == 1)
        {
            slots.Add(Vector3.zero);
        }
        else
        {
            float angleStep = 360f / count;
            for (int i = 0; i < count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * _crowdRadius;
                float z = Mathf.Sin(angle) * _crowdRadius;
                slots.Add(new Vector3(x, 0, z));
            }
        }

        List<GridOccupant> sortedUnits = activeUnits.OrderByDescending(u =>
            Vector3.SqrMagnitude(u.transform.position - transform.position)
        ).ToList();

        List<Vector3> availableSlots = new List<Vector3>(slots);

        foreach (GridOccupant unit in sortedUnits)
        {
            Vector3 bestSlot = Vector3.zero;
            float bestDist = float.MaxValue;
            int bestSlotIndex = -1;

            for (int i = 0; i < availableSlots.Count; i++)
            {
                Vector3 worldSlotPos = transform.TransformPoint(availableSlots[i]);
                float dist = Vector3.SqrMagnitude(unit.transform.position - worldSlotPos);

                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestSlot = availableSlots[i];
                    bestSlotIndex = i;
                }
            }

            if (bestSlotIndex != -1)
            {
                unit.SetVisualOffset(bestSlot);
                availableSlots.RemoveAt(bestSlotIndex);
            }
        }
    }

    // Neighbors ========================================================================================================================
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

        if (pawnUnit is PlayerController player)
        {
            if (HasUtilityItem)
            {
                player.EquipUtility(_utilityItem);
                _utilityItem = null;
            }
        }
    }

    // Utility ====================
    public void AddUtility(UtilityController utility)
    {
        _utilityItem = utility;
    }

    public void RemoveUtility()
    {
        _utilityItem = null;
    }

    // Zone ==================================================================================================
    public void AddZone(BaseZone newBaseZone)
    {
        if (_activeBaseZone != null)
        {
            _activeBaseZone.Expire();
        }

        _activeBaseZone = newBaseZone;
    }

    public void RemoveZone()
    {
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


    //Editor only
    public void DeleteSelf()
    {
        DetachLink(_north, n => n._south = null);
        DetachLink(_south, n => n._north = null);
        DetachLink(_east, n => n._west = null);
        DetachLink(_west, n => n._east = null);

        void DetachLink(Node neighbour, Action<Node> unlink)
        {
            if (neighbour != null)
                unlink(neighbour);
        }

        DestroyImmediate(gameObject);
    }

    // Gizmos ================================================================================================
#if UNITY_EDITOR
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

        //Gizmos.DrawWireSphere(center, 0.5f);

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