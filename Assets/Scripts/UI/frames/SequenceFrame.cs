using UnityEngine;

namespace UI
{
    /// <summary>
    /// Base class for any “frame” in your opening sequence.
    /// Attach this to a GameObject and assign the sub‐elements in the Inspector.
    /// </summary>
    public abstract class SequenceFrame : MonoBehaviour
    {
        [Tooltip("How long to display this frame, in seconds.")]
        public float displayTime = 1f;

        /// <summary>
        /// Called when this frame should show itself.
        /// </summary>
        public abstract void PlayFrame();
    }
}