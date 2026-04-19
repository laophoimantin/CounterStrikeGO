using DG.Tweening;

/// <summary>
/// Block the vision. Prevent any attacks from occurring on this node.
/// </summary>
public class SmokeZone : BaseZone
{
    protected override Tween OnZoneCreated()
    {
        return null;
    }

    public override bool IsHideable() => true;
}