using System.Collections;
using UnityEngine;
using UI;

/// <summary>
/// A SequenceFrame that moves a train GameObject from its current local start position
/// to a target GameObject's position over its displayTime, and draws a gizmo at the destination.
/// </summary>
public class PortalFrame : SequenceFrame
{
    [Header("Portal")]
    [SerializeField] private GameObject portal;
    private void Reset()
    {
        // Initialize frame displayTime if unset
        if (displayTime <= 0)
            displayTime = 1f;
    }
    
    public override void PlayFrame()
    {
       portal.SetActive(true);
    }
}