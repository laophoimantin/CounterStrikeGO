using System.Collections.Generic;
using Characters.Enemy;
using Characters.Player;
using UnityEngine;

namespace Grid
{
    [ExecuteInEditMode]
    public class Node
        : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private List<Node> _connectedNodes = new();
        [SerializeField] private Node _northNode;
        [SerializeField] private Node _southNode;
        [SerializeField] private Node _eastNode;
        [SerializeField] private Node _westNode;

        [Header("Connection Settings")] [SerializeField, Tooltip("Max distance to detect connected nodes")]
        private float _connectionRadius = 5.1f;

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
            FindConnectedNodes();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //Testing
            if (!Application.isPlaying)
                FindConnectedNodes();
        }
#endif

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

        [ContextMenu("Auto-Link Neighbors")]
        private void LinkNeighbors()
        {
            NodeManager.Instance.RegisterNodes();
            
            Vector2Int myPos = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.z)
            );

            _northNode = NodeManager.Instance.GetNodeAtPosition(myPos + Vector2Int.up);
            _southNode = NodeManager.Instance.GetNodeAtPosition(myPos + Vector2Int.down);
            _eastNode  = NodeManager.Instance.GetNodeAtPosition(myPos + new Vector2Int(1, 0));
            _westNode  = NodeManager.Instance.GetNodeAtPosition(myPos + new Vector2Int(-1, 0));
        
            Debug.Log("Linked neighbors for " + name);
        }
        
        
      

        private void FindConnectedNodes()
        {
            _connectedNodes.Clear();

            Node[] allNodes = FindObjectsOfType<Node>();

            foreach (Node node in allNodes)
            {
                if (node == this) continue;

                float distance = Vector3.Distance(transform.position, node.transform.position);
                if (distance <= _connectionRadius)
                {
                    _connectedNodes.Add(node);
                }
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (var connected in _connectedNodes)
            {
                if (connected == null) continue;
                Gizmos.DrawLine(transform.position, connected.transform.position);
            }
        }
    }
}