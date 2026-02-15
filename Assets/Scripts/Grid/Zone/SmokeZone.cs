using System;
using System.Collections.Generic;
using Pawn;

namespace Grid
{
    public class SmokeZone : NodeZone
    {
        public override void OnUnitEnter(List<GridUnit> units, Action onComplete)
        {
            onComplete?.Invoke();
        }

        public override bool IsObscuring() => true;
    }
}