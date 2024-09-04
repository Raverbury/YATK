using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider2D;
    private Animator animator;

    public float speed = 3.7f;
    public float focusSpeed = 1.5f;

    private Vector2 moveDir;

    // Start is called before the first frame update
    void Awake()
    {
        moveDir = Vector2.zero;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");
        moveDir.Normalize();
        transform.Translate(moveDir * (Input.GetButton("Focus") ? focusSpeed : speed));
        animator.Play(0 == moveDir.x ? "Default.anim_reimu_front" : "Default.anim_reimu_side");
        spriteRenderer.flipX = moveDir.x > 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (STG.Constant.LAYER_ENEMY_BULLET == other.gameObject.layer || STG.Constant.LAYER_ENEMY == other.gameObject.layer)
        {
            // MasterHolder.PlayerMiss?.Invoke();
            // Debug.Log("Ded");
            // gameObject.SetActive(false);
        }
    }
}
