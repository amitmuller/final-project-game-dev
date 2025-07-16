using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private List<CameraCheckpoint> cameraCheckpoints = new List<CameraCheckpoint>();
    [SerializeField] private List<SpriteRenderer> outdoorSections = new List<SpriteRenderer>();
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        foreach (CameraCheckpoint checkpoint in cameraCheckpoints)
        {
            checkpoint.OnPlayerPassedCheckpoint += CameraCheckpoint_OnPlayerPassedCheckpoint;
        }
    }

    private void CameraCheckpoint_OnPlayerPassedCheckpoint(
        object sender, CameraCheckpoint.OnPlayerPassedCheckpointEventArgs e)
    {
        if (e.isGoingOutdoors)
        {
            // Activate outdoor sections
            foreach (SpriteRenderer section in outdoorSections)
            {
                section.DOKill();
                section.DOFade(1f, fadeDuration);
            }
        }
        else
        {
            // Deactivate outdoor sections
            foreach (SpriteRenderer section in outdoorSections)
            {
                section.DOKill();
                section.DOFade(0f, fadeDuration);
            }
        }
    }
}
