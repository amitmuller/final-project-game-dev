using UnityEngine;
using System.Collections;

namespace Interactable_objects
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class FragmentBehavior : MonoBehaviour
    {
        public string stickySurfaceTag = "sticky";
        private bool stuck = false;

        private Collider2D col;

        void Awake()
        {
            col = GetComponent<Collider2D>();
        }
            

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.gameObject.tag);
            if (stuck) return;
            
            if (other.CompareTag(stickySurfaceTag))
            {
                // Stick to closest point on the collider
                StartCoroutine(stickToPoint(other));
            }
        }

        private IEnumerator stickToPoint(Collider2D other)
        {
            float delay = Random.Range(0.1f, 1f); // Random delay between 0.1 and 0.5 seconds
            yield return new WaitForSeconds(delay);
            
            Vector2 closest = other.ClosestPoint(transform.position);
            StickToPoint(closest);
            col.isTrigger = true;
        }

        private void StickToPoint(Vector2 point)
        {
            stuck = true;

            // Move object to contact point
            transform.position = point;

            // Freeze Rigidbody
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static; // Freeze in place
        }
    

        void OnEnable()
        {
            // StartCoroutine(BecomeTriggerAfterDelay());
        }

        // private IEnumerator BecomeTriggerAfterDelay()
        // {
        //     // yield return new WaitForSeconds(triggxerDelay);
        //     Debug.Log("Become trigger");
        //
        //     if (col != null)
        //     {
        //         col.isTrigger = true;
        //     }
        // }
    }

}