// NoiseManager.cs
using System;
using UnityEngine;

public static class NoiseManager
{
    /// <summary>
    /// Raised whenever something makes noise that enemies can investigate.
    /// </summary>
    public static event Action<Vector2> OnNoiseRaised;

    /// <summary>
    /// Call this to notify all listeners that a noise occurred at worldPosition.
    /// </summary>
    public static void RaiseNoise(Vector2 worldPosition)
    {
        OnNoiseRaised?.Invoke(worldPosition);
    }
}