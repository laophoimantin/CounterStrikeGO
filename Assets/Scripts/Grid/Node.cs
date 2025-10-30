using System.Collections.Generic;
using Characters.Enemy;
using Characters.Player;
using UnityEngine;

namespace Grid
{
    [RequireComponent(typeof(Collider))]
 public class Node : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private List<Node> _connectedNodes = new();
        private Renderer _renderer;

        [Header("Connection Settings")]
        [SerializeField, Tooltip("Max distance to detect connected nodes")]
        private float _connectionRadius = 5.1f;
        
        private PlayerController _occupyingPlayer;
        //Test
        [SerializeField] private List<EnemyController> _occupyingEnemies = new List<EnemyController>();

        [SerializeField] private bool _isOccupiedByPlayerByPlayer = false;
        private bool _isHighlighted = false;
        #endregion

        #region Public Fields

        public List<Node> ConnectedNodes => _connectedNodes;

        public bool IsOccupiedByPlayer
        {
            get => _isOccupiedByPlayerByPlayer;
            set
            {
                _isOccupiedByPlayerByPlayer = value;
                UpdateColor();
            }
        }

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
            if (!Application.isPlaying)
                FindConnectedNodes();
        }
#endif

        public void AssignPlayer(PlayerController player)
        {
            _occupyingPlayer = player;
            _isOccupiedByPlayerByPlayer = true;
            UpdateColor();
            AttackEnemy();
        }

        public void UnassignPlayer(PlayerController player)
        {
            _occupyingPlayer  = null;
            _isOccupiedByPlayerByPlayer = false;
            UpdateColor();
        }



        private void AttackEnemy()
        {
            if (_occupyingEnemies != null && _occupyingEnemies.Count > 0)
            {
                foreach (var enemy in _occupyingEnemies)
                    enemy.Die();
            }
            
        }
            
            
            
            
        public void AssignEnemy(EnemyController enemy)
        {
            if (!_occupyingEnemies.Contains(enemy))
            {
                _occupyingEnemies.Add(enemy);
            }
        }

        public void UnAssignEnemy(EnemyController enemy)
        {
            _occupyingEnemies.Remove(enemy);
        }
        
        


        public void Highlight(bool state)
        {
            _isHighlighted = state;
            UpdateColor();
        }
        
        private void UpdateColor()
        {
            if (_renderer == null) return;

            if (_isOccupiedByPlayerByPlayer)
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