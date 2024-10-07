using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : PausableMono
{
    public enum AnimState : int
    {
        Idle,
        Move,
        Attack,
    }

    public bool showHP = true;
    private float _maxHP = 0;
    public float MaxHP
    {
        get
        {
            return _maxHP;
        }
        set
        {
            _maxHP = value;
            HP = value;
        }
    }
    private float _hp = 0;
    public float HP
    {
        get
        {
            return _hp;
        }
        private set
        {
            EntitySetHP?.Invoke(value, MaxHP);
            _hp = value;
        }
    }
    public bool IsInvulnerable = true;
    public bool HasRefilledHP = false;

    public UnityAction<float, float> EntitySetHP;
    public UnityAction EntityDie;

    [SerializeField, HideInInspector]
    private Animator animator;
    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;

    private Dictionary<Bomb, bool> touchingBombs = new();

    private void OnValidate()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (STG.Constant.LAYER_PLAYER_BULLET == other.gameObject.layer)
        {
            if (other.gameObject.TryGetComponent(out PlayerBullet playerBullet))
            {
                float damage = Mathf.Clamp(playerBullet.damage, 0f, HP);
                TakeDamage(damage);
            }
            other.gameObject.SetActive(false);
        }
        if (STG.Constant.LAYER_PLAYER_BOMB == other.gameObject.layer)
        {
            if (other.gameObject.TryGetComponent(out Bomb bomb))
            {
                touchingBombs.Add(bomb, true);

            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (STG.Constant.LAYER_PLAYER_BOMB == other.gameObject.layer)
        {
            if (other.gameObject.TryGetComponent(out Bomb bomb))
            {
                touchingBombs.Remove(bomb);
            }
        }
    }

    protected override void PausableUpdate()
    {
        foreach (var kvp in touchingBombs)
        {
            Bomb bomb = kvp.Key;
            float damage = Mathf.Clamp(bomb.damage, 0f, HP);
            TakeDamage(damage);
        }
    }

    private void TakeDamage(float damage)
    {
        if (IsInvulnerable)
        {
            return;
        }
        HP -= damage;
    }

    public bool IsDead()
    {
        return HP == 0;
    }

    public void SetAnimState(AnimState state)
    {
        string animClipName = state switch
        {
            AnimState.Move => "side",
            AnimState.Attack => "attack",
            _ => "front",
        };
        animator.Play(animClipName);
    }

    public void SetEmptyHpCircle()
    {
        EntitySetHP?.Invoke(-1, 1);
        HasRefilledHP = false;
    }

    /// <summary>
    /// Move an enemy to destination over some frames, setting anims automatically
    /// If already at destination on call, then don't move/wait at all
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="destination"></param>
    /// <param name="durationInFrames"></param>
    /// <returns></returns>
    public IEnumerator<float> _MoveEnemyToOver(Vector2 destination, int durationInFrames)
    {
        Vector2 initialPos = transform.position;
        float initialDistance = Vector2.Distance(initialPos, destination);
        float maxSpeed = initialDistance / durationInFrames;
        if (Vector2.Distance(destination, initialPos) > 1f)
        {
            SetAnimState(AnimState.Move);
            for (int i = 0; i < durationInFrames; i++)
            {
                transform.position = Vector2.MoveTowards(transform.position, destination, maxSpeed);
                spriteRenderer.flipX = (transform.position.x > destination.x) || transform.position.x >= destination.x && spriteRenderer.flipX;

                yield return 1;
            }
        }
        SetAnimState(AnimState.Idle);
    }

    /// <summary>
    /// Refill the HP circle over some frames (for cinematic purposes), HP starts from 0
    /// Enemy is invulnerable while doing so
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="destination"></param>
    /// <param name="durationInFrames"></param>
    /// <returns></returns>
    public IEnumerator<float> _RefillHPOver(int maxHP, int durationInFrames)
    {
        IsInvulnerable = true;
        MaxHP = maxHP;
        float hpStep = (float)maxHP / (durationInFrames - 1);
        for (int i = 0; i < durationInFrames; i++)
        {
            HP = Mathf.RoundToInt(hpStep * i);

            yield return 1;
        }
        IsInvulnerable = false;
        HasRefilledHP = true;

    }
}
