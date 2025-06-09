using System;
using UnityEngine;
using Interactable_objects.object_utills.enums;

namespace Interactable_objects
{
    [RequireComponent(typeof(Collider2D))]
    public class HidableObject : MonoBehaviour
    {

        [Header("Rendering")]
        [SerializeField] private HideLayer hideLayer = HideLayer.Back;
        public HideLayer Layer => hideLayer;
        private GameObject indicatorInstance;

        [Header("Hide boundaries")]
        public float LeftX;
        public float RightX;
        public float TopY;

        /*  Properties for PlayerHide  */

        private void Start()
        {
            var bounds = GetComponent<Collider2D>().bounds;
            LeftX = bounds.min.x;
            RightX = bounds.max.x;
            TopY = bounds.max.y;
            var prefab = Resources.Load<GameObject>("hideIcon");
            if (prefab)
            {
                indicatorInstance = Instantiate(prefab);
                indicatorInstance.transform.localPosition = new Vector3((LeftX + RightX)/2,
                    TopY +0.5f, 0f);

// // Reset transform BEFORE parenting
//                 indicatorInstance.transform.localScale = Vector3.one;
//                 indicatorInstance.transform.rotation = Quaternion.identity;
//
// // Parent safely: this avoids inheriting world scale
//                 indicatorInstance.transform.SetParent(transform, worldPositionStays: false);
//
// // Set final local position relative to parent
//                 indicatorInstance.transform.localPosition = new Vector3(0f, 1f, 0f);
//                 indicatorInstance.transform.localRotation = Quaternion.identity;
//                 indicatorInstance.transform.localScale = Vector3.one; // defensive second call


            }
            else
            {
                Debug.LogError("Couldn't load hideIcon prefab from Resources", this);
            }
            indicatorInstance.SetActive(false);
        }

        
        

        /*  Let PlayerHide know we’re nearby  */
        private void OnTriggerEnter2D(Collider2D other)
        {
            var ph = other.GetComponent<Characters.Player.PlayerHide>();
            if (ph)
            {
                ph.SetNearbyHidable(this);
                // indicatorInstance.SetActive(true);
            
            }
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            var ph = other.GetComponent<Characters.Player.PlayerHide>();
            if (ph)
            {
                if (ph.IsHiding())
                {
                    // indicatorInstance.SetActive(false);
                }
                else
                {
                    // indicatorInstance.SetActive(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var ph = other.GetComponent<Characters.Player.PlayerHide>();
            if (ph && !ph.IsHiding())
            {
                ph.SetNearbyHidable(null);
                indicatorInstance.SetActive(false);
            }
        }

        public void setIndicator(bool turnOn)
        {
            
            indicatorInstance.SetActive(turnOn);
        }
    }
}