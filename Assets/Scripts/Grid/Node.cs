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
        private INodeEffect[] _effects;

        private void Awake()
        {
            _effects = GetComponents<INodeEffect>();
            foreach(var effect in _effects)
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


        // Unit Management
        public void AddUnit(GridUnit unit)
        {
            if (!_units.Contains(unit))
            {
                _units.Add(unit);
            }
        }

        public void RemoveUnit(GridUnit unit)
        {
            _units.Remove(unit);
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