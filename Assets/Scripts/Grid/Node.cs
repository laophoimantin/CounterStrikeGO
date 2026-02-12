using System;
using System.Collections.Generic;
using System.Linq;
using Pawn;
using Core;
using TMPro;
using UnityEngine;

namespace Grid
{
    public class Node : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private TextMeshPro _textMesh;
        // ------------------------------------------------------------
        [Header("Core")]
        [SerializeField] private int _xValue;
        [SerializeField] private int _yValue;
        [SerializeField] private float _size;

        public Vector3 WorldPos => transform.position;

        // ------------------------------------------------------------
        [SerializeField] public bool IsObstacle;
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
        private List<GridUnit> _units = new();

        // ------------------------------------------------------------
        [Header("Crowd Control")]
        [SerializeField] private float _crowdRadius = 0.35f;

        // ------------------------------------------------------------
        private INodeEffect[] _effects;

        private void Awake()
        {
            _effects = GetComponents<INodeEffect>();
            foreach (var effect in _effects)
            {
                effect.Initialize(this);
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
        public void AddUnit(GridUnit unit)
        {
            if (!_units.Contains(unit))
            {
                _units.Add(unit);
                RearrangeUnits();
            }
        }

        public void RemoveUnit(GridUnit unit)
        {
            _units.Remove(unit);
            RearrangeUnits();
        }

        public bool HasPlayer()
        {
            return _units.Any(u => u.Team == TeamSide.Player);
        }

        public bool HasEnemy()
        {
            return _units.Any(u => u.Team == TeamSide.Enemy);
        }

        public GridUnit GetPlayer()
        {
            foreach (GridUnit unit in _units)
            {
                if (unit.Team == TeamSide.Player)
                {
                    return unit;
                }
            }

            return null;
        }

        public List<GridUnit> GetEnemies()
        {
            List<GridUnit> enemiesFound = new List<GridUnit>();

            foreach (GridUnit unit in _units)
            {
                if (unit.Team == TeamSide.Enemy)
                {
                    enemiesFound.Add(unit);
                }
            }

            return enemiesFound;
        }


        public List<GridUnit> GetAllUnits()
        {
            return _units;
        }


       public void RearrangeUnits()
    {
        int count = _units.Count;
        if (count == 0) return;

        // 1. Generate Slots (Same as before)
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

        // 2. THE LOGIC FLIP: Sort Units by Distance from Center (Furthest First)
        // The unit entering from the neighbor tile is distance ~1.0
        // The unit standing on the node is distance ~0.0
        // We want the Entering unit to pick first.
        
        List<GridUnit> sortedUnits = _units.OrderByDescending(u => 
            Vector3.SqrMagnitude(u.transform.position - transform.position)
        ).ToList();

        List<Vector3> availableSlots = new List<Vector3>(slots);

        // 3. Assign Slots
        foreach (GridUnit unit in sortedUnits)
        {
            Vector3 bestSlot = Vector3.zero;
            float bestDist = float.MaxValue;
            int bestSlotIndex = -1;

            // Find the slot closest to THIS unit's current position (Entrance)
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

            // Assign and remove the slot so nobody else takes it
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

        public void TriggerNode(GridUnit unit)
        {
            if (_effects == null || _effects.Length == 0) return;

            foreach (var effect in _effects)
            {
                effect.OnEnter(unit);
            }
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

            if (IsObstacle)
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(center, 0.5f);

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
}