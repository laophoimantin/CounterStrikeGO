using System;
using System.Collections;
using System.Collections.Generic;
using Characters.Enemy.EnemyBehaviors;
using Characters.Enemy.EnemyActions;
using Characters.Player;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private bool _isDead = false;

        [Header("Component References")] [SerializeField]
        private Transform _enemyModel;

        [SerializeField] private BaseEnemyBehavior _currentBehavior;

        [Header("Enemy State")] [SerializeField]
        private Direction _facingDirection = Direction.None;

        private readonly Direction[] _dirs =
        {
            Direction.North, Direction.East, Direction.South, Direction.West
        };

        [SerializeField] private Node _currentNode;

        public Direction CurrentFacingDirection => _facingDirection;
        public Node CurrentNode => _currentNode;


        void OnEnable()
        {
            EnemyManager.Instance.RegisterEnemy(this);
        }

        void OnDisable()
        {
        }

        void Start()
        {
            if (_currentNode == null)
                Debug.LogWarning($"{gameObject.name} has no node assigned!!!");
            if (_currentBehavior == null)
                Debug.LogWarning($"{gameObject.name} has no behavior assigned!!!");
            if (_facingDirection == Direction.None)
                Debug.LogWarning($"{gameObject.name} has no facing direction assigned!!!");

            if (_currentNode != null)
                _currentNode.AddEnemy(this);
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
                OnActionFinished();
                yield break;
            }

            foreach (BaseEnemyAction action in plan)
            {
                yield return action.Execute(this);
            }

            OnActionFinished();
        }


        // --- AI Action & Utility Methods ---
        // Actions ==============================================================================================

        #region Actions Methods

        /// <summary>
        /// Scans forward in the enemy's facing direction to detect the player.
        /// This scan is blocked by obstacles.
        /// </summary>
        /// <param name="range">The maximum number of nodes to check.</param>
        /// <returns>True if the player is spotted within range, false otherwise.</returns>
        public bool ScanForPlayerInFront(int range)
        {
            Node nodeToScan = GetNodeInFront();

            while (nodeToScan != null && range > 0)
            {
                if (nodeToScan.HasPlayer)
                {
                    Debug.Log("Guard at " + _currentNode.name + " SPOTTED PLAYER at " + nodeToScan.name);
                    return true;
                }

                if (nodeToScan.IsObstacle)
                {
                    return false;
                }

                range--;
                nodeToScan = GetNodeInDirection(nodeToScan, _facingDirection);
            }

            return false;
        }


        /// <summary>
        /// Updates the enemy's current node. 
        /// </summary>
        /// <param name="newNode">The new node the enemy is now occupying.</param>
        public void UpdateNodeData(Node newNode)
        {
            if (newNode == null) return;

            if (_currentNode != null)
            {
                _currentNode.UnAssignEnemy(this);
            }

            _currentNode = newNode;
            _currentNode.AddEnemy(this);
        }


        public IEnumerator Move(Node targetNode, float duration)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = targetNode.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            transform.position = endPos;
            UpdateNodeData(targetNode);
        }

        public IEnumerator Rotate(Quaternion targetRotation, float duration)
        {
            Quaternion startRot = _enemyModel.rotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _enemyModel.rotation = Quaternion.Slerp(startRot, targetRotation, t);
                yield return null;
            }

            _enemyModel.rotation = targetRotation;
        }


        private void OnActionFinished()
        {
            Attack(_currentNode);
            EnemyManager.Instance.OnEnemyFinished(this);
        }

        public void Attack(Node targetNode)
        {
            if (targetNode.HasPlayer)
            {
                targetNode.Player.Die();
            }
        }

        public void Die(Action onDeathComplete = null)
        {
            if (_isDead) return;
            _isDead = true;

            StartCoroutine(DeathRoutine(onDeathComplete));
        }

        private IEnumerator DeathRoutine(Action onDeathComplete)
        {
            float duration = 1f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _enemyModel.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }
            transform.localScale = Vector3.zero;
            
            _currentNode.UnAssignEnemy(this);
            EnemyManager.Instance.UnregisterEnemy(this);
            onDeathComplete?.Invoke();
        }
        
        
        #endregion

        #region Utility Methods

        /// <summary>
        /// Returns the node of the given node based on a direction.
        /// </summary>
        /// <param name="node">The origin node.</param>
        /// <param name="dir">The direction to look in.</param>
        /// <returns>The adjacent node, or null if there is none.</returns>
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

        /// <summary>
        /// Return the direction of the adjacent node based on the given node
        /// </summary>
        /// <param name="from">The origin node.</param>
        /// <param name="to">The adjacent node to check.</param>
        /// <returns>The direction to the target node, or the current facing direction if not adjacent.</returns>
        public Direction GetDirectionFromTargetNode(Node from, Node to)
        {
            if (to == null) return _facingDirection;

            if (from.NorthNode == to) return Direction.North;
            if (from.SouthNode == to) return Direction.South;
            if (from.EastNode == to) return Direction.East;
            if (from.WestNode == to) return Direction.West;

            return _facingDirection;
        }


        /// <summary>
        /// Set the facing direction of the model.
        /// </summary>
        /// <param name="newDirection">The new direction to face.</param>
        /// <returns>A Quaternion representing the new world rotation for the model.</returns>
        public Quaternion SetAndGetWorldRotation(Direction newDirection)
        {
            _facingDirection = newDirection;
            return GetWorldRotation(newDirection);
        }

        /// <summary>
        /// Return the world rotation base on the direction.
        /// </summary>
        /// <returns>A Quaternion representing the world rotation.</returns>
        public Quaternion GetWorldRotation(Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return Quaternion.Euler(0, 0, 0);
                case Direction.South: return Quaternion.Euler(0, 180, 0);
                case Direction.East: return Quaternion.Euler(0, 90, 0);
                case Direction.West: return Quaternion.Euler(0, -90, 0);
                default: return _enemyModel.rotation;
            }
        }

        /// <summary>
        /// Get Node In Front lul
        /// </summary>
        /// <returns></returns>
        public Node GetNodeInFront()
        {
            return GetNodeInDirection(_currentNode, _facingDirection);
        }


        /// <summary>
        /// Return the direction of the adjacent node based on the current node
        /// </summary>
        /// <returns></returns>
        public Direction GetDirectionFromCurrentNode(Node targetNode)
        {
            if (targetNode == null) return _facingDirection;
            return GetDirectionFromTargetNode(_currentNode, targetNode);
        }

        /// <summary>
        /// Gets a new rotation based on a number of clockwise "steps" from the current facing direction.
        /// </summary>
        /// <param name="step">The number of 90-degree clockwise steps (1 for clockwise, -1 for counter-clockwise, 2 for 180-degree turn).</param>
        /// <returns>The new rotation.</returns>
        public Quaternion GetRotationByStep(int step)
        {
            int index = Array.IndexOf(_dirs, _facingDirection);
            int newIndex = (index + step + _dirs.Length) % _dirs.Length;
            return SetAndGetWorldRotation(_dirs[newIndex]);
        }


        /// <summary>
        /// Calculates the rotation needed to face an adjacent target node.
        /// </summary>
        /// <param name="targetNode">The node to face towards.</param>
        /// <returns>The new rotation.</returns>
        public Quaternion GetRotationTowardsNode(Node targetNode)
        {
            Direction dirToNode = GetDirectionFromCurrentNode(targetNode);
            return SetAndGetWorldRotation(dirToNode);
        }

        /// <summary>Gets the rotation for a 180-degree turn.</summary>
        public Quaternion GetRotationTurnAround()
        {
            return GetRotationByStep(+2);
        }

        /// <summary>Gets the rotation for a 90-degree clockwise turn.</summary>
        public Quaternion GetRotationClockwise()
        {
            return GetRotationByStep(+1);
        }

        /// <summary>Gets the rotation for a 90-degree counter-clockwise turn.</summary>
        public Quaternion GetRotationCounterClockwise()
        {
            return GetRotationByStep(-1);
        }

        #endregion


        // Editor ====================================================================================

        #region Editor Methods

        public void SetDirection(Direction dir)
        {
            _enemyModel.rotation = SetAndGetWorldRotation(dir);
        }

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
                {
                    Debug.LogWarning("No NodeManager instance found!", this);
                    newNode = null;
                }
                else
                    newNode = NodeManager.Instance.GetNodeFromWorldPosition(_enemyModel.position);
            }

            if (_currentNode != null)
                _currentNode.UnAssignEnemy(this);


            if (newNode == null)
            {
                Debug.LogWarning(name + " has no valid node!" + warningLog, this);
                return;
            }

            _currentNode = newNode;

            SnapPosition(_currentNode.transform.position);
            _currentNode.AddEnemy(this);
        }

        public void UnAssignNode()
        {
            _currentNode.UnAssignEnemy(this);
            _currentNode = null;
        }


        private void SnapPosition(Vector3 pos)
        {
            _enemyModel.position = new Vector3(pos.x, transform.position.y, pos.z);
        }

        #endregion
    }
}