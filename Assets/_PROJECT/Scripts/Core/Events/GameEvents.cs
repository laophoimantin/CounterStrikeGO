using UnityEngine;
using UnityEngine.UI;

public struct OnTurnChangedEvent
{
    public TurnType NewTurn;
}

public struct OnPlayerDeadEvent
{
}

public struct OnPlayerActionStartedEvent
{
}

public struct OnPlayerActionFinishedEvent
{
    public bool EndTurn;

    public OnPlayerActionFinishedEvent(bool endTurn)
    {
        EndTurn = endTurn;
    }
}


public struct OnEnemyActionStartedEvent
{
}

public struct OnEnemyActionFinishedEvent
{
}

public struct OnEnemyKilledEvent
{
    private int RemainingEnemyCount;

    public OnEnemyKilledEvent(int remain)
    {
        RemainingEnemyCount = remain;
    }
}


public struct OnGameEndedEvent
{
}


// Test
public struct PlayerScoredEventTest
{
    public int NewTotalScore;
    public int PointsGained;
}


public class ScoreDisplayTest : MonoBehaviour
{
    private Text _scoreText;

    private void Awake()
    {
        _scoreText = GetComponent<Text>();
        _scoreText.text = "Score: 0";
    }

    private void OnEnable()
    {
        EventDispatcher.Instance.Subscribe<PlayerScoredEventTest>(OnPlayerScored);
    }

    private void OnDisable()
    {
        if (EventDispatcher.Instance != null)
        {
            EventDispatcher.Instance.Unsubscribe<PlayerScoredEventTest>(OnPlayerScored);
        }
    }

    private void OnPlayerScored(PlayerScoredEventTest eventData)
    {
        _scoreText.text = "Score: " + eventData.NewTotalScore;

        Debug.Log($"UI: {eventData.PointsGained} points gained!");
    }
}

public class PlayerControllerTest : MonoBehaviour
{
    private int _currentScore = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddScore(10);
        }
    }

    public void AddScore(int points)
    {
        _currentScore += points;
        Debug.Log($"Player: Gain {points} points! Total: {_currentScore}");

        PlayerScoredEventTest scoreEvent = new PlayerScoredEventTest
        {
            NewTotalScore = _currentScore,
            PointsGained = points
        };

        EventDispatcher.Instance.SendEvent(scoreEvent);
    }
}