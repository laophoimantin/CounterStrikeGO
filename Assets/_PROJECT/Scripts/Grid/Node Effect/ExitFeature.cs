using Pawn;
using Core;
using Grid;
using UnityEngine;

namespace Grid
{
    [CreateAssetMenu(fileName = "ExitFeature", menuName = "Grid/Node Feature/Exit")]
    public class ExitFeature : BaseNodeFeature
    {
        public override void OnEnter(PawnUnit pawnUnit)
        {
            if (pawnUnit is PlayerController)
            {
                GameManager.Instance.EvaluateWin();
            }
        }
    }
}