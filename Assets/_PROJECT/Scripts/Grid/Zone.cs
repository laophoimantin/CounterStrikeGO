using Core.Events;
using Core.TurnSystem;
using DG.Tweening;
using Pawn;

namespace Grid
{
    public abstract class Zone : GridOccupant
    {
        protected int _duration;
        private ZoneVisual _zoneVisual;
        void Awake()
        {
            _zoneVisual = (ZoneVisual)_visual;
        }
        

        public virtual Tween Initialize(Node hostNode, int duration)
        {
            _currentNode = hostNode;
            _duration = duration;

            this.Subscribe<OnTurnChangedEvent>(HandleTurnChange);

            _currentNode.AddUnit(this);
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
            Sequence seq = DOTween.Sequence();
            seq.Append(_zoneVisual.FlyAnim());
            
            seq.OnComplete(() =>
            {
                _currentNode.RemoveZone();
                _currentNode.RemoveUnit(this);
                Destroy(gameObject);
            });
        }

        public virtual bool IsWalkable() => true;

        public virtual bool IsHideable() => false;
        protected abstract Tween OnZoneCreated();
    }
}