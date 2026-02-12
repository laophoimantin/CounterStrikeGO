using System;
using System.Collections;
using UnityEngine;
using Core.Events;
using Core.TurnSystem;
using DG.Tweening;
using Grid;

namespace Pawn
{
    public class PlayerController : GridUnit
    {
        public override TeamSide Team => TeamSide.Player;

        [Header("Component References")]
        [SerializeField] private Transform _playerModel;
        public override Transform VisualModel => _playerModel;

        [SerializeField] private Node _currentNode;
        [Range(0.1f, 2f)] [SerializeField] private float _actionDurationModifier;
        private bool _isMoving = false;
        private bool _canMove = true;

        private Direction _tempMoveDirection = Direction.None;

        [Header("Visual Feedback")]
        [SerializeField] private float _liftHeight = 0.5f;
        [SerializeField] private float _liftDuration = 0.2f;

        void OnEnable()
        {
            this.Subscribe<OnTurnChangedEvent>(HandleTurnChanged);
        }

        void OnDisable()
        {
            if (EventDispatcher.Instance != null)
                this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChanged);
        }

        void Start()
        {
            if (_currentNode != null)
            {
                _currentNode.AddUnit(this);
                transform.position = _currentNode.WorldPos;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} has no node assigned!");
            }
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

            if (target == null || target.IsObstacle)
            {
                _tempMoveDirection = Direction.None;
                return;
            }

            StartCoroutine(Move(target));
            _tempMoveDirection = Direction.None;
        }


        private IEnumerator Move(Node targetNode)
        {
            this.SendEvent(new OnPlayerActionStartedEvent());

            _isMoving = true;
            _canMove = false;

            UpdateNodeData(targetNode);

            Vector3 startPos = transform.position;
            Vector3 endPos = targetNode.WorldPos;

            float duration = TurnManager.Instance.GlobalActionDuration * _actionDurationModifier;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            transform.position = endPos;
            _isMoving = false;
            _currentNode.TriggerNode(this);

            TryAttack(_currentNode);
        }


        private void TryAttack(Node targetNode)
        {
            if (targetNode.HasEnemy())
            {
                var enemies = targetNode.GetEnemies();
                EnemyManager.Instance.ResolveAttack(enemies, FinishAction);
            }
            else
            {
                this.SendEvent(new OnPlayerActionFinishedEvent());
            }
        }

        private void UpdateNodeData(Node newNode)
        {
            if (newNode == null) return;

            if (_currentNode != null)
            {
                _currentNode.RemoveUnit(this);
            }

            _currentNode = newNode;
            _currentNode.AddUnit(this);
        }

        private void FinishAction()
        {
            this.SendEvent(new OnPlayerActionFinishedEvent());
        }

        public override void Die(Action onDeathComplete = null)
        {
            StartCoroutine(DieAnim());
        }

        // Testing
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

        // On Click
        // On Click ====================================================================================
        public void OnPickedUp()
        {
            _playerModel.DOKill();

            _playerModel.DOLocalMoveY(_liftHeight, _liftDuration).SetEase(Ease.OutBack);
        }

        public void OnDropped()
        {
            _playerModel.DOKill();

            _playerModel.DOLocalMoveY(0f, _liftDuration).SetEase(Ease.OutBounce);
        }

        // Editor ====================================================================================

        #region Editor Methods

#if UNITY_EDITOR
        public void SetOrMoveNode(Direction? dir = null)
        {
            if (NodeManager.Instance == null)
            {
                // Try to find it if an instance is missing (common in Editor mode)
                var found = FindObjectOfType<NodeManager>();
                if (found == null) return;
            }

            Node newNode;

            if (dir.HasValue)
            {
                newNode = GetNodeInDirection(_currentNode, dir.Value);
            }
            else
            {
                // Snap to nearest node
                newNode = FindObjectOfType<NodeManager>().GetNodeFromWorldPosition(transform.position);
            }

            if (newNode == null)
            {
                Debug.LogWarning("No valid node found!");
                return;
            }

            // Logic
            if (_currentNode != null) _currentNode.RemoveUnit(this);
            _currentNode = newNode;
            _currentNode.AddUnit(this);

            // Visual Snap
            transform.position = _currentNode.transform.position;

            UnityEditor.EditorUtility.SetDirty(this);
        }

        private Node GetNodeInDirection(Node node, Direction dir)
        {
            if (node == null) return null;
            return dir switch
            {
                Direction.North => node.NorthNode,
                Direction.South => node.SouthNode,
                Direction.East => node.EastNode,
                Direction.West => node.WestNode,
                _ => null
            };
        }
#endif

        #endregion
    }
}