using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public enum AnimState : int
    {
        Idle,
        Move,
        Attack,
    }

    public bool showHP = true;
    private int _maxHP = 0;
    public int MaxHP
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
    private int _hp = 0;
    public int HP
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

    public UnityAction<int, int> EntitySetHP;
    public UnityAction EntityDie;

    [SerializeField, HideInInspector]
    private Animator animator;

    private void OnValidate()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (STG.Constant.LAYER_PLAYER_BULLET == other.gameObject.layer)
        {
            if (other.gameObject.TryGetComponent(out PlayerBullet playerBullet))
            {
                int damage = Mathf.Clamp(playerBullet.damage, 0, HP);
                TakeDamage(damage);
            }
            other.gameObject.SetActive(false);
        }
    }

    private void TakeDamage(int damage)
    {
        HP -= damage;
    }

    public bool IsDead()
    {
        return HP <= 0;
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

    public static IEnumerator<float> MoveEnemyToOver(Enemy enemy, Vector2 destination, int durationInFrames)
    {
        Vector2 initialPos = enemy.transform.position;
        float initialDistance = Vector2.Distance(initialPos, destination);
        float maxSpeed = initialDistance / durationInFrames;
        enemy.SetAnimState(AnimState.Move);
        for (int i = 0; i < durationInFrames; i++)
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, destination, maxSpeed);

            yield return 1;
        }
        enemy.SetAnimState(AnimState.Idle);
    }
}
