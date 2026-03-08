using System.Collections;
using DG.Tweening;

namespace Grid
{
    public class SmokeZone : Zone
    {
        protected override Tween OnZoneCreated()
        {
            return null;
        }

        public override bool IsHideable() => true;
    }
}