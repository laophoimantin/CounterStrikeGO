using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manage all the enemies, execute each enemy when it is enemy turn 
/// </summary>
public class EnemyManager : Singleton<EnemyManager>
{
    private readonly List<EnemyController> _activeEnemiesList = new();
    private bool _hasKilledEnemy = false;
    public bool AreAllEnemiesDefeated() => _activeEnemiesList.Count <= 0;
    public bool HasKilledEnemy() => _hasKilledEnemy;

    private int _pendingCount;
    private int _finishedCount = 0;

    void OnEnable()
    {
        this.Subscribe<OnTurnChangedEvent>(HandleTurnChange);
    }

    void OnDisable()
    {
        this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChange);
    }

    // Enemy registration =======================================================

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!_activeEnemiesList.Contains(enemy))
        {
            _activeEnemiesList.Add(enemy);
            enemy.OnDeath += UnregisterEnemy;
        }
    }

    public void UnregisterEnemy(EnemyController enemy)
    {
        enemy.OnDeath -= UnregisterEnemy;
        
        _activeEnemiesList.Remove(enemy);
        if (!_hasKilledEnemy)
            _hasKilledEnemy = true;

        if (EnemyGraveyardManager.Instance != null)
        {
            EnemyGraveyardManager.Instance.CollectCorpse(enemy);
        }
        
        if (_pendingCount > 0)
        {
            _pendingCount--;
            // if (_finishedCount >= _pendingCount)
            //     StartCoroutine(EndEnemyActionPhase());
        }
    }

    // Turn System =========================================================================
    private void HandleTurnChange(OnTurnChangedEvent eventData)
    {
        if (eventData.NewTurn != TurnType.EnemyPlanning)
            return;
        List<EnemyController> snapshot = _activeEnemiesList
            .Where(enemy => enemy != null)
            .ToList();
        StartCoroutine(BeginEnemyAction(snapshot));
    }

    private IEnumerator BeginEnemyAction(List<EnemyController> snapshot)
    {
        yield return new WaitForSeconds(0.1f);
        this.SendEvent(new OnEnemyActionStartedEvent());

        if (snapshot.Count == 0)
        {
            yield return StartCoroutine(EndEnemyActionPhase());
            yield break;
        }

        var enemyGroups = snapshot
            .GroupBy(e => e.ExecutionPriority)
            .OrderBy(group => group.Key);

        foreach (var group in enemyGroups)
        {
            _pendingCount = group.Count();
            _finishedCount = 0;
            
            foreach (var enemy in group)
            {
                enemy.StartAction(); 
            }

            yield return new WaitUntil(() => _finishedCount >= _pendingCount || _pendingCount == 0);
        }

        yield return StartCoroutine(EndEnemyActionPhase());
    }

    public void OnEnemyFinished(EnemyController enemy)
    {
        _finishedCount++;
    }

    private IEnumerator EndEnemyActionPhase()
    {
        yield return new WaitForSeconds(0.1f);
        this.SendEvent(new OnEnemyActionFinishedEvent());
    }
}