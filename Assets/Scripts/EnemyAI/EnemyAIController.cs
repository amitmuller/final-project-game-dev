// EnemyAIController.cs
using UnityEngine;
using Characters.Player;  // for PlayerHide
using EnemyAI;           // for state interfaces & enums

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAIController : MonoBehaviour
{
    [Header("State Assets")]
    public CalmState      calmState;
    public AlertState     alertState;
    public SearchingState searchingState;
    public ChaseState     chaseState;

    [Header("Initial Settings")]
    public EnemyStateType initialState       = EnemyStateType.Calm;
    public Transform      playerTransform;    // assign your Player here
    public float          detectionRange     = 5f;
    public float          noiseDetectionRange = 5f;

    [Header("Movement Speeds")]
    public float calmMoveSpeed   = 2f;
    public float chaseMoveSpeed  = 4f;
    public float searchMoveSpeed = 2.5f;

    [Header("State Durations")]
    public float alertDuration  = 1.5f;
    public float searchDuration = 5f;

    // Runtime-only fields
    [HideInInspector] public float alertTimer;
    [HideInInspector] public float searchTimer;
    [HideInInspector] public Vector2 lastKnownNoisePosition = Vector2.zero;

    [HideInInspector] public PlayerHide playerHideScript;

    private Rigidbody2D _rigidbody2D;
    private IEnemyState _currentState;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        // Cache PlayerHide if it exists
        if (playerTransform != null)
            playerHideScript = playerTransform.GetComponent<PlayerHide>();
    }

    void OnEnable()
    {
        NoiseManager.OnNoiseRaised += HandleNoiseRaised;
    }

    void OnDisable()
    {
        NoiseManager.OnNoiseRaised -= HandleNoiseRaised;
    }

    void Start()
    {
        // Pick the initial state asset
        switch (initialState)
        {
            case EnemyStateType.Alert:
                _currentState = alertState;
                break;
            case EnemyStateType.Searching:
                _currentState = searchingState;
                break;
            case EnemyStateType.Chase:
                _currentState = chaseState;
                break;
            case EnemyStateType.Calm:
            default:
                _currentState = calmState;
                break;
        }
        _currentState.EnterState(this);
    }

    void Update()
    {
        _currentState.UpdateState(this);
    }

    /// <summary>
    /// Called when some object raises a noise.
    /// </summary>
    private void HandleNoiseRaised(Vector2 noisePosition)
    {
        float distanceToNoise = Vector2.Distance(transform.position, noisePosition);
        // Only investigate if within noiseDetectionRange and not already chasing
        if (distanceToNoise <= noiseDetectionRange &&
            _currentState != chaseState)
        {
            lastKnownNoisePosition = noisePosition;
            ChangeState(searchingState);
        }
    }

    /// <summary>
    /// Switch to a new state.
    /// </summary>
    public void ChangeState(IEnemyState newState)
    {
        if (newState == null || newState == _currentState) return;

        _currentState.ExitState(this);
        _currentState = newState;
        Debug.Log($"[EnemyAI] {name} → {_currentState.StateType}");
        _currentState.EnterState(this);
    }

    /// <summary>
    /// Move toward targetPosition at given speed.
    /// Uses Rigidbody2D if available, else falls back to Transform.
    /// </summary>
    public void MoveTowards(Vector2 targetPosition, float moveSpeed)
    {
        if (_rigidbody2D != null)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            _rigidbody2D.velocity = direction * moveSpeed;
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Stops all movement immediately.
    /// </summary>
    public void StopMovement()
    {
        if (_rigidbody2D != null)
            _rigidbody2D.velocity = Vector2.zero;
    }
}
