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

        /*  Properties for PlayerHide  */

        private void Start()
        {
            var bounds = GetComponent<Collider2D>().bounds;
            LeftX = bounds.min.x;
            RightX = bounds.max.x;
            var prefab = Resources.Load<GameObject>("hideIcon");
            if (prefab)
            {
                indicatorInstance = Instantiate(prefab, transform);
                indicatorInstance.transform.localPosition = new Vector3(0, 1f, 0); // adjust offset as needed
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