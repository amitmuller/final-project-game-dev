using UnityEngine;

namespace Characters.Enemies
{
    public enum MovementType
    {
        Static,
        OneDirection,
        BackAndForth
    }

    [CreateAssetMenu(fileName = "NewEnemyMovementData", menuName = "Enemy/Movement Data")]
    public class EnemyMovementData : ScriptableObject
    {
        public MovementType movementType;

        [Header("Direction and Speed")]
        public Vector2 direction = Vector2.right;
        public float speed = 2f;
        public float moveDistance = 3f;

        [Header("Hold Times")]
        public float holdTimeAtStart = 0.5f;
        public float holdTimeAtEnd = 0.5f;
    }

}