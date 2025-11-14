using System;
using System.Collections;
using UnityEngine;
using Core.Events;
using Core.TurnSystem;
using Grid;


namespace Characters.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Private Fields

        [Header("Movement")] [SerializeField] private Node _currentNode;

        [Range(0f, 10f)] [SerializeField] protected float _actionDurationModifier = 0f;
        private float _actionDuration;


        private bool _isMoving = false;

        #endregion

        #region Event Handlers


        #endregion

        void OnEnable()
        {
            this.Subscribe<OnTurnChangedEvent>(HandleTurnChanged);
        }

        void OnDisable()
        {
            if (NewEventDispatcher.Instance != null)
            {
                this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChanged);
            }
        }

        void Start()
        {
            _actionDuration = TurnManager.Instance.ActionDuration;
        }

        private void Update()
        {
        }

        private void HandleTurnChanged(OnTurnChangedEvent eventData)
        {
            TurnType turnType = eventData.NewTurn;
            if (turnType != TurnType.PlayerAction) return;
        }


        public void TryMoveTo(Node target)
        {
            if (_isMoving || target == null) return;
            if (!_currentNode.ConnectedNodes.Contains(target)) return;

            StartCoroutine(MoveToNode(target));
        }

        private IEnumerator MoveToNode(Node target)
        {
            TurnManager.Instance.StartActionPhase();

            _isMoving = true;

            _currentNode.UnassignPlayer(this);
            target.AssignPlayer(this);

            Vector3 start = transform.position;
            Vector3 end = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);


            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * (_actionDuration + _actionDurationModifier);
                transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            _currentNode = target;
            _isMoving = false;

            TurnManager.Instance.EndTurn();
        }
    }
}