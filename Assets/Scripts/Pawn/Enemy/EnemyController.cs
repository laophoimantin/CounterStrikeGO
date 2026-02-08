using System;
using System.Collections;
using System.Collections.Generic;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn
{
    public class EnemyController : GridUnit
    {

        public override TeamSide Team => TeamSide.Enemy;

        private bool _isDead = false;

        [Header("Component References")] 
        [SerializeField] private Transform _enemyModel;

        [SerializeField] private BaseEnemyBehavior _currentBehavior;

        [Header("Enemy State")] 
        [SerializeField] private Direction _facingDirection = Direction.None;

        private readonly Direction[] _dirs =
        {
            Direction.North, Direction.East, Direction.South, Direction.West
        };

        [SerializeField] private Node _currentNode;

        public Direction CurrentFacingDirection => _facingDirection;
        public Node CurrentNode => _currentNode;

        public Action<EnemyController> OnDestroyed;

        void OnEnable()
        {
            EnemyManager.Instance.RegisterEnemy(this);
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
                _currentNode.AddUnit(this);
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
                if (nodeToScan.HasPlayer())
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

        public IEnumerator Move(Node targetNode, float duration)
        {
            // Logic
            UpdateNodeData(targetNode);
            
            // Visual
            // Vector3 endPos = targetNode.transform.position;
            Vector3 startPos = transform.position;
            Vector3 endPos = targetNode.WorldPos;
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            transform.position = endPos;
        }

        public IEnumerator Rotate(Direction newDirection, float duration)
        {
            // Logic
            SetFacingDirection(newDirection);
            
            // Visual
            Quaternion startRot = _enemyModel.rotation;
            Quaternion targetRotation = GetRotationForDirection(newDirection);
            
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

        private void OnActionFinished()
        {
            Attack(_currentNode);
            EnemyManager.Instance.OnEnemyFinished(this);
        }

        private void Attack(Node targetNode)
        {
            if (targetNode.HasPlayer())
            {
                GridUnit player = targetNode.GetPlayer();
                player.Die(() => {
                    Debug.Log("End!");
                });
            }
        }

        public override void Die(Action onDeathComplete = null)
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
            
            _currentNode.RemoveUnit(this);
            OnDestroyed?.Invoke(this);
            onDeathComplete?.Invoke();
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
                default: return _enemyModel.rotation;
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
            _enemyModel.rotation = GetRotationForDirection(dir);
            SetFacingDirection(dir);
        }

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
                newNode = manager.GetNodeFromWorldPosition(_enemyModel.position);
            }
            
            if (newNode == null)
            {
                Debug.LogWarning(name + " has no valid node!" + warningLog, this);
                return;
            }
            
            if (_currentNode != null)
                _currentNode.RemoveUnit(this);

            _currentNode = newNode;
            _currentNode.AddUnit(this);
    
            SnapPosition(_currentNode.transform.position);
            
#if UNITY_EDITOR
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
            if (_enemyModel != null)
                _enemyModel.localPosition = Vector3.zero;
        }

        #endregion
    }
}