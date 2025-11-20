using Characters.Player;
using Core.TurnSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Events
{

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
        
    }

    public struct OnEnemyActionStartedEvent
    {
        
    }

    public struct OnEnemyActionFinishedEvent
    {
        
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
    
    

    public class ScoreDisplayTest:MonoBehaviour
    {
        private Text _scoreText;

        private void Awake()
        {
            _scoreText = GetComponent<Text>();
            _scoreText.text = "Score: 0"; 
        }

        private void OnEnable()
        {
            NewEventDispatcher.Instance.Subscribe<PlayerScoredEventTest>(OnPlayerScored);
        }

        private void OnDisable()
        {
            if (NewEventDispatcher.Instance != null)
            {
                NewEventDispatcher.Instance.Unsubscribe<PlayerScoredEventTest>(OnPlayerScored);
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

            NewEventDispatcher.Instance.SendEvent(scoreEvent);
        }
    }
}