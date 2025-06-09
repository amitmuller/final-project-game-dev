using UnityEngine;
using System.Collections;

namespace Interactable_objects
{
    [RequireComponent(typeof(Collider2D))]
    public class FragmentBehavior : MonoBehaviour
    {
        private float triggerDelay = 0.5f;

        private Collider2D col;

        void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        void OnEnable()
        {
            StartCoroutine(BecomeTriggerAfterDelay());
        }

        private IEnumerator BecomeTriggerAfterDelay()
        {
            yield return new WaitForSeconds(triggerDelay);
            Debug.Log("Become trigger");

            if (col != null)
            {
                col.isTrigger = true;
            }
        }
    }

}