using System;
using System.Collections;
using Characters.Enemy.EnemyBehaviors;
using Core.TurnSystem;
using Core.Events;
using Grid;
using Interfaces;
using EventType = Core.Events.EventType;
using UnityEngine;

namespace Characters.Enemy
{
    public enum Direction
    {
        North,
        South,
        East,
        West,
        None
    }

    public class EnemyController : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private Transform _enemyModel;
        [SerializeField] private BaseEnemyBehavior _currentBehavior;
        [SerializeField] private Direction _facingDirection = Direction.None;
        [SerializeField] private Node _currentNode;
        private Node _nodeToScan;

        #endregion

        #region Public Fields

        public BaseEnemyBehavior CurrentBehavior => _currentBehavior;
        public Direction CurrentFacingDirection => _facingDirection;
        public Node CurrentNode => _currentNode;

        #endregion

        void OnEnable()
        {
            EnemyTurnCoordinator.Instance.RegisterEnemy(this);
        }

        void OnDisable()
        {
            if (EnemyTurnCoordinator.Instance != null)
                EnemyTurnCoordinator.Instance.UnregisterEnemy(this);
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
            {
                _currentNode.AssignEnemy(this);
            }
        }

        public void StartAction()
        {
            StartCoroutine(ExecuteBehavior());
        }

        private IEnumerator ExecuteBehavior()
        {
            if (_currentBehavior != null)
                yield return _currentBehavior.Execute(this);
            EnemyTurnCoordinator.Instance.OnEnemyFinished(this);
        }

        public bool CheckForPlayer(int range)
        {
            Node nodeToScan = GetNodeInDirection(_currentNode, _facingDirection);

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

        public void UpdateNodeData(Node newNode)
        {
            if (newNode == null) return;

            if (_currentNode != null)
            {
                _currentNode.UnAssignEnemy(this);
            }

            _currentNode = newNode;
            _currentNode.AssignEnemy(this);
        }

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

        
        public Direction GetDirectionOfNode(Node targetNode)
        {
            if (targetNode == null) return _facingDirection; 

            if (_currentNode.NorthNode == targetNode) return Direction.North;
            if (_currentNode.SouthNode == targetNode) return Direction.South;
            if (_currentNode.EastNode == targetNode) return Direction.East;
            if (_currentNode.WestNode == targetNode) return Direction.West;

            return _facingDirection; 
        }

        public Quaternion RotationDirection(Direction newDirection)
        {
            _facingDirection = newDirection;

            switch (_facingDirection)
            {
                case Direction.North: return Quaternion.Euler(0, 0, 0);
                case Direction.South: return Quaternion.Euler(0, 180, 0);
                case Direction.East:  return Quaternion.Euler(0, 90, 0);
                case Direction.West:  return Quaternion.Euler(0, -90, 0);
                default: return _enemyModel.rotation;
            }
        }
        
        public Quaternion GetRotationByStep(int step)
        {
            // step = +1 for clockwise, -1 for counterclockwise
            Direction[] dirs = { Direction.North, Direction.East, Direction.South, Direction.West };
            int index = Array.IndexOf(dirs, _facingDirection);
            int newIndex = (index + step + dirs.Length) % dirs.Length;
            return RotationDirection(dirs[newIndex]); 
        }



        public Quaternion GetRotationTowardsNode(Node targetNode)    
        {
            Direction dirToNode = GetDirectionOfNode(targetNode);
            return RotationDirection(dirToNode);
        }
        
        
        public Quaternion GetRotationTurnAround()
        {
            return GetRotationByStep(+2);
        }

        public Quaternion GetRotationClockwise()
        {
            return GetRotationByStep(+1);
        }
        
        public Quaternion GetRotationCounterClockwise()
        {
            return GetRotationByStep(-1);
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
        
        
        
        public void Attack()
        {
            if (_currentNode.HasPlayer())
            {
                //Destroy(_currentNode.Cu)
            }
        }
        
        public void Die()
        {
            _currentNode.UnAssignEnemy(this);
            Destroy(gameObject);    
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
    }
}