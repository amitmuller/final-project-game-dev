using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int attackPower;
    [SerializeField] private int superAttackPower;
    [SerializeField] private float timeOfAttack = 0.3f;
    [FormerlySerializedAs("attackRadius")]
    [SerializeField] private Vector2 hitboxSize = new Vector2(2f, 2f);

    [Header("Explosion Settings")]
    [Tooltip("Base explosion force (effectively scaled by mass and wearoff)")]
    [SerializeField] private float explosionForce = 50f;
    [Tooltip("Optional upward force factor (0 to disable)")]
    [SerializeField] private float upliftModifier = 0f;
    [SerializeField] private float attackBacklashForce = 5f;
    [SerializeField] private MMF_Player attackFeedback;

    [Header("Hitbox Collider")]
    [Tooltip("Box collider used for detecting attackable objects")]
    [SerializeField] private BoxCollider2D hitboxCollider;

    private Rigidbody2D _rb;
    private PlayerMove _move;
    private characterGround _ground;
    private bool isAttacking = false;
    private readonly List<Collider2D> targetsInRange = new List<Collider2D>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ground = GetComponent<characterGround>();
        _move = GetComponent<PlayerMove>();

        if (hitboxCollider == null)
            Debug.Log("Hitbox Collider is not assigned on PlayerAttack", this);
        else
        {
            hitboxCollider.isTrigger = true;
            hitboxCollider.size = hitboxSize;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
            Attack();
    }

    private void Attack()
    {
        isAttacking = true;
        Invoke(nameof(NotAttacking), timeOfAttack);

        
        

        // Process each target
        var snapshot = targetsInRange.ToArray();
        foreach (var col in snapshot)
        {
            if (col == null) { targetsInRange.Remove(col); continue; }
            var body = col.attachedRigidbody;
            if (body == null) continue;
            if (col.CompareTag("breakableObject"))
            {
                col.GetComponent<BreakObjects>()?.BreakObject();
                attackFeedback?.PlayFeedbacks();
                // Player knockback
                _rb.AddForce(-FacingDirection * attackBacklashForce, ForceMode2D.Impulse);
                // AddExplosionForce(body, explosionForce, transform.position, hitboxSize.magnitude * 0.5f, upliftModifier);
                targetsInRange.Remove(col);
            }
        }
    }

    private void NotAttacking()
    {
        isAttacking = false;
    }

    private Vector2 FacingDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("breakableObject") && !targetsInRange.Contains(other))
            targetsInRange.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("breakableObject"))
            targetsInRange.Remove(other);
    }

    private void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftMod)
    {
        Debug.Log("add back force");
        Vector2 dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        if (wearoff <= 0) return;
        Vector2 force = dir.normalized * explosionForce * wearoff / body.mass;
        body.AddForce(force, ForceMode2D.Force);
        if (upliftMod > 0f)
        {
            Vector2 uplift = Vector2.up * explosionForce * upliftMod * wearoff / body.mass;
            body.AddForce(uplift, ForceMode2D.Force);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (hitboxCollider != null)
            Gizmos.DrawWireCube(hitboxCollider.bounds.center, hitboxCollider.size);
        else
            Gizmos.DrawWireCube(transform.position, hitboxSize);
    }
}
