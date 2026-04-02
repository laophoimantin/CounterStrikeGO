using System.Collections.Generic;

public class SniperBehavior : BaseEnemyBehavior
{
    public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
    {
        var plan = new List<BaseEnemyAction>();



        return plan;
    }

    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {

    }
}