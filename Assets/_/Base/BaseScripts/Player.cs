/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using UnityEngine;
using V_AnimationSystem;
using CodeMonkey.Utils;
using CodeMonkey;

/*
 * Player movement with WASD
 * */
public class Player : MonoBehaviour, EnemyHandler.IEnemyTargetable {
    
    public static Player instance;

    private const float SPEED = 50f;
    
    private Player_Base playerBase;
    private Rigidbody2D playerRigidbody2D;
    private Vector3 moveDir;
    private State state;
    private int health;

    private enum State {
        Normal,
    }

    private void Awake() {
        instance = this;
        playerBase = gameObject.GetComponent<Player_Base>();
        playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        SetStateNormal();
        health = 4;
    }

    private void Update() {
        switch (state) {
        case State.Normal:
            HandleMovement();
            break;
        }
    }
    
    private void SetStateNormal() {
        state = State.Normal;
    }

    private void HandleMovement() {
        float moveX = 0f;
        float moveY = 0f;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            moveX = +1f;
        }

        moveDir = new Vector3(moveX, moveY).normalized;
    }

    private void FixedUpdate() {
        bool isIdle = moveDir.x == 0 && moveDir.y == 0;
        if (isIdle) {
            playerBase.PlayIdleAnim();
        } else {
            playerBase.PlayMoveAnim(moveDir);
            //transform.position += moveDir * SPEED * Time.deltaTime;
            playerRigidbody2D.MovePosition(transform.position + moveDir * SPEED * Time.fixedDeltaTime);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.name == "Trophy") {
            CMDebug.TextPopup("Win!", transform.position);
        }
    }

    public void DamageKnockback(Vector3 knockbackDir, float knockbackDistance) {
        transform.position += knockbackDir * knockbackDistance;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void Damage(EnemyHandler enemyHandler) {
    }

    public void Damage(CharacterWaypointsHandler enemyHandler) {
        Damage(enemyHandler.GetPosition());
    }

    public void Damage(Vector3 attackerPosition) {
        Vector3 bloodDir = (GetPosition() - attackerPosition).normalized;
        Blood_Handler.SpawnBlood(GetPosition(), bloodDir);
        // Knockback
        transform.position += bloodDir * 1.5f;
        health--;
        if (health == 0) {
            FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(), bloodDir);
            gameObject.SetActive(false);
            //transform.Find("Body").gameObject.SetActive(false);
        }
    }

    public bool IsDead() {
        return health <= 0;
    }

}
