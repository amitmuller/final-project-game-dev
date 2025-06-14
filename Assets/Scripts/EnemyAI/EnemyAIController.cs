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
    public float[] patrolPointsX;
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
    public float searchMoveSpeed     = 2.5f;

    [Header("State Durations")]
    public float searchDuration = 10f;

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

    public static readonly List<EnemyAIController> AllEnemies = new List<EnemyAIController>();
    private float size;
    public EnemyStateType CurrentStateType { get; private set; }
    
    private bool walkingRight = false;
    private Rigidbody2D _rigidbody2D;
    private IEnemyState _currentState;
    [SerializeField] SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _rigidbody2D    = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        patrolY         = transform.position.y;
        AllEnemies.Add(this); 
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log(playerTransform);
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
        NoiseManager.OnNoiseRaised += HandleNoise; //subscribe to noise manager
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
        walkingRight = IsWalkingRight();
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
        Debug.Log($"[EnemyAI] {name} -> {CurrentStateType}");
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    // private void LateUpdate()
    // {
    //     if (ExclamationIcon != null)
    //         ExclamationIcon.transform.localScale = _exclamationOriginalScale;
    //
    //     if (QuestionIcon != null)
    //         QuestionIcon.transform.localScale = _questionOriginalScale;
    //
    //     if (filledQuestionIcon != null)
    //         filledQuestionIcon.transform.localScale = _filledQuestionOriginalScale;
    // }


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
        if (_rigidbody2D != null) _rigidbody2D.linearVelocity = Vector2.zero;
    }

    public bool IsPlayerHiding(){
        return _playerHideScript != null && _playerHideScript.IsHiding();
    }

    public Vector2 GetLastKnownPlayerPosition() => _lastKnownPlayerPosition;
    
    public bool IsInChasingDistanceFromPlayer()
    {
        
        
        Vector2 origin = (Vector2)transform.position + new Vector2(0, 1.5f);
        Vector2 toPlayer = new Vector2(playerTransform.position.x, playerTransform.position.y) - origin;
        Vector2 direction = GetFacingDirection(); // or any direction you want
        float length = detectionRange;

        Debug.DrawLine(origin, origin + direction * length, Color.red);
        if (toPlayer.magnitude > detectionRange)
            return false;

        Vector2 facing = GetFacingDirection();
        float angle = Vector2.Angle(facing, toPlayer.normalized);

        if (angle <= 30f && !IsPlayerHiding()) // half of 60°
        {
            return true;
        }

        return false;
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
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = (Vector2)transform.position+new Vector2(0, 1.5f);
        Vector2 facing = Application.isPlaying ? GetFacingDirection() : Vector2.right;

        float range = detectionRange;
        float angle = 30f;

        Vector2 leftDir = Quaternion.Euler(0, 0, -angle) * facing;
        Vector2 rightDir = Quaternion.Euler(0, 0, angle) * facing;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + leftDir * range);
        Gizmos.DrawLine(origin, origin + rightDir * range);
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

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int rayCount = 30;
        float fov = 60f;
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
    }

}
