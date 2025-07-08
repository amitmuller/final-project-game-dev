using System;
using Characters.Player;
using Radishmouse;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NoiseUIManager : MonoBehaviour
{
    public static NoiseUIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image noiseBarFill;
    [FormerlySerializedAs("speedBarRect")] [SerializeField] private RectTransform noiseBarRect;

    [Header("Noise Settings")]
    [SerializeField] private float noiseThreshold = 0.9f; // normalized 0–1
    [SerializeField] private float decayRate = 0.5f;       // how fast it fades down
    [SerializeField] private float noiseCooldown = 1f;
    [SerializeField] private PlayerHide player;
    [SerializeField] private ParticleSystem noiseEffect;
    
    [Header("Waveform Settings")]
    [SerializeField] private UILineRenderer waveformLine;
    [SerializeField] NoiseSO noiseSO;

    private float currentNoise = 0f;
    private float noiseTimer = 0f;
    private float waveOffset   = 0f;

    [Header("Initial SO values (from inspector)")]
    [Tooltip("Amplitude of the sine wave, controls the height of the wave")]
    [SerializeField] private float initialAmplitude  = 0;
    [Tooltip("Frequency of the sine wave, controls the cycles of the wave")]
    [SerializeField] private float initialFrequency  =  2.27f;
    [Tooltip("How many points in the wave")]
    [SerializeField] private int   initialResolution = 422;
    [Tooltip("Scroll speed of the wave")]
    [SerializeField] private float initialSpeed      = 16.88f;
    [Tooltip("Micro-ripple amplitude")]
    [SerializeField] private float initialAmplitude2 =  0f;
    [Tooltip("Micro-ripple frequency")]
    [SerializeField] private float initialFrequency2 =  1.52f;
    [Tooltip("Anchor index (custom use)")]
    [SerializeField] private int   initialAnchor     = 23;
    [Header("Max SO values (for level==1)")]
    [SerializeField] private float maxAmplitude       =  1f;
    [SerializeField] private float maxAmplitude2      =  0.72f;
    [SerializeField] private float maxFrequency2      = 2.14f;

    
    private ParticleSystem burst;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        burst = Instantiate(
            noiseEffect,
            player.transform.position,
            Quaternion.identity,
            player.transform
        );
        burst.transform.localPosition = Vector3.zero;
        ResetSOValues();
        UpdateUI();

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        noiseTimer -= Time.deltaTime;
        if (currentNoise > 0f)
        {
            currentNoise -= decayRate * Time.deltaTime;
            currentNoise = Mathf.Max(0f, currentNoise);
            UpdateUI();
        }

        // Trigger noise event if over threshold
        if (currentNoise >= noiseThreshold && noiseTimer <= 0f && !player.IsHiding())
        {
            NoiseManager.RaiseNoise(player.transform.position); 
            burst.Play();
            noiseTimer = noiseCooldown;
        }
    }

    public void AddNoise(float intensity)
    {
        currentNoise = Mathf.Clamp01(currentNoise + intensity);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (noiseBarFill != null)
        {
            noiseBarFill.fillAmount = currentNoise;
            noiseBarFill.color = currentNoise >= noiseThreshold
                ? Color.red
                : Color.Lerp(Color.green, Color.yellow, currentNoise);;
        }
        if (noiseBarFill   != null) noiseBarFill.enabled = false;
        if (waveformLine   != null) DrawWaveform(currentNoise);
    }
    
    private void DrawWaveform(float level)
    {
        Color c = currentNoise >= noiseThreshold 
            ? Color.red 
            : Color.Lerp(Color.green, Color.yellow, currentNoise);
        waveformLine.color = c;
        noiseSO.amplitude   = Mathf.Lerp(initialAmplitude,  maxAmplitude,  level);
        noiseSO.amplitude2  = Mathf.Lerp(initialAmplitude2, maxAmplitude2, level);
        noiseSO.frequency2  = Mathf.Lerp(initialFrequency2, maxFrequency2, level);
    }
    
    /// <summary>
    /// Call this to blast your SO back to the original, inspector-set values.
    /// </summary>
    [ContextMenu("Reset SO Values")]
    public void ResetSOValues()
    {
        if (noiseSO == null) return;

        noiseSO.amplitude   = initialAmplitude;
        noiseSO.frequency   = initialFrequency;
        noiseSO.resolution  = initialResolution;
        noiseSO.speed       = initialSpeed;
        noiseSO.amplitude2  = initialAmplitude2;
        noiseSO.frequency2  = initialFrequency2;
        noiseSO.anchor      = initialAnchor;
    }

    /// <summary>
    /// Your existing checkpoint‐callable reset: resets timer, waveOffset AND SO.
    /// </summary>
    public void reset()
    {
        noiseTimer = 0f;
        waveOffset = 0f;
        currentNoise = 0f;
        ResetSOValues();
        UpdateUI();
    }


}
