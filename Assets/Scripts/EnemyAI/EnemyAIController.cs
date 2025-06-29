// Assets/Scripts/EnemyAI/EnemyAIController.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using EnemyAI;
using Characters.Player;
using CodeMonkey;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAIController : MonoBehaviour
{
    // ── State Assets 
    [Header("State Assets")]
    public CalmState      calmState;
    public AlertState     alertState;
    public SearchingState searchingState;
    public ChaseState     chaseState;

    public EnemyStateType prevState = EnemyStateType.Calm;
    // ── Player Reference 
    [Header("Player Reference")]
    [HideInInspector] public Transform playerTransform;
    private PlayerHide _playerHideScript;
    private Vector2 _lastKnownPlayerPosition;
    private Vector2 _playerStartPosition;
    [SerializeField] private Camera _camera; 
    [HideInInspector] public bool isAlertPatrolling = false;
    [HideInInspector] public bool isGoingToStarAlertPatrolling = false;
    // ── Patrol Settings (Calm)
    [Header("Patrol Settings (Calm)")]
    [Tooltip("X positions to patrol between")]
    public float[] patrolPoints;
    [HideInInspector] public int currentPatrolIndex = 0;
    [HideInInspector] public float patrolY;  // captured at Awake
    public static int ConversationEncounterCount = 5;
    // ── Patrol Settings (Alert)
    [Header("Alert Patrol")]
    [Tooltip("Half-width of the left/right sweep while the enemy is Alert")]
    public float alertPatrolRadius = 8f;

    [Header("vars for alert patrol")] 
    public float spreadRadius = 10f;
    public float alertSpeed = 1.25f;
    
    // ── Detection & Movement 
    [Header("Ranges & Speeds")]
    public float detectionRange      = 15f;
    public float calmMoveSpeed       = 2f;
    public float chaseMoveSpeed      = 4f;
    public float chaseDashSpeed      = 6f;
    public float searchMoveSpeed     = 2.5f;

    [Header("State Durations")]
    public float searchDuration = 7f;
    public float alertDuration = 30f;

    // ── Group Conversation Fields 
    [HideInInspector] public bool isConversing  = false;
    [HideInInspector] public bool conversationCompleted = false;
    [HideInInspector] public float conversationTimer = 0f;

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
    
    [Header("Enemy UI")]
    [SerializeField] public GameObject ExclamationIcon;
    [SerializeField] public GameObject QuestionIcon;
    [SerializeField] public Image filledQuestionIcon;
    private Vector3 _exclamationOriginalScale;
    private Vector3 _questionOriginalScale;
    private Vector3 _filledQuestionOriginalScale;
    
    private Vector2 _initialPosition;
    private IEnemyState _initialState;

    
    [Header("FOV Settings")]
    private float fovYOffset = 6.5f;
    private GameObject _fovMeshObject;
    private Vector3 _fovOriginalLocalScale;
    [SerializeField] private float fieldOfViewAngle = 120f;

    public static readonly List<EnemyAIController> AllEnemies = new List<EnemyAIController>();
    private float size;
    public EnemyStateType CurrentStateType { get; private set; }
    
    private bool walkingRight = false;
    private Rigidbody2D _rigidbody2D;
    private IEnemyState _currentState;
    private int _originalSpriteOrder;
    [SerializeField] SpriteRenderer _spriteRenderer;
    
    [Header("Searching state")]
    public float moveToNoiseTimer;

    void Awake()
    {
        _originalSpriteOrder = _spriteRenderer.sortingOrder;
        _rigidbody2D    = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        patrolY         = transform.position.y;
        AllEnemies.Add(this); 
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerTransform = playerTransform.transform;
        if (playerTransform != null)
        {
            _playerHideScript = playerTransform.GetComponent<PlayerHide>();
            _playerStartPosition = playerTransform.position;
        }
        size = transform.localScale.x;
        CreateFOVMesh();
        _initialPosition = transform.position;
        _initialState = calmState;
        initIcons();
    }
    

    void Start()
    {
        // Start in Calm
        _currentState    = calmState;
        CurrentStateType = EnemyStateType.Calm;
        _currentState.EnterState(this);
        UpdateSpriteColor();
        NoiseManager.OnNoiseRaised += HandleNoise;
    }
    
    void OnDestroy()
    {
        // unsubscribe
        AllEnemies.Remove(this);
        NoiseManager.OnNoiseRaised -= HandleNoise;
    }

    private void FixedUpdate()
    {
        if(!IsPlayerHiding()) _lastKnownPlayerPosition = playerTransform.position;
    }
    private bool IsWalkingRight() => _rigidbody2D.linearVelocity.x > 0.01f;

    private void Update()
    {
        // walkingRight = IsWalkingRight();
        // Debug.Log("walking right+"+ _rigidbody2D.linearVelocity.x );
        // transform.localScale = new Vector3(!walkingRight ? size : -size, size, size);
        _spriteRenderer.flipX = walkingRight;
        if (_fovMeshObject != null)
        {
            _fovMeshObject.transform.localScale = new Vector3(
                _fovOriginalLocalScale.x * (walkingRight ? 1f : -1f),
                _fovOriginalLocalScale.y,
                _fovOriginalLocalScale.z
            );
        }
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
        Debug.Log($"[Enemy] {name} -> {CurrentStateType}");
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
        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
        walkingRight = dir.x > 0;
        if (_rigidbody2D != null)
        {
           
            _rigidbody2D.linearVelocity = dir * speed;
        }
        else
        {
            transform.position = Vector2.MoveTowards
                (transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
    
    
    private void HandleNoise(Vector2 worldPos)
    {
        // forward the event into whatever state we’re in
        _currentState.OnNoiseRaised(worldPos, this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player") && !IsPlayerHiding())
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameManager.Instance.checkpoint(collision.transform);
        }
    }


    private void initIcons()
    {
        ExclamationIcon.SetActive(false);
        QuestionIcon.SetActive(false);
        filledQuestionIcon.gameObject.SetActive(false);
        _exclamationOriginalScale = ExclamationIcon.transform.localScale;
        _questionOriginalScale = QuestionIcon.transform.localScale;
        _filledQuestionOriginalScale = filledQuestionIcon.transform.localScale;
    }

    
    public void StopMovement()
    {
        Debug.Log("Stop Movement");
        if (_rigidbody2D != null) _rigidbody2D.linearVelocity = Vector2.zero;
    }

    public bool IsPlayerHiding(){
        return _playerHideScript != null && _playerHideScript.IsHiding();
    }

    public Vector2 GetLastKnownPlayerPosition() => _lastKnownPlayerPosition;
    
  
    public bool IsInChasingDistanceFromPlayer()
    {
        if (IsPlayerHiding()) return false;

        Vector2 origin = (Vector2)transform.position + new Vector2(0, 1.5f);
        Vector2 toPlayer = (Vector2)playerTransform.position - origin;

        // 1. Early out by distance
        if (toPlayer.sqrMagnitude > detectionRange * detectionRange)
            return false;

        // 2. Use dot product instead of expensive angle math
        Vector2 dirToPlayer = toPlayer.normalized;
        Vector2 facing = GetFacingDirection();

        float dot = Vector2.Dot(facing, dirToPlayer);
        float minDot = Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad); // precompute for performance

        return dot >= minDot;
    }



    
    private Vector2 GetFacingDirection()
    {
        return walkingRight ? Vector2.right : Vector2.left;
    }


    public bool GetIsWalkingRight() => walkingRight;

    public void ExclamationIconSwitch(bool turnOn)
    {
        ExclamationIcon.SetActive(turnOn);
    }
    public void QuesitonIconSwitch(bool turnOn)
    {
        QuestionIcon.SetActive(turnOn);
    }
    public void ResetEnemy()
    {
        transform.position = _initialPosition;
        ChangeState(_initialState);
        StopMovement();
    }
    public void PatrolEnemy()
    {
        transform.position = _initialPosition;
        ChangeState(_initialState);
        // StopMovement();
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(0, 1.5f);
        Vector2 facing = Application.isPlaying ? GetFacingDirection() : Vector2.right;

        float range = detectionRange;

        // Actual detection logic cone (half angle)
        float detectionHalfAngle = fieldOfViewAngle / 2f;

        // Visual: detection cone (used for logic)
        Vector2 leftLogic = Quaternion.Euler(0, 0, -detectionHalfAngle) * facing;
        Vector2 rightLogic = Quaternion.Euler(0, 0, detectionHalfAngle) * facing;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + leftLogic * range);
        Gizmos.DrawLine(origin, origin + rightLogic * range);

        // Visual: mesh cone (entire cone) for reference
        Gizmos.color = new Color(0, 1, 0, 0.5f); // green
        int rays = 20;
        float angleStep = fieldOfViewAngle / rays;
        for (int i = 0; i <= rays; i++)
        {
            float angle = -fieldOfViewAngle / 2f + i * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * facing;
            Gizmos.DrawLine(origin, origin + dir * range);
        }

        // Draw base arc (optional)
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(origin, range);
    }


    private void CreateFOVMesh()
    {
        _fovMeshObject = new GameObject("FOVMesh");
        _fovMeshObject.transform.SetParent(transform);
        _fovMeshObject.transform.localPosition = new Vector3(0f, fovYOffset, 0f);

        MeshFilter meshFilter = _fovMeshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = _fovMeshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = meshRenderer.material = Resources.Load<Material>("Sprite-Lit-Default");
        // Set color with reduced alpha (0 = transparent, 1 = opaque)
        Color newColor = meshRenderer.material.color;
        newColor.a = 0.3f; // for example, 30% visible
        meshRenderer.material.color = newColor;
        _fovMeshObject.GetComponent<Renderer>().sortingLayerName = "Player";
        _fovMeshObject.GetComponent<Renderer>().sortingOrder =10;
        _fovOriginalLocalScale = _fovMeshObject.transform.localScale;
        PolygonCollider2D polyCollider = _fovMeshObject.AddComponent<PolygonCollider2D>();
        _fovMeshObject.AddComponent<EnemyFOVTrigger>();
        polyCollider.isTrigger = true;
        
        Rigidbody2D rb = _fovMeshObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.gravityScale = 0;
        rb.interpolation = RigidbodyInterpolation2D.None;

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int rayCount = 30;
        float fov = fieldOfViewAngle;
        float viewDistance = detectionRange;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;
        float angleStep = fov / rayCount;
        float startAngle = -fov / 2f;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + i * angleStep;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
            vertices[i + 1] = dir * viewDistance;

            if (i < rayCount)
            {
                int idx = i * 3;
                triangles[idx] = 0;
                triangles[idx + 1] = i + 1;
                triangles[idx + 2] = i + 2;
            }
        }
        

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        Vector2[] colliderPoints = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colliderPoints[i] = vertices[i];
        }
        polyCollider.SetPath(0, colliderPoints);
    }

    /// <summary>
    /// Override the sprite’s sorting order at runtime.
    /// </summary>
    public void SetSortingOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
        _fovMeshObject.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Restore the original order from Awake().
    /// </summary>
    public void RestoreSortingOrder()
    {
        _spriteRenderer.sortingOrder = _originalSpriteOrder;
        _fovMeshObject.gameObject.SetActive(true);
    }
}


