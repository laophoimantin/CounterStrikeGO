using System.Collections;

namespace Grid
{
    public class SmokeZone : NodeZone
    {
        public override IEnumerator OnUnitEnter()
        {
            yield break;
        }

        public override bool IsObscuring() => true;
    }
}