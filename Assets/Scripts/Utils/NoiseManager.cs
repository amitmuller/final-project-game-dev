// NoiseManager.cs
using System;
using UnityEngine;

public static class NoiseManager
{
    public static event Action<Vector2> OnNoiseRaised;

    /// <summary>Most recent noise position.</summary>
    public static Vector2 LastNoisePosition { get; private set; }
    /// <summary>Time.time when that noise happened.</summary>
    public static float   LastNoiseTime     { get; private set; }

    public static void RaiseNoise(Vector2 worldPosition)
    {
        LastNoisePosition = worldPosition;
        LastNoiseTime     = Time.time;
        OnNoiseRaised?.Invoke(worldPosition);
    }
}
