using DG.Tweening;
using UnityEngine;

/// <summary>
/// The arrow that show where can player move
/// </summary>
public class PlayerIndicator : MonoBehaviour
{
    [Header("Arrow Object References")]
    [SerializeField] private GameObject _northArrow;
    [SerializeField] private GameObject _eastArrow;
    [SerializeField] private GameObject _southArrow;
    [SerializeField] private GameObject _westArrow;

    [Header("Settings")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.1f, 0);

    [SerializeField] private PlayerController _player;

    void Awake()
    {
        SetAllArrowsActive(false);
    }

    void OnEnable()
    {
        this.Subscribe<OnTurnChangedEvent>(HandleTurnChanged);
    }

    void OnDisable()
    {
        this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChanged);
    }

    void Start()
    {
        UpdateIndicators();

        float bobDistance = 0.15f;
        float bobDuration = 0.5f; 

        _northArrow.transform.DOLocalMoveZ(_northArrow.transform.localPosition.z + bobDistance, bobDuration)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetLink(gameObject);

        _southArrow.transform.DOLocalMoveZ(_southArrow.transform.localPosition.z - bobDistance, bobDuration)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetLink(gameObject);

        _eastArrow.transform.DOLocalMoveX(_eastArrow.transform.localPosition.x + bobDistance, bobDuration)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetLink(gameObject);

        _westArrow.transform.DOLocalMoveX(_westArrow.transform.localPosition.x - bobDistance, bobDuration)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetLink(gameObject);
    }

    private void HandleTurnChanged(OnTurnChangedEvent eventData)
    {
        if (eventData.NewTurn == TurnType.PlayerPlanning)
        {
            UpdateIndicators();
        }
        else
        {
            SetAllArrowsActive(false);
        }
    }

    private void UpdateIndicators()
    {
        if (_player == null || _player.CurrentNode == null) return;

        transform.position = _player.transform.position + _offset;

        Node current = _player.CurrentNode;

        _northArrow.SetActive(IsWalkable(current.NorthNode));
        _eastArrow.SetActive(IsWalkable(current.EastNode));
        _southArrow.SetActive(IsWalkable(current.SouthNode));
        _westArrow.SetActive(IsWalkable(current.WestNode));
    }

    private bool IsWalkable(Node node)
    {
        return node != null && node.IsWalkable();
    }

    private void SetAllArrowsActive(bool active)
    {
        _northArrow.SetActive(active);
        _eastArrow.SetActive(active);
        _southArrow.SetActive(active);
        _westArrow.SetActive(active);
    }
}