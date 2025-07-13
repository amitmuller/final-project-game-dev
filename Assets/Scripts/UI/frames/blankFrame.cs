using UnityEngine;
using UI;

public class BlankFrame : SequenceFrame
{
    /// <summary>
    /// No-op: this frame just sits on screen for displayTime seconds.
    /// </summary>
    public override void PlayFrame()
    {
        // Intentionally empty.
    }
}