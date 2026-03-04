using Pawn;
using Core;
using Grid;
using UnityEngine;

namespace Grid
{
    [CreateAssetMenu(fileName = "ExitFeature", menuName = "Grid/Node Feature/Exit")]
    public class ExitFeature : BaseNodeFeature
    {
        public override void OnEnter(GridUnit unit)
        {
            if (unit is PlayerController)
            {
                GameManager.Instance.EvaluateWin();
            }
        }
    }
}