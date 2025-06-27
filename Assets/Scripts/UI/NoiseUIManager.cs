using Characters.Player;
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
    [SerializeField] private float noiseThreshold = 0.8f; // normalized 0–1
    [SerializeField] private float decayRate = 0.5f;       // how fast it fades down
    [SerializeField] private float noiseCooldown = 1f;
    [SerializeField] private PlayerHide player;
    
    [Header("Waveform Settings")]
    [SerializeField] private LineRenderer waveformLine;
    private int     waveformPoints     = 256;
    [SerializeField] private float   envelopeSpeed      = 0.5f;  // burst travel speed
    [SerializeField] private float   envelopePower      = 4f;    // higher = sharper bursts
    [SerializeField] private float   burstFrequency     = 6f;    // base cycles in a burst
    [SerializeField] private float   burstVariation     = 12f;   // extra freq added in peaks
    [SerializeField] private float   amplitudeBoost     = 2.5f;  // global boost for drama

    [Header("Micro-Wave Settings")]
    [SerializeField] private float   microCycles        = 30f;   // tiny fast ripples
    [SerializeField] private float   microSpeed         = 3f;    // how fast those ripples scroll
    [SerializeField] private float   microAmp           = 0.15f; // ripple amplitude fraction

    [Header("Jitter")]
    [SerializeField] private float   jitterAmount       = 0f;  // hand-drawn wobble
    private float currentNoise = 0f;
    private float noiseTimer = 0f;
    private float waveOffset   = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (waveformLine != null)
        {
            waveformLine.positionCount = waveformPoints;
            waveformLine.useWorldSpace = false;
        }
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
            waveOffset += Time.deltaTime * envelopeSpeed;
            UpdateUI();
        }

        // Trigger noise event if over threshold
        if (currentNoise >= noiseThreshold && noiseTimer <= 0f && !player.IsHiding())
        {
            NoiseManager.RaiseNoise(player.transform.position); 
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
            noiseBarFill.color = Color.Lerp(Color.white, Color.red, currentNoise);
        }
        if (noiseBarFill   != null) noiseBarFill.enabled = false;
        if (waveformLine   != null) DrawWaveform(currentNoise);
    }
    
    private void DrawWaveform(float level)
    {
        var r      = noiseBarRect.rect;
        float halfW = r.width  * 0.5f;
        float halfH = r.height * 0.5f;

        // color
        Color c = level >= noiseThreshold 
            ? Color.red 
            : Color.Lerp(Color.green, Color.yellow, level);
        waveformLine.startColor = waveformLine.endColor = c;

        for (int i = 0; i < waveformPoints; i++)
        {
            float t = i / (float)(waveformPoints - 1);

            // 1) envelope for bursts
            float env = Mathf.PerlinNoise(t * envelopeSpeed + waveOffset, waveOffset);
            env = Mathf.Pow(env, envelopePower);

            // 2) dynamic frequency & amplitude
            float freq = burstFrequency + env * burstVariation;
            float amp  = halfH * level * env * amplitudeBoost;

            // 3) core wave + scrolling
            float phase = t * freq * Mathf.PI * 2f + waveOffset;
            float y1    = Mathf.Sin(phase);

            // 4) micro-ripples on top
            float microPhase = t * microCycles * Mathf.PI * 2f + waveOffset * microSpeed;
            float y2         = Mathf.Sin(microPhase) * microAmp;

            // 5) jitter for organic feel
            float j = (Mathf.PerlinNoise(t * 10f, waveOffset * 2f) * 2f - 1f) * jitterAmount;

            float x = Mathf.Lerp(-halfW, +halfW, t);
            float y = (y1 + y2 + j) * amp;

            waveformLine.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
    

    public void reset()
    {
        noiseTimer = 0f;
    }


}
