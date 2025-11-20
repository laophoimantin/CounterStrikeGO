using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using Core;
using UnityEngine;
using Core.Events;
using Core.TurnSystem;
using Grid;


namespace Characters.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _playerModel;
        [SerializeField] private Node _currentNode;
        [Range(0.1f, 2f)] [SerializeField] private float _actionDurationModifier;
        private bool _isMoving = false;
        private bool _canMove = true;

        private Direction _tempMoveDirection = Direction.None;
        
        
        void OnEnable()
        {
            this.Subscribe<OnTurnChangedEvent>(HandleTurnChanged);
        }

        void OnDisable()
        {
            if (NewEventDispatcher.Instance != null)
                this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChanged);
        }

        void Start()
        {
            if (_currentNode == null)
                Debug.LogWarning($"{gameObject.name} has no node assigned!!!");

            if (_currentNode != null)
                _currentNode.PlacePlayer(this);
        }

        // Turn System =========================================================================
        private void HandleTurnChanged(OnTurnChangedEvent eventData)
        {
            _canMove = (eventData.NewTurn == TurnType.PlayerPlanning);
            if (eventData.NewTurn == TurnType.PlayerPlanning && _tempMoveDirection != Direction.None)
                TryMoveTo(_tempMoveDirection);
        }

        // Actions ====================================================        
        public void TryMoveTo(Direction direction)
        {
            _tempMoveDirection = direction;
            if (!_canMove || _isMoving) return;

            Node target = _tempMoveDirection switch
            {
                Direction.North => _currentNode.NorthNode,
                Direction.South => _currentNode.SouthNode,
                Direction.East => _currentNode.EastNode,
                Direction.West => _currentNode.WestNode,
                _ => null
            };

            if (target == null) return;
            StartCoroutine(MoveRoutine(target));
            _tempMoveDirection = Direction.None;
        }


        private IEnumerator MoveRoutine(Node target)
        {
            this.SendEvent(new OnPlayerActionStartedEvent());
            
            _isMoving = true;
            _canMove = false;

            Node origin = _currentNode;
            _currentNode = target;

            origin.RemovePlayer();
            target.PlacePlayer(this);


            Vector3 start = _playerModel.position;
            Vector3 end = new Vector3(target.transform.position.x, _playerModel.position.y,
                target.transform.position.z);

            float duration = TurnManager.Instance.GlobalActionDuration * _actionDurationModifier;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _playerModel.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            _playerModel.position = end;
            _isMoving = false;

            _currentNode.TriggerEffect(this);
            TryAttack(_currentNode);
        }
        

        private void TryAttack(Node targetNode)
        {
            var enemies = targetNode.Enemies;
            if (enemies.Count == 0)
            {
                this.SendEvent(new OnPlayerActionFinishedEvent());
                return;
            }
            EnemyManager.Instance.ResolveAttack(enemies, this);
        }


        
        
        public void Die()
        {
            StartCoroutine(DieAnim());
        }

        private IEnumerator DieAnim()
        {
            float duration = 1f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _playerModel.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }

            transform.localScale = Vector3.zero;
            this.SendEvent(new OnPlayerDeadEvent());
        }

        // Editor ====================================================================================

        #region Editor Methods

        public void SetOrMoveNode(Direction? dir = null)
        {
            Node newNode;
            string warningLog = dir.HasValue ? " No valid node to move to!" : " Invalid spot, couldn't find a node!";

            if (dir.HasValue)
            {
                newNode = GetNodeInDirection(_currentNode, dir.Value);
            }
            else
            {
                if (NodeManager.Instance == null)
                    Debug.LogWarning("No NodeManager instance found!", this);
                newNode = NodeManager.Instance.GetNodeFromWorldPosition(_playerModel.position);
            }

            if (_currentNode != null)
                _currentNode.RemovePlayer();


            if (newNode == null)
            {
                Debug.LogWarning(name + " has no valid node!" + warningLog, this);
                return;
            }

            _currentNode = newNode;

            SnapPosition(_currentNode.transform.position);
            _currentNode.PlacePlayer(this);
        }


        private void SnapPosition(Vector3 pos)
        {
            _playerModel.position = new Vector3(pos.x, transform.position.y, pos.z);
        }

        private Node GetNodeInDirection(Node node, Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return node.NorthNode;
                case Direction.South: return node.SouthNode;
                case Direction.East: return node.EastNode;
                case Direction.West: return node.WestNode;
                default: return null;
            }
        }

        #endregion
    }
}