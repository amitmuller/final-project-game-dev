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
        private float noiseLevelToAdd = 0.1f;
        private static Material s_blackUnlit;

        void Awake()
        {
            col = GetComponent<Collider2D>();
            if (s_blackUnlit == null)
            {
                s_blackUnlit = new Material(Shader.Find("Unlit/Color"));
                s_blackUnlit.color = Color.black;
            }

            CreateBackMeshOutline();
        }
        
        private void CreateBackMeshOutline()
        {
            // grab the existing mesh
            var mf = GetComponent<MeshFilter>();
            if (mf == null || mf.mesh == null) return;

            // make a child that uses the same mesh
            var go = new GameObject("Outline");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            // scale it up slightly so the black peeks out
            go.transform.localScale = Vector3.one * 1.3f;

            // copy mesh
            var of = go.AddComponent<MeshFilter>();
            of.mesh = mf.mesh;

            // give it our black material
            var or = go.AddComponent<MeshRenderer>();
            or.material = s_blackUnlit;

            // match sorting so it sits behind the fragment
            var myRenderer = GetComponent<MeshRenderer>();
            or.sortingLayerID = myRenderer.sortingLayerID;
            or.sortingOrder   = myRenderer.sortingOrder - 1;
        }
            

        private void OnTriggerEnter2D(Collider2D other)
        {
            
            
            if (other.CompareTag(stickySurfaceTag))
            {
                if (stuck) return;
                StartCoroutine(stickToPoint(other));
            }

            if (stuck && other.CompareTag("Player"))
            {
                // NoiseUIManager.Instance?.AddNoise(noiseLevelToAdd);
            }
            
        }

        private IEnumerator stickToPoint(Collider2D other)
        {
            float delay = Random.Range(0.1f, 0.5f); // Random delay between 0.1 and 0.5 seconds
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