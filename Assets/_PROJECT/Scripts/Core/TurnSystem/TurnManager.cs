public class TurnManager : Singleton<TurnManager>
{
    private TurnType _currentTurn;
    private bool _actionPhaseActive = false;
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
        if (_actionPhaseActive) return;
        _actionPhaseActive = true;

        SetTurn(nextActionTurn);
    }

    private void EndActionPhase(TurnType nextPlainningTurn)
    {
        if (!_actionPhaseActive) return;
        _actionPhaseActive = false;

        SetTurn(nextPlainningTurn);
    }

    private void SetTurn(TurnType next)
    {
        if (_lock) return;
        if (!IsValidTransition(_currentTurn, next)) return;

        _currentTurn = next;
        this.SendEvent(new OnTurnChangedEvent { NewTurn = _currentTurn });
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
                return to == TurnType.PlayerPlanning
                       || to == TurnType.EnemyPlanning;

            case TurnType.EnemyPlanning:
                return to == TurnType.EnemyAction;

            case TurnType.EnemyAction:
                return to == TurnType.PlayerPlanning;

            default:
                return false;
        }
    }
}