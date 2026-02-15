using System;
using System.Collections.Generic;
using Core.Events;
using Core.TurnSystem;
using Pawn;
using UnityEngine;

namespace Grid
{
    public abstract class NodeZone : MonoBehaviour
    {
        protected Node _hostNode;
        protected int _duration;

        public virtual void Initialize(Node hostNode, int duration, Action onComplete)
        {
            this.Subscribe<OnTurnChangedEvent>(HandleTurnChange);
            _hostNode = hostNode;
            _duration = duration;
            
            _hostNode.AddZone(this);
            if (hostNode.HasUnit())
            {
                OnUnitEnter(hostNode.GetAllUnits(), onComplete);
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        void OnDisable()
        {
            this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChange);
        }
        
        private void HandleTurnChange(OnTurnChangedEvent eventData)
        {
            if (eventData.NewTurn == TurnType.PlayerPlanning)
            {
                _duration--;
                if (_duration <= 0)
                {
                    OnDurationExpired();
                }
            }
        }

        protected virtual void OnDurationExpired()
        {
            _hostNode.RemoveZone(); 
            Destroy(gameObject);
        }

        public void ForceDestroy()
        {
            Destroy(gameObject);
        }

        public abstract void OnUnitEnter(List<GridUnit> units, Action onComplete);

        public virtual bool IsWalkable() => true;

        public virtual bool IsObscuring() => false;
    }
}