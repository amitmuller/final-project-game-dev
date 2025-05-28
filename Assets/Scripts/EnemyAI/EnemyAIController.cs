// Assets/Scripts/EnemyAI/EnemyAIController.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using EnemyAI;
using Characters.Player;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAIController : MonoBehaviour
{
    // ── State Assets 
    [Header("State Assets")]
    public CalmState      calmState;
    public AlertState     alertState;
    public SearchingState searchingState;
    public ChaseState     chaseState;

    // ── Player Reference 
    [Header("Player Reference")]
    [Tooltip("Drag your Player GameObject here")]
    public Transform playerTransform;
    private PlayerHide _playerHideScript;
    private Vector2 _lastKnownPlayerPosition;

    // ── Patrol Settings (Calm)
    [Header("Patrol Settings (Calm)")]
    [Tooltip("X positions to patrol between")]
    public float[] patrolPointsX;
    [HideInInspector] public int currentPatrolIndex = 0;
    [HideInInspector] public float patrolY;  // captured at Awake

    // ── Detection & Movement 
    [Header("Ranges & Speeds")]
    public float detectionRange      = 5f;
    public float calmMoveSpeed       = 2f;
    public float chaseMoveSpeed      = 4f;
    public float searchMoveSpeed     = 2.5f;

    [Header("State Durations")]
    public float alertDuration  = 10f;
    public float searchDuration = 10f;

    // ── Group Conversation Fields 
    [HideInInspector] public bool isConversing  = false;
    [HideInInspector] public bool conversationCompleted = false;
    [HideInInspector] public float conversationTimer = 0f;
    private bool _isInCameraSpace;

    // ── State Colors 
    [Header("State Colors (Sprite)")]
    [Tooltip("Color when in Calm state")]
    public Color calmStateColor = Color.white;
    [Tooltip("Color when in Alert state")]
    public Color alertStateColor = Color.yellow;
    [Tooltip("Color when in Searching state")]
    public Color searchingStateColor = Color.cyan;
    [Tooltip("Color when in Chase state")]
    public Color chaseStateColor = Color.red;

    // ── Runtime State Tracking 
    [HideInInspector] public float alertTimer;
    [HideInInspector] public float searchTimer;
    [HideInInspector] public Vector2 lastKnownNoisePosition;

    public static readonly List<EnemyAIController> AllEnemies = new List<EnemyAIController>();
    public EnemyStateType CurrentStateType { get; private set; }

    private Rigidbody2D _rigidbody2D;
    private IEnemyState _currentState;
    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _rigidbody2D    = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        patrolY         = transform.position.y;

        if (playerTransform != null)
            _playerHideScript = playerTransform.GetComponent<PlayerHide>();
    }

    void Start()
    {
        // Start in Calm
        _currentState    = calmState;
        CurrentStateType = EnemyStateType.Calm;
        _currentState.EnterState(this);
        UpdateSpriteColor();
        NoiseManager.OnNoiseRaised += HandleNoise; //subscribe to noise manager
    }
    
    void OnDestroy()
    {
        // unsubscribe
        NoiseManager.OnNoiseRaised -= HandleNoise;
    }

    void Update()
    {
        _lastKnownPlayerPosition = playerTransform.position;
        _currentState.UpdateState(this);
    }
    

    /// <summary>
    /// Switch to a new state and update sprite color.
    /// </summary>
    public void ChangeState(IEnemyState newState)
    {
        if (newState == null || newState == _currentState) return;

        _currentState.ExitState(this);
        _currentState    = newState;
        CurrentStateType = newState.StateType;
        Debug.Log($"[EnemyAI] {name} → {CurrentStateType}");
        _currentState.EnterState(this);
        UpdateSpriteColor();
    }

    /// <summary>
    /// Set the sprite’s color based on CurrentStateType.
    /// </summary>
    private void UpdateSpriteColor()
    {
        if (_spriteRenderer == null) return;
        switch (CurrentStateType)
        {
            case EnemyStateType.Calm:
                _spriteRenderer.color = calmStateColor;
                break;
            case EnemyStateType.Alert:
                _spriteRenderer.color = alertStateColor;
                break;
            case EnemyStateType.Searching:
                _spriteRenderer.color = searchingStateColor;
                break;
            case EnemyStateType.Chase:
                _spriteRenderer.color = chaseStateColor;
                break;
        }
    }

    public void MoveTowards(Vector2 targetPosition, float speed)
    {
        if (_rigidbody2D != null)
        {
            Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
            _rigidbody2D.linearVelocity = dir * speed;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    public void StopMovement()
    {
        if (_rigidbody2D != null)
            _rigidbody2D.linearVelocity = Vector2.zero;
    }

    public bool IsPlayerHiding() => _playerHideScript != null && _playerHideScript.IsHiding();

    public Vector2 GetLastKnownPlayerPosition() => _lastKnownPlayerPosition;
    public bool IsVisibleOnCamera() => _isInCameraSpace;
    
    private void HandleNoise(Vector2 worldPos)
    {
        // forward the event into whatever state we’re in
        _currentState.OnNoiseRaised(worldPos, this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
            _isInCameraSpace = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
            _isInCameraSpace = false;
    }
}
