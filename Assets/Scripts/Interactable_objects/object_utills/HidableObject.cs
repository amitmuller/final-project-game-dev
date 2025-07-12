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
        // private GameObject indicatorInstance;
        [SerializeField] private Transform indicatorRight;
        [SerializeField] private Transform indicatorLeft;
        
        [SerializeField] private Transform iconRight;
        [SerializeField] private Transform iconLeft;
        
        [Header("Indicator Sorting (layer/order)")]
        [Tooltip("Sorting Order when hiding behind (back) furniture")]
        [SerializeField] private int indicatorBackOrder  = 6;
        [Tooltip("Sorting Order when hiding in front of (front) furniture")]
        [SerializeField] private int indicatorFrontOrder = 40;
        
        
        

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
            
            Transform oldParent = iconRight.parent;
            iconRight.transform.parent = null;
            iconRight.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            iconRight.transform.parent = oldParent;
            
            Transform oldParent2 = iconLeft.parent;
            iconLeft.transform.parent = null;
            iconLeft.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            iconLeft.transform.parent = oldParent2;
            
            if (hideLayer == HideLayer.Front)
            {
                iconRight.GetComponent<SpriteRenderer>().sortingOrder = indicatorFrontOrder;  
                iconLeft.GetComponent<SpriteRenderer>().sortingOrder = indicatorFrontOrder;
            }
            else
            {
                iconRight.GetComponent<SpriteRenderer>().sortingOrder = indicatorBackOrder; 
                iconLeft.GetComponent<SpriteRenderer>().sortingOrder = indicatorBackOrder;
            }

            setOffAllIndicator();

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
                setOffAllIndicator();
            }
        }
        
        public void setOffAllIndicator()
        {
            indicatorLeft.gameObject.SetActive(false);
            indicatorRight.gameObject.SetActive(false);
        }
        
        

        public void setIndicator(bool turnOn, HideEdge edge)
        {
            switch (edge)
            {
                case HideEdge.Left:
                    iconLeft.gameObject.SetActive(turnOn);
                    indicatorLeft.gameObject.SetActive(turnOn);
                    break;
                case HideEdge.Right:
                    iconRight.gameObject.SetActive(turnOn);
                    indicatorRight.gameObject.SetActive(turnOn);
                    break;
                case HideEdge.None:
                    if (indicatorLeft.gameObject.activeSelf)
                    {
                        indicatorLeft.gameObject.SetActive(turnOn);
                    }
                    if (indicatorRight.gameObject.activeSelf)
                    {
                        indicatorRight.gameObject.SetActive(turnOn);
                    }
                    break;
            }
        }
        
        public void setPartialIndicator(bool turnOn, HideEdge edge)
        {
            switch (edge)
            {
                case HideEdge.Left:
                    indicatorLeft.gameObject.SetActive(turnOn);
                    iconLeft.gameObject.SetActive(false);
                    break;
                case HideEdge.Right:
                    indicatorRight.gameObject.SetActive(turnOn);
                    iconRight.gameObject.SetActive(false);
                    break;
                case HideEdge.None:
                    if (indicatorLeft.gameObject.activeSelf)
                    {
                        indicatorLeft.gameObject.SetActive(turnOn);
                    }
                    if (indicatorRight.gameObject.activeSelf)
                    {
                        indicatorRight.gameObject.SetActive(turnOn);
                    }
                    break;
            }
        }
    }
}