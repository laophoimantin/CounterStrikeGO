using Core.TurnSystem;
using Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pawn
{
    public class EnemyController : GridUnit
    {

        private bool _isDead = false;

        [Header("References")]
        private EnemyVisual _enemyVisual;

        private BaseEnemyBehavior _currentBehavior;
        [SerializeField] private BaseEnemyBehavior _defaultBehavior;
        [SerializeField] private FollowingNoiseBehavior _noiseBehavior;
        [SerializeField] private FlashedBehavior _flashedBehavior;

        [Header("Enemy State")]
        [SerializeField] private Direction _facingDirection = Direction.None;

        private readonly Direction[] _dirs =
        {
            Direction.North, Direction.East, Direction.South, Direction.West
        };

        public Direction CurrentFacingDirection => _facingDirection;



        private List<Node> _astarPath = new();
        public List<Node> AstarPath => _astarPath;

        public Node StartNode => 0 < _astarPath.Count ? _astarPath[0] : null;
        public Node NextNode => 0 + 1 < _astarPath.Count ? _astarPath[1] : null;
        public Node UpcomingNode => 0 + 2 < _astarPath.Count ? _astarPath[2] : null;



        private int _flashTurnsRemaining;



        public Action<EnemyController> OnDestroyed;

        void OnEnable()
        {
            EnemyManager.Instance.RegisterEnemy(this);
        }

        void Start()
        {
            if (_currentNode == null)
                Debug.LogWarning($"{gameObject.name} has no node assigned!!!");
            if (_defaultBehavior == null || _noiseBehavior == null)
                Debug.LogWarning($"{gameObject.name} has no behavior assigned!!!");
            if (_facingDirection == Direction.None)
                Debug.LogWarning($"{gameObject.name} has no facing direction assigned!!!");

            if (_currentNode != null)
                _currentNode.AddUnit(this);

            _enemyVisual = _visual as EnemyVisual;
            if (_enemyVisual == null)
                Debug.Log("VISUALLLLLL!");

            _currentBehavior = _defaultBehavior;
        }


        public void StartAction()
        {
            StartCoroutine(ExecuteBehavior());
        }

        private IEnumerator ExecuteBehavior()
        {
            if (_isDead) yield break;

            List<BaseEnemyAction> plan = _currentBehavior.PlanActions(this);

            if (plan == null || plan.Count == 0)
            {
                FinishTurn();
                yield break;
            }

            foreach (BaseEnemyAction action in plan)
            {
                yield return action.Execute(this);

                if (TryAttack(_currentNode))
                {
                    FinishTurn();
                    yield break;
                }
            }

            FinishTurn();
        }

        private void FinishTurn()
        {
            AdvancePath();
            AdvanceFlashed();
            HasReachedNoiseDestination();
            HasEndFlashed();
            EnemyManager.Instance.OnEnemyFinished(this);
        }

        // --- AI Action & Utility Methods ---
        // Actions ==============================================================================================

        #region Actions Methods
        public bool ScanForPlayerInFront(int range)
        {
            Node nodeToScan = GetNodeInFront();

            while (nodeToScan != null && range > 0)
            {
                if (nodeToScan.HasUnitsOfType<PlayerController>())
                {
                    return true;
                }

                if (!nodeToScan.IsWalkable())
                {
                    return false;
                }

                range--;
                nodeToScan = GetNodeInDirection(nodeToScan, _facingDirection);
            }

            return false;
        }


        public IEnumerator Move(Node targetNode, float duration)
        {
            // Logic
            UpdateNodeData(targetNode);

            // Visual
            yield return _visual.MoveTo(targetNode.WorldPos, duration);
        }

        public IEnumerator Rotate(Direction newDirection, float duration)
        {
            // Logic
            SetFacingDirection(newDirection);

            // Visual
            Quaternion targetRot = GetRotationForDirection(newDirection);
            yield return _visual.RotateTo(targetRot, duration);
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


        private bool TryAttack(Node targetNode)
        {
            if (targetNode.HasUnitsOfType<PlayerController>() && !targetNode.IsHidden())
            {
                GridUnit player = targetNode.GetUnitByType<PlayerController>();
                player.Terminate();
                return true;
            }

            return false;
        }

        public override void Terminate(Action onDeathComplete)
        {
            if (_isDead) return;
            _isDead = true;

            StartCoroutine(_visual.DeadAnim(1f, () =>
            {
                _currentNode.RemoveUnit(this);
                OnDestroyed?.Invoke(this);
                onDeathComplete?.Invoke();
            }));
        }

        public void HearNoise(Node noiseOrigin, Action onReactionComplete)
        {
            _currentBehavior = _noiseBehavior;
            _astarPath = AstarPathfinder.FindPath(_currentNode, noiseOrigin);
            StartCoroutine(ReactToNoise(_astarPath, onReactionComplete));
        }

        private IEnumerator ReactToNoise(List<Node> path, Action onReactionComplete)
        {
            if (path == null || path.Count < 2)
            {
                yield break;
            }

            Direction targetDirection = GetDirectionFromCurrentNode(NextNode);

            yield return Rotate(targetDirection, 0.1f);
            onReactionComplete?.Invoke();
        }

        public void AdvancePath()
        {
            if (_astarPath != null && _astarPath.Count > 0)
                _astarPath.RemoveAt(0);
        }

        public bool HasReachedNoiseDestination()
        {
            if (_astarPath == null || _astarPath.Count <= 1)
            {
                _currentBehavior = _defaultBehavior;
                return true;
            }
            return false;
        }


        public void GetFlashed(int duration, Action onReactionComplete)
        {
            _currentBehavior = _flashedBehavior;
            _flashTurnsRemaining = Mathf.Max(_flashTurnsRemaining, duration);
            onReactionComplete?.Invoke();
        }
        public void AdvanceFlashed()
        {
            if (_flashTurnsRemaining > 0)
                _flashTurnsRemaining--;
        }

        public bool HasEndFlashed()
        {
            if (_flashTurnsRemaining <= 0)
            {
                _currentBehavior = _defaultBehavior;
                return true;
            }
            return false;
        }


        #endregion





        #region Utility Methods

        // 1. STATE MANAGEMENT (Setters & Core Getters)
        // ==============================================================================================

        /// <summary>
        /// STATE CHANGE: Updates the internal facing direction state
        /// Call this only after a rotation action is complete
        /// </summary>
        public void SetFacingDirection(Direction newDirection)
        {
            _facingDirection = newDirection;
        }

        // 2. NODE QUERIES (Finding Neighbors)
        // ==============================================================================================

        /// <summary>
        /// Returns the node directly in front of the enemy based on current facing direction
        /// </summary>
        public Node GetNodeInFront()
        {
            return GetNodeInDirection(_currentNode, _facingDirection);
        }

        /// <summary>
        /// Returns the neighbor of a specific node in a specific direction
        /// </summary>
        public Node GetNodeInDirection(Node node, Direction dir)
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

        // 3. DIRECTION LOGIC (Calculating Directions)
        // ==============================================================================================

        /// <summary>
        /// Calculates the direction from the current node to a target node
        /// </summary>
        public Direction GetDirectionFromCurrentNode(Node targetNode)
        {
            if (targetNode == null) return _facingDirection;
            return GetDirectionFromTargetNode(_currentNode, targetNode);
        }

        /// <summary>
        /// Calculates the direction between two adjacent nodes
        /// </summary>
        public Direction GetDirectionFromTargetNode(Node from, Node to)
        {
            if (to == null) return _facingDirection;

            if (from.NorthNode == to) return Direction.North;
            if (from.SouthNode == to) return Direction.South;
            if (from.EastNode == to) return Direction.East;
            if (from.WestNode == to) return Direction.West;

            return _facingDirection; // Default if not adjacent
        }

        /// <summary>
        /// Returns the Direction enum for a step (+1 for Clockwise)
        /// Useful for passing data to RotateActions
        /// </summary>
        private Direction GetDirectionByStep(int step)
        {
            int index = Array.IndexOf(_dirs, _facingDirection);
            int newIndex = (index + step + _dirs.Length) % _dirs.Length;
            return _dirs[newIndex];
        }

        // 4. ROTATION LOGIC (Math & Quaternions)
        // ==============================================================================================

        /// <summary>
        /// Calculates the rotation needed to face a specific direction
        /// </summary>
        private Quaternion GetRotationForDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return Quaternion.Euler(0, 0, 0);
                case Direction.South: return Quaternion.Euler(0, 180, 0);
                case Direction.East: return Quaternion.Euler(0, 90, 0);
                case Direction.West: return Quaternion.Euler(0, -90, 0);
                default: return _visual.GetRotation();
            }
        }

        /// <summary>
        /// Calculates the rotation needed to face an adjacent target node
        /// </summary>
        public Quaternion GetRotationTowardsNode(Node targetNode)
        {
            Direction dirToNode = GetDirectionFromCurrentNode(targetNode);
            return GetRotationForDirection(dirToNode);
        }

        /// <summary>
        /// Helper to get a rotation based on steps (90 degree increments)
        /// </summary>
        private Quaternion GetRotationByStep(int step)
        {
            Direction futureDir = GetDirectionByStep(step);
            return GetRotationForDirection(futureDir);
        }


        // Rotation Presets ==============================================================================================

        public Quaternion GetRotationTurnAround() => GetRotationByStep(+2);
        public Quaternion GetRotationClockwise() => GetRotationByStep(+1);
        public Quaternion GetRotationCounterClockwise() => GetRotationByStep(-1);

        // Direction Presets ==============================================================================================

        public Direction GetDirectionTurnAround() => GetDirectionByStep(+2);
        public Direction GetDirectionClockwise() => GetDirectionByStep(+1);
        public Direction GetDirectionCounterClockwise() => GetDirectionByStep(-1);

        #endregion


        // Editor ====================================================================================

        #region Editor Methods

        public void SetDirection(Direction dir)
        {
            _visual.SetRotation(GetRotationForDirection(dir));
            SetFacingDirection(dir);
        }
#if UNITY_EDITOR
        public void SetOrMoveNode(Direction? dir = null)
        {
            NodeManager manager = NodeManager.Instance;
            if (manager == null)
                manager = FindObjectOfType<NodeManager>();

            if (manager == null)
            {
                Debug.LogError("No NodeManager found in scene. Cannot move.", this);
                return;
            }

            Node newNode;
            string warningLog = dir.HasValue ? " No valid node to move to!" : " Invalid spot, couldn't find a node!";

            if (dir.HasValue)
            {
                newNode = GetNodeInDirection(_currentNode, dir.Value);
            }
            else
            {
                newNode = manager.GetNodeFromWorldPosition(transform.position);
            }

            if (newNode == null)
            {
                Debug.LogWarning(name + " has no valid node!" + warningLog, this);
                return;
            }

            if (_currentNode != null)
            {
                _currentNode.RemoveUnit(this);
                UnityEditor.EditorUtility.SetDirty(_currentNode);
            }
            _currentNode = newNode;
            _currentNode.AddUnit(this);

            SnapPosition(_currentNode.transform.position);


            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void UnAssignNode()
        {
            if (_currentNode == null) return;
            _currentNode.RemoveUnit(this);
            _currentNode = null;
        }


        private void SnapPosition(Vector3 targetPos)
        {
            transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            _visual.SetPosition(targetPos);
        }

        #endregion
    }
}