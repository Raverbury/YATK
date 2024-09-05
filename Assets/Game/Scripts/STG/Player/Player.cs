using System;
using System.Collections.Generic;
using MEC;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    public static Player instance;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider2D;
    private Animator animator;

    public float speed = 3.7f;
    public float focusSpeed = 1.5f;

    private Vector2 moveDir;

    private int _remainingLife = 2;
    public int remainingLife
    {
        get
        {
            return _remainingLife;
        }
    }

    private readonly int deathBombFrames = 10;
    private int framesLeftToDeathBomb = -1;

    private bool shouldMiss = false;
    private bool shouldCheckMovement = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        moveDir = Vector2.zero;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        // check miss
        if (framesLeftToDeathBomb > 0)
        {
            framesLeftToDeathBomb -= 1;
        }
        else if (shouldMiss)
        {
            PlayerMiss();
        }

        // check movement
        if (shouldCheckMovement)
        {
            moveDir.x = Input.GetAxisRaw("Horizontal");
            moveDir.y = Input.GetAxisRaw("Vertical");
            moveDir.Normalize();
            Vector3 pos = transform.position;
            moveDir *= Input.GetButton("Focus") ? focusSpeed : speed;
            float newX = pos.x + moveDir.x;
            float newY = pos.y + moveDir.y;
            pos.x = (newX < STG.Constant.GAME_BORDER_LEFT) ?
                STG.Constant.GAME_BORDER_LEFT : (newX > STG.Constant.GAME_BORDER_RIGHT) ?
                    STG.Constant.GAME_BORDER_RIGHT : newX;
            pos.y = (newY > STG.Constant.GAME_BORDER_TOP) ?
                STG.Constant.GAME_BORDER_TOP : (newY < STG.Constant.GAME_BORDER_BOTTOM) ?
                    STG.Constant.GAME_BORDER_BOTTOM : newY;
            transform.position = pos;
            animator.Play(0 == moveDir.x ? "Default.anim_reimu_front" : "Default.anim_reimu_side");
            spriteRenderer.flipX = (moveDir.x > 0) || moveDir.x >= 0 && spriteRenderer.flipX;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (STG.Constant.LAYER_ENEMY_BULLET == other.gameObject.layer || STG.Constant.LAYER_ENEMY == other.gameObject.layer)
        {
            // MasterHolder.PlayerMiss?.Invoke();
            // Debug.Log("Ded");
            // gameObject.SetActive(false);
            PlayerGetHit();
        }
    }

    private void PlayerGetHit()
    {
        // any invuln check
        shouldMiss = true;
        circleCollider2D.enabled = false;
        framesLeftToDeathBomb = deathBombFrames;
        shouldCheckMovement = false;
        // play miss sound
        MasterHolder.PlayerMiss?.Invoke();
    }

    private void PlayerMiss()
    {
        if (0 == remainingLife)
        {
            // gameover
        }
        _remainingLife -= 1;
        LifeCounter.SetLife?.Invoke(remainingLife);
        shouldMiss = false;
        Timing.RunCoroutine(_SetInvulnerable(300));
        Timing.RunCoroutine(_DoRespawn());
        // update ui
    }

    IEnumerator<float> _SetInvulnerable(int duration)
    {
        circleCollider2D.enabled = false;
        for (int __delay = duration; __delay > 0; __delay--)
        {
            if (__delay % 4 == 0)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            BroadcastMessage("SetMagicCircleRadius", (float)__delay / duration, SendMessageOptions.DontRequireReceiver);
            yield return Timing.WaitForOneFrame;
        }
        circleCollider2D.enabled = true;
        spriteRenderer.enabled = true;
    }

    IEnumerator<float> _DoRespawn()
    {
        animator.Play("Default.anim_reimu_front");
        const int REVIVE_DURATION = 60;
        const int REVIVE_STEP_DISTANCE = 7;
        shouldCheckMovement = false;
        Vector3 pos = transform.position;
        pos.x = STG.Constant.GAME_CENTER_X;
        pos.y = -360 - REVIVE_STEP_DISTANCE * REVIVE_DURATION;
        transform.position = pos;
        for (int __delay = 0; __delay < REVIVE_DURATION; __delay++)
        {
            pos.y += REVIVE_STEP_DISTANCE;
            transform.position = pos;
            yield return Timing.WaitForOneFrame;
        }
        shouldCheckMovement = true;
    }
}
