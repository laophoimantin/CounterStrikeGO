using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public virtual IEnumerator Initialize(Node hostNode, int duration)
        {
            this.Subscribe<OnTurnChangedEvent>(HandleTurnChange);
            _hostNode = hostNode;
            _duration = duration;
            
            _hostNode.AddZone(this);
            if (hostNode.HasUnit())
            {
                yield return OnUnitEnter();
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

        public abstract IEnumerator OnUnitEnter();

        public virtual bool IsWalkable() => true;

        public virtual bool IsObscuring() => false;
    }
}