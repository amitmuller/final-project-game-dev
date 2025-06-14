using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NoiseUIManager : MonoBehaviour
{
    public static NoiseUIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image noiseBarFill;
    [SerializeField] private RectTransform thresholdMarkerRect;
    [FormerlySerializedAs("speedBarRect")] [SerializeField] private RectTransform noiseBarRect;

    [Header("Noise Settings")]
    [SerializeField] private float noiseThreshold = 0.8f; // normalized 0–1
    [SerializeField] private float decayRate = 0.5f;       // how fast it fades down
    [SerializeField] private float noiseCooldown = 1f;

    private float currentNoise = 0f;
    private float noiseTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PositionThresholdMarker();
    }

    private void Update()
    {
        noiseTimer -= Time.deltaTime;
        Debug.Log(currentNoise);

        if (currentNoise > 0f)
        {
            currentNoise -= decayRate * Time.deltaTime;
            currentNoise = Mathf.Max(0f, currentNoise);
            UpdateUI();
        }

        // Trigger noise event if over threshold
        if (currentNoise >= noiseThreshold && noiseTimer <= 0f)
        {
            NoiseManager.RaiseNoise(transform.position); // Or another global point
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
    }

    private void PositionThresholdMarker()
    {
        if (thresholdMarkerRect == null || noiseBarRect == null) return;

        float thresholdPercent = Mathf.Clamp01(noiseThreshold); // Value from 0 to 1
        float barWidth = noiseBarRect.rect.width;

        // Calculate X offset from left edge
        float localX = barWidth * thresholdPercent;

        // Set anchored position
        Vector2 newPos = thresholdMarkerRect.anchoredPosition;
        newPos.x = localX;
        thresholdMarkerRect.anchoredPosition = newPos;

        Debug.Log($"Threshold marker set to {localX} px ({thresholdPercent * 100}% of bar width)");
    }


}
