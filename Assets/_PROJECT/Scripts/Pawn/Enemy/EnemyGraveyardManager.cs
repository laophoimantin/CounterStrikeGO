using DG.Tweening;
using UnityEngine;

public class EnemyGraveyardManager : Singleton<EnemyGraveyardManager>
{
    [Header("Graveyard Settings")]
    [Tooltip("The starting point where the first dead body goes")]
    [SerializeField] private Transform _graveyardStartPoint;

    [Tooltip("How much space to put between each dead pawn")]
    [SerializeField] private float _spacing = 1.5f;

    [Tooltip("How many dead pawns fit in one row before starting a new row?")]
    [SerializeField] private int _maxPerCol = 5;

    private int _deadCount = 0;

    private Vector3 GetNextSlotPosition()
    {
        // Calculate Row and Column based on how many have died
        int row = _deadCount % _maxPerCol;
        int col = _deadCount / _maxPerCol;

        // Calculate the exact world position for this slot
        Vector3 offset = new Vector3(col * _spacing, 0, row * _spacing);
        Vector3 finalPos = _graveyardStartPoint.position + offset;

        _deadCount++; // Increment for the next guy

        return finalPos;
    }

    public void CollectCorpse(EnemyController deadEnemy)
    {
        Vector3 finalRestingPlace = GetNextSlotPosition();

        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            deadEnemy.transform.position = finalRestingPlace;
            deadEnemy.EnemyVisual.SetPosition(finalRestingPlace);
        });

        seq.Append(deadEnemy.EnemyVisual.DropDown());
        seq.Append(deadEnemy.EnemyVisual.Bounce());
    }
}