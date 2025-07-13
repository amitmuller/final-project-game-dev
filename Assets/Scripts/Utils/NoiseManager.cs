using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseManager : MonoBehaviour
{
    public static event Action<Vector2> OnNoiseRaised;

    /// <summary>Most recent noise position.</summary>
    public static Vector2 LastNoisePosition { get; private set; }
    /// <summary>Time.time when that noise happened.</summary>
    public static float   LastNoiseTime     { get; private set; }

    public static NoiseManager Instance { get; private set; }

    [Header("Ripple Prefab (in Resources)")]
    [Tooltip("ParticleSystem prefab for your sound ripple.")]
    public string rippleResourcePath = "Sound ripple";

    // Pool of live ripples so we can clear them later
    private readonly List<ParticleSystem> _activeRipples = new List<ParticleSystem>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    /// <summary>
    /// Call this to raise noise at a given world position.
    /// Instantiates a ripple and schedules it for cleanup.
    /// </summary>
    public void RaiseNoise(Vector2 worldPosition)
    {
        LastNoisePosition = worldPosition;
        LastNoiseTime     = Time.time;
        OnNoiseRaised?.Invoke(worldPosition);

        // Load the prefab from Resources (you did this once before)
        var noisePrefab = Resources.Load<ParticleSystem>("Sound ripple");
        if (noisePrefab == null)
        {
            Debug.LogError($"[{nameof(NoiseManager)}] Couldn’t load '{rippleResourcePath}' from Resources");
            return;
        }

        // Instantiate a new ripple at the noise position
        var ripple = Instantiate(noisePrefab, worldPosition, Quaternion.identity, transform);
        _activeRipples.Add(ripple);

        // Ensure it starts immediately
        var main = ripple.main;
        main.startDelay = 0f;
        ripple.Play();

        // Schedule its destruction after its duration + max lifetime
        float lifetime = main.duration + main.startLifetime.constantMax;
        StartCoroutine(DestroyAfter(ripple, lifetime));
    }

    /// <summary>
    /// Destroys a ripple instance after the given delay, and removes it from the active list.
    /// </summary>
    private IEnumerator DestroyAfter(ParticleSystem ps, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (ps != null)
        {
            _activeRipples.Remove(ps);
            Destroy(ps.gameObject);
        }
    }

    /// <summary>
    /// Immediately stops and destroys all active ripples (e.g. on player death).
    /// </summary>
    public void ClearAllRipples()
    {
        // Stop any pending coroutines so we don't double-destroy
        StopAllCoroutines();

        foreach (var ripple in _activeRipples)
        {
            if (ripple != null)
            {
                ripple.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                Destroy(ripple.gameObject);
            }
        }

        _activeRipples.Clear();
    }

    private void OnEnable()
    {
        GameManager.OnPlayerDead += HandlePlayerDead;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerDead -= HandlePlayerDead;
    }

    private void HandlePlayerDead()
    {
        // When the player dies, wipe out any ripples immediately
        ClearAllRipples();
    }
}
