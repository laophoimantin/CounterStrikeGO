using System.Collections.Generic;
using Characters.Enemy;
using Characters.Player;
using UnityEngine;

namespace Grid
{
    [ExecuteInEditMode]
    public class Node : MonoBehaviour
    {



        #region Private Fields

        private int _xValue , _yValue;
        private float _size;
        private Vector3 _position;
        




        [SerializeField] private List<Node> _connectedNodes = new();
        [SerializeField] private Node _northNode;
        [SerializeField] private Node _southNode;
        [SerializeField] private Node _eastNode;
        [SerializeField] private Node _westNode;


        [SerializeField] private List<EnemyController> _enemiesOnNode = new();
        [SerializeField] private PlayerController _playerOnNode;
        private bool _isObstacle = false;

        #endregion

        #region Public Fields

        public Node NorthNode => _northNode;
        public Node SouthNode => _southNode;
        public Node EastNode => _eastNode;
        public Node WestNode => _westNode;


        public List<Node> ConnectedNodes => _connectedNodes;
        public PlayerController PlayerOnNode => _playerOnNode;


        public bool HasPlayer()
        {
            return _playerOnNode != null;
        }


        public bool HasEnemy()
        {
            return _enemiesOnNode.Count > 0;
        }

        public bool IsObstacle => _isObstacle;

        #endregion

        void Awake()
        {

        }

        void Start()
        {

        }
        public void AssignValue(int x, int y, float size, Vector3 pos)
        {
            _xValue = x;
            _yValue = y;
            _size = size;
            _position = pos;
        }

        public void AssignPlayer(PlayerController player)
        {
            _playerOnNode = player;
            AttackEnemy();
        }

        public void UnassignPlayer(PlayerController player)
        {
            _playerOnNode = null;
        }


        public void AssignEnemy(EnemyController enemy)
        {
            if (!_enemiesOnNode.Contains(enemy))
            {
                _enemiesOnNode.Add(enemy);
            }
        }

        public void UnAssignEnemy(EnemyController enemy)
        {
            _enemiesOnNode.Remove(enemy);
        }

        private void AttackEnemy()
        {
            if (_enemiesOnNode != null && _enemiesOnNode.Count > 0)
            {
                foreach (var enemy in _enemiesOnNode)
                    enemy.Die();
            }
        }

        public Vector2Int GetCoordinate()
        {
            Vector2Int coordinate = new Vector2Int(_xValue, _yValue);
            return coordinate;
        }
    }
}