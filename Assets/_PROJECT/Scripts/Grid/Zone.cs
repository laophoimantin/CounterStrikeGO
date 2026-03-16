using Core.Events;
using Core.TurnSystem;
using DG.Tweening;
using Pawn;
using UnityEngine;

namespace Grid
{
    public abstract class Zone : MonoBehaviour
    {
		private int _duration;
        protected Node _currentNode;
        private bool _isExpired = false;
        [SerializeField] private ZoneVisual _zoneVisual;
     

        public virtual Tween Initialize(Node hostNode, int duration)
        {
            _currentNode = hostNode;
            _duration = duration;

            this.Subscribe<OnTurnChangedEvent>(HandleTurnChange);

            _currentNode.AddZone(this);

            Sequence seq = DOTween.Sequence();

            seq.Append(_zoneVisual.ActivateZoneModel());

            Tween created = OnZoneCreated();
            if (created != null)
                seq.Append(created);

            return seq;
        }

        void OnDisable()
        {
            this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChange);
        }

        private void HandleTurnChange(OnTurnChangedEvent eventData)
        {
            if (_isExpired) return;
            
            if (eventData.NewTurn == TurnType.PlayerPlanning)
            {
                _duration--;
                if (_duration <= 0)
                {
                    Expire();
                }
            }
        }

        public void Expire()
        {
            _isExpired = true;
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                _currentNode.RemoveZone();
            });
            seq.Append(_zoneVisual.FlyUp());
            seq.AppendCallback(() =>
            {
                Destroy(gameObject);
            });
        }

        public virtual bool IsWalkable() => true;

        public virtual bool IsHideable() => false;
        protected abstract Tween OnZoneCreated();
    }
}