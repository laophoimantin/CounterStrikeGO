using System.Collections.Generic;
using Characters.Enemy;
using Characters.Player;
using UnityEngine;

namespace Grid
{
    [RequireComponent(typeof(Collider))]
 public class OldNode
        : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private List<OldNode> _connectedNodes = new();
        [SerializeField] private OldNode _northNode;
        [SerializeField] private OldNode _southNode;
        [SerializeField] private OldNode _eastNode;
        [SerializeField] private OldNode _westNode;
        private Renderer _renderer;

        [Header("Connection Settings")]
        [SerializeField, Tooltip("Max distance to detect connected nodes")]
        private float _connectionRadius = 5.1f;
        
        [SerializeField] private List<EnemyController> _enemiesOnNode = new();
        [SerializeField] private PlayerController _playerOnNode;
        private bool _isObstacle= false;
        private bool _isHighlighted = false;
        #endregion

        #region Public Fields
        
        public OldNode NorthNode => _northNode;
        public OldNode SouthNode => _southNode;
        public OldNode EastNode => _eastNode;
        public OldNode WestNode => _westNode;
        

        public List<OldNode> ConnectedNodes => _connectedNodes;
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
            _renderer = GetComponent<Renderer>();
        }

        void Start()
        {
            if (_renderer != null)
                _renderer.material.color = Color.green;
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
            UpdateColor();
            AttackEnemy();
        }

        public void UnassignPlayer(PlayerController player)
        {
            _playerOnNode = null;
            UpdateColor();
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
            
        


        
        // Visualization
        public void Highlight(bool state)
        {
            _isHighlighted = state;
            UpdateColor();
        }
        
        private void UpdateColor()
        {
            if (_renderer == null) return;

            if (HasPlayer())
            {
                _renderer.material.color = Color.cyan;
            }
            else if (_isHighlighted)
            {
                _renderer.material.color = Color.yellow;
            }
            else
            {
                _renderer.material.color = Color.green;
            }
        }

        
        
        

        private void FindConnectedNodes()
        {
            _connectedNodes.Clear();

            OldNode[] allNodes = FindObjectsOfType<OldNode>();

            foreach (OldNode node in allNodes)
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