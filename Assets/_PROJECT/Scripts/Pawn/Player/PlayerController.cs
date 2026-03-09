using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.TurnSystem;
using DG.Tweening;
using Grid;
using UnityEditor;
using UnityEngine;

namespace Pawn
{
    public class PlayerController : PawnUnit
    {
        [Header("References")]
        private PlayerVisual _playerVisual;

        [Range(0.1f, 2f)] [SerializeField] private float _actionDurationModifier;
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
            if (!_canMove || _isMoving || _hasUtility) return;

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
            this.SendEvent(new OnPlayerSteppedEvent());
            ExecuteMoveSequence(target);
            _tempMoveDirection = Direction.None;
        }

        private void ExecuteMoveSequence(Node targetNode)
        {
            _isMoving = true;
            _canMove = false;

            UpdateNodeData(targetNode);
            float duration = TurnManager.Instance.GlobalActionDuration * _actionDurationModifier;

            Sequence seq = DOTween.Sequence();

            seq.Append(_playerVisual.MoveTo(targetNode.WorldPos, duration));

			Sequence postMoveSeq = TryAttack(_currentNode);
            if (postMoveSeq != null)
            {
                seq.Append(postMoveSeq);
            }

            seq.OnComplete(() =>
            {
                _isMoving = false;
                _currentNode.TriggerEnter(this); 
                FinishAction(ShouldEndTurn());
            });
        }


        private Sequence TryAttack(Node targetNode)
        {
            if (targetNode.HasUnitsOfType<EnemyController>() && !targetNode.IsHideable())
            {
                var enemies = targetNode.GetUnitsByType<EnemyController>().ToList();

                return Attack(enemies);
            }


            return null;
        }

        private Sequence Attack(List<EnemyController> enemies)
        {
            Sequence attackSeq = DOTween.Sequence();

            foreach (var enemy in enemies)
            {
                Sequence deathSeq = enemy.Terminate();

                if (deathSeq != null)
                {
                    attackSeq.Insert(0, deathSeq);
                }
            }

            return attackSeq;
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

        public override Sequence Terminate()
        {
            Sequence deathSequence = DOTween.Sequence();
            deathSequence.Append(_playerVisual.FlyAnim());
            deathSequence.OnComplete(() => { this.SendEvent(new OnPlayerDeadEvent()); });
            return deathSequence;
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
            _currentUtility.Throw(targetNode, () => FinishAction(endsTurn));
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
                EditorUtility.SetDirty(_currentNode);
            }

            _currentNode = newNode;
            _currentNode.AddUnit(this);

            // Visual Snap
            transform.position = _currentNode.transform.position;

            EditorUtility.SetDirty(this);
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