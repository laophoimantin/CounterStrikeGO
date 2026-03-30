using DG.Tweening;

public class SmokeZone : BaseZone
{
    protected override Tween OnZoneCreated()
    {
        return null;
    }

    public override bool IsHideable() => true;
}