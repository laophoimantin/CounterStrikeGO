using System.Collections.Generic;
using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    private TurnType _currentTurn = TurnType.None;
    private bool _lock = false;

    public int StepCount { get; private set; }

    void OnEnable()
    {
        this.Subscribe<OnGameEndedEvent>(Lock);

        // Player Turn Events
        this.Subscribe<OnPlayerActionStartedEvent>(HandlePlayerStarted);
        this.Subscribe<OnPlayerActionFinishedEvent>(HandlePlayerFinished);

        //Enemy Turn Events
        this.Subscribe<OnEnemyActionStartedEvent>(HandleEnemyStarted);
        this.Subscribe<OnEnemyActionFinishedEvent>(HandleEnemyFinished);
    }

    void OnDisable()
    {
        this.Unsubscribe<OnGameEndedEvent>(Lock);

        // Player Turn Events
        this.Unsubscribe<OnPlayerActionStartedEvent>(HandlePlayerStarted);
        this.Unsubscribe<OnPlayerActionFinishedEvent>(HandlePlayerFinished);

        //Enemy Turn Events
        this.Unsubscribe<OnEnemyActionStartedEvent>(HandleEnemyStarted);
        this.Unsubscribe<OnEnemyActionFinishedEvent>(HandleEnemyFinished);
    }

    void Start()
    {
        SetTurn(TurnType.PlayerPlanning);
    }

    private void Lock(OnGameEndedEvent eventData)
    {
        _lock = true;
    }

    private void StartActionPhase(TurnType nextActionTurn)
    {
        SetTurn(nextActionTurn);
    }

    private void EndActionPhase(TurnType nextPlanningTurn)
    {
        SetTurn(nextPlanningTurn);
    }

    // private void SetTurn(TurnType next)
    // {
    //     if (_lock) return;
    //
    //     if (!IsValidTransition(_currentTurn, next)) return;
    //
    //     _currentTurn = next;
    //     this.SendEvent(new OnTurnChangedEvent { NewTurn = _currentTurn });
    //
    // }
    private bool _isTransitioning = false;
    private Queue<TurnType> _pendingTurns = new Queue<TurnType>(); // Hàng đợi cho bọn nôn nóng
    private void SetTurn(TurnType next)
    {
        if (_lock) return;
        _pendingTurns.Enqueue(next);

        if (_isTransitioning) return; 

        _isTransitioning = true;

        while (_pendingTurns.Count > 0)
        {
            TurnType queuedNext = _pendingTurns.Dequeue();

            if (!IsValidTransition(_currentTurn, queuedNext)) 
            {
                continue;
            }

            _currentTurn = queuedNext;
        
            this.SendEvent(new OnTurnChangedEvent { NewTurn = _currentTurn });
        }

        _isTransitioning = false;
    }
    
    
    
    
    
    
    
    

    // Player Turn Events
    private void HandlePlayerStarted(OnPlayerActionStartedEvent eventData)
    {
        if (_currentTurn != TurnType.PlayerPlanning) return;
        StepCount++;
        StartActionPhase(TurnType.PlayerAction);
    }

    private void HandlePlayerFinished(OnPlayerActionFinishedEvent eventData)
    {
        if (_currentTurn != TurnType.PlayerAction) return;
        if (eventData.EndTurn)
        {
            EndActionPhase(TurnType.EnemyPlanning);
        }
        else
        {
            EndActionPhase(TurnType.PlayerPlanning);
        }
    }

    //Enemy Turn Events
    private void HandleEnemyStarted(OnEnemyActionStartedEvent eventData)
    {
        if (_currentTurn != TurnType.EnemyPlanning) return;
        StartActionPhase(TurnType.EnemyAction);
    }

    private void HandleEnemyFinished(OnEnemyActionFinishedEvent eventData)
    {
        if (_currentTurn != TurnType.EnemyAction) return;
        EndActionPhase(TurnType.PlayerPlanning);
    }

    private bool IsValidTransition(TurnType from, TurnType to)
    {
        switch (from)
        {
            case TurnType.PlayerPlanning:
                return to == TurnType.PlayerAction;

            case TurnType.PlayerAction:
                return to == TurnType.PlayerPlanning || to == TurnType.EnemyPlanning;

            case TurnType.EnemyPlanning:
                return to == TurnType.EnemyAction;

            case TurnType.EnemyAction:
                return to == TurnType.PlayerPlanning;
            
            case TurnType.None:
                return to == TurnType.PlayerPlanning;

            default:
                return false;
        }

        /*
        // *** Pattern Matching ***
        return (from, to) switch
        {
            (TurnType.PlayerPlanning, TurnType.PlayerAction) => true,
            (TurnType.PlayerAction, TurnType.PlayerPlanning) => true,
            (TurnType.PlayerAction, TurnType.EnemyPlanning) => true,

            (TurnType.EnemyPlanning, TurnType.EnemyAction) => true,
            (TurnType.EnemyAction, TurnType.PlayerPlanning) => true,

            (_, TurnType.PlayerPlanning) when from == default => true,

            _ => false
        };
        */
    }
}