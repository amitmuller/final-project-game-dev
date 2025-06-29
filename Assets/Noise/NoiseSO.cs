using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseSO", menuName = "ScriptableObjects/NoiseSO", order = 1)]
public class NoiseSO : ScriptableObject
{
    [Tooltip("Amplitude of the sine wave, controls the height of the wave")]
    public float amplitude = 1;
    [Tooltip("Frequency of the sine wave, controls the cycles of the wave")]
    public float frequency = 1;
    [Tooltip("Number of points in the sine wave, higher values will result in a smoother wave")]
    public int resolution = 100;
    [Tooltip("Speed of the wave, controls how fast the wave moves over time")]
    public float speed = 1;

    public float amplitude2 = 1;
    public float frequency2 = 1;

    [Min(1)]
    public int anchor = 11;
}
