using System.Collections.Generic;
using Characters.Enemy;
using Characters.Player;
using Core;
using TMPro;
using UnityEngine;

namespace Grid
{
    public class Node : MonoBehaviour
    {
        // ------------------------------------------------------------
        [Header("Core")] [SerializeField] private int _xValue;
        [SerializeField] private int _yValue;
        [SerializeField] private float _size;
        [SerializeField] private Vector3 _worldPos;

        [SerializeField] private TextMeshPro _textMesh;

        // ------------------------------------------------------------
        [Header("Type / Obstacle")] [SerializeField]
        private bool _isObstacle = false;

        [SerializeField] private NodeType _nodeType = NodeType.Normal;

        public NodeType NodeType => _nodeType;

        public bool IsObstacle => _isObstacle;

        // ------------------------------------------------------------
        [Header("Neighbours")] [SerializeField]
        private List<Node> _connectedNodes = new();

        [SerializeField] private Node _north;
        [SerializeField] private Node _south;
        [SerializeField] private Node _east;
        [SerializeField] private Node _west;

        public List<Node> ConnectedNodes => _connectedNodes;
        public Node NorthNode => _north;
        public Node SouthNode => _south;
        public Node EastNode => _east;
        public Node WestNode => _west;

        // ------------------------------------------------------------
        [Header("Occupancy")] [SerializeField] private List<EnemyController> _enemies = new();
        [SerializeField] private PlayerController _player;

        public bool HasPlayer => _player != null;
        public bool HasEnemies => _enemies.Count > 0;
        public PlayerController Player => _player;
        public List<EnemyController> Enemies => _enemies;


        public void Initialize(int x, int y, Vector3 worldPos, float size)
        {
            _xValue = x;
            _yValue = y;
            _size = size;
            _worldPos = worldPos;

            name = $"({x}, {y})";
            _textMesh.text = name;
        }

        public Vector2Int Get2DCoordinate()
        {
            Vector2Int coordinate = new Vector2Int(_xValue, _yValue);
            return coordinate;
        }


        // Neighbors ========================================================================================================================
        public void AssignNeighbour(Node other, Direction dir)
        {
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

        // Occupancy logic ================================================================================================

        public void PlacePlayer(PlayerController player)
        {
            _player = player;
        }

        public void RemovePlayer()
        {
            _player = null;
        }


        public void AddEnemy(EnemyController enemy)
        {
            if (!_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        public void UnAssignEnemy(EnemyController enemy)
        {
            _enemies.Remove(enemy);
        }

        public void TriggerEffect(PlayerController player)
        {
            switch (NodeType)
            {
                case NodeType.Normal:
                    break;
                case NodeType.HidingSpot:
                    break;
                case NodeType.Trapdoor:
                    break;
                case NodeType.Item:
                    break;
                case NodeType.Objective:
                    GameManager.Instance.OnPlayerPickedUpObjective();
                    _nodeType = NodeType.Normal;
                    break;
                case NodeType.Exit:
                    GameManager.Instance.CheckWinCondition();
                    break;
            }
        }


        // Gizmos ================================================================================================
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            float size = _size;
            Vector3 half = new Vector3(size * 0.5f, 0, size * 0.5f);
            Vector3 center = new Vector3(_worldPos.x, 0, _worldPos.z);
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
            else
            {
                switch (_nodeType)
                {
                    case NodeType.Normal:
                        Gizmos.color = Color.green;
                        break;
                    case NodeType.HidingSpot:
                        Gizmos.color = Color.blue;
                        break;
                    case NodeType.Trapdoor:
                        Gizmos.color = Color.magenta;
                        break;
                    case NodeType.Exit:
                        Gizmos.color = Color.yellow;
                        break;
                    case NodeType.Item:
                        Gizmos.color = Color.cyan;
                        break;
                }
            }

            Gizmos.DrawWireSphere(center, 0.5f);


            Gizmos.color = Color.cyan;

            if (_north != null)
            {
                Gizmos.DrawLine(transform.position, _north.transform.position);
            }

            if (_south != null)
            {
                Gizmos.DrawLine(transform.position, _south.transform.position);
            }

            if (_east != null)
            {
                Gizmos.DrawLine(transform.position, _east.transform.position);
            }

            if (_west != null)
            {
                Gizmos.DrawLine(transform.position, _west.transform.position);
            }
        }
    }
}