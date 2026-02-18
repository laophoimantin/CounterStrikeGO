using Core.Events;
using Core.TurnSystem;
using Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pawn
{
    public class PlayerController : GridUnit
    {

        [Header("References")]
        private PlayerVisual _playerVisual;

        [Range(0.1f, 2f)][SerializeField] private float _actionDurationModifier;
        private bool _isMoving = false;
        private bool _canMove = true;

        private Direction _tempMoveDirection = Direction.None;
        private UtilityController _currentUtility;
        private bool _hasUtility = false;
        public bool HasUtility => _hasUtility;

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

            _playerVisual = _visual as PlayerVisual;
            if (_playerVisual == null)
                Debug.Log("VISUALLLLLL!");
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

            if (target == null || !target.IsWalkable())
            {
                _tempMoveDirection = Direction.None;
                return;
            }

            StartAction();
            StartCoroutine(Move(target));
            _tempMoveDirection = Direction.None;
        }


        private IEnumerator Move(Node targetNode)
        {
            _isMoving = true;
            _canMove = false;

            UpdateNodeData(targetNode);

            float duration = TurnManager.Instance.GlobalActionDuration * _actionDurationModifier;
            yield return _visual.MoveTo(targetNode.WorldPos, duration);

            _isMoving = false;
            _currentNode.TriggerEnter(this);

            yield return PostMove(_currentNode);
            FinishAction(ShouldEndTurn());
        }


        private IEnumerator PostMove(Node targetNode)
        {
            if (targetNode.HasUnitsOfType<EnemyController>())
            {
                var enemies = targetNode.GetUnitsByType<EnemyController>().ToList();
                yield return Attack(enemies);
            }
        }
        private IEnumerator Attack(List<EnemyController> enemies)
        {
            int pending = enemies.Count;

            foreach (var enemy in enemies)
                enemy.Terminate(() => pending--);

            while (pending > 0)
                yield return null;
        }

        private bool ShouldEndTurn()
        {
            if (_hasUtility)
            {
                return false;
            }
                return true;
        }

        private void StartAction()
        {
            this.SendEvent(new OnPlayerActionStartedEvent());
        }
        private void FinishAction(bool shouldEnd)
        {
            this.SendEvent(new OnPlayerActionFinishedEvent(shouldEnd));
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

        public override void Terminate(Action onDeathComplete = null)
        {
            StartCoroutine(_visual.DeadAnim(1f, () =>
            {
                OnDeathEvent(onDeathComplete);
            }));
        }

        private void OnDeathEvent(Action onDeathComplete = null)
        {
            this.SendEvent(new OnPlayerDeadEvent());
            onDeathComplete?.Invoke();
        }


        public void EquipUtility(UtilityController newUtility)
        {

            _currentUtility = newUtility;
            _currentUtility.OnPickUp(this);
            _hasUtility = true;
            _playerVisual.SwitchUtilityModel(_hasUtility);
        }

        public void TryUseUtility(Node targetNode)
        {
            if (!_hasUtility) return;
            List<Node> validNodes = NodeManager.Instance.GetNodesInRange(_currentNode, _currentUtility.ThrowRange);
            if (validNodes.Contains(targetNode))
            {
                StartAction();
                UseUtility(targetNode);
            }
        }

        private void UseUtility(Node targetNode)
        {
            bool endsTurn = _currentUtility.EndsTurn;
            _currentUtility.Throw(targetNode,() => FinishAction(endsTurn));
            _currentUtility = null; // Unequip utility
            _hasUtility = false;
            _playerVisual.SwitchUtilityModel(_hasUtility);
        }

        // On Click ====================================================================================
        public void OnPickedUp()
        {
            _playerVisual.PickedUpAnim();
        }

        public void OnDropped()
        {
            _playerVisual.DroppedAnim();
        }


        // Editor ====================================================================================

        #region Editor Methods

#if UNITY_EDITOR
        public void SetOrMoveNode(Direction? dir = null)
        {
            if (NodeManager.Instance == null)
            {
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
            if (_currentNode != null)
            {
                _currentNode.RemoveUnit(this);
                UnityEditor.EditorUtility.SetDirty(_currentNode);
            }
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