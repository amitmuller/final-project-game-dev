using UnityEngine;

//This script is used by both movement and jump to detect when the character is touching the ground

public class characterGround : MonoBehaviour
{
        private bool onGround;
       
        [Header("Collider Settings")]
        [SerializeField][Tooltip("Length of the ground-checking collider")] private float groundLength = 0.95f;
        [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 colliderOffset;
        [SerializeField] private float width;
        [SerializeField] private float height;
        [SerializeField][Tooltip("Vertical offset for the ground-checking box")] private float yOffset = 0.1f;

        [Header("Layer Masks")]
        [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask groundLayer;
 

        private void Update()
        {
            //Determine if the player is stood on objects on the ground layer, using a pair of raycasts
            Vector2 boxCenter = (Vector2)transform.position + Vector2.down * yOffset;
            onGround = Physics2D.OverlapBox(boxCenter, new Vector2(width, height), 0f, groundLayer);

            // onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        }

        private void OnDrawGizmos()
        {
            //Draw the ground colliders on screen for debug purposes
            // if (onGround) { Gizmos.color = Color.magenta; } else { Gizmos.color = Color.yellow; }
            // Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
            // Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);

            Vector2 boxCenter = (Vector2)transform.position + Vector2.down * yOffset;
            Gizmos.color = onGround ? Color.magenta : Color.yellow;
            Gizmos.DrawWireCube(boxCenter, new Vector3(width, height, 0f));
        }

        //Send ground detection to other scripts
        public bool GetOnGround() { return onGround; }
}