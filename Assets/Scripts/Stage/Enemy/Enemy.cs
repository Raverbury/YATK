using System.Collections.Generic;
using Assets.Scripts.Util;
using MEC;
using Unity.Serialization;
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

    [DontSerialize, HideInInspector]
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
    private bool isInvulnerable = true;
    private bool hasRefilledHP = false;
    private bool shouldDieOnHPDepletion = false;
    private List<ItemStack> customItemDrops = new();
    public bool IsBoss = true;

    public UnityAction<float, float> EntitySetHP;
    public UnityAction EntityDie;

    [SerializeField, HideInInspector]
    private Animator animator;
    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private SpriteRenderer atuneRingSpriteRenderer;

    private Dictionary<FriendlyDamageArea, bool> touchingBombs = new();

    private void OnValidate()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (STG.Constant.LAYER_PLAYER_BOMB == other.gameObject.layer)
        {
            if (other.gameObject.TryGetComponent(out FriendlyDamageArea bomb))
            {
                touchingBombs.Add(bomb, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (STG.Constant.LAYER_PLAYER_BOMB == other.gameObject.layer)
        {
            if (other.gameObject.TryGetComponent(out FriendlyDamageArea bomb))
            {
                touchingBombs.Remove(bomb);
            }
        }
    }

    protected override void PausableUpdate()
    {
        foreach (var kvp in touchingBombs)
        {
            FriendlyDamageArea bomb = kvp.Key;
            TakeDamage(bomb.damage);
            if (IsDead()) {
                return;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
        {
            return;
        }
        damage = Mathf.Clamp(damage, 0f, HP);
        if (damage == 0f)
        {
            return;
        }
        HP -= damage;
        if (IsDead())
        {
            DropRewards();
            if (shouldDieOnHPDepletion)
            {
                StageManager.DestroyEnemy(this);
                // TODO: swap this for enep01 sound for boss?
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_EXPLODE, 0.3f);
            }
            else {
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_EXPLODE, 1f);
            }
        }
    }

    public bool IsDead()
    {
        return HP == 0 && hasRefilledHP;
    }

    public bool IsNearDeath()
    {
        return hasRefilledHP && HP < MaxHP * 0.1f && HP > 0f;
    }

    public void InitEnemy(List<ItemStack> dropRewards, bool isBossEnemy)
    {
        customItemDrops = dropRewards;
        shouldDieOnHPDepletion = !isBossEnemy;
        showHP = isBossEnemy;
        IsBoss = isBossEnemy;
        atuneRingSpriteRenderer.gameObject.SetActive(isBossEnemy);
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
        hasRefilledHP = false;
        isInvulnerable = true;
        HP = 0;
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
    /// Move an enemy to destination over some frames, setting anims automatically.<br/>
    /// If already at destination on call, then don't move/wait at all.<br/>
    /// Identical to _MoveEnemyToOver, except use fairy animation style
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="destination"></param>
    /// <param name="durationInFrames"></param>
    /// <returns></returns>
    public IEnumerator<float> _MoveEnemyToOverFairyStyle(Vector2 destination, int durationInFrames)
    {
        Vector3 initialPos = transform.position;
        float initialDistance = Vector2.Distance(initialPos, destination);
        float maxSpeed = initialDistance / durationInFrames;
        float angle = Mathf.Abs(initialPos.AngleTo(destination));
        if (Vector2.Distance(destination, initialPos) > 1f)
        {
            // moving up/down, use idle/forward
            if (angle > 45f && angle < 135f) {
                SetAnimState(AnimState.Idle);
            }
            // else use side
            else {
                SetAnimState(AnimState.Move);
            }
            for (int i = 0; i < durationInFrames; i++)
            {
                transform.position = Vector2.MoveTowards(transform.position, destination, maxSpeed);
                spriteRenderer.flipX = (transform.position.x > destination.x) || transform.position.x >= destination.x && spriteRenderer.flipX;

                yield return 1;
            }
        }
        SetAnimState(AnimState.Idle);
    }

    public IEnumerator<float> _MoveEnemyCircular(float startingAngleDegrees, float angularVelocityDegrees, float velocity, int durationFrames) {
        // transform.eulerAngles = new Vector3(0f, 0f, startingAngleDegrees);
        // TODO: return immediately if angular vel = 0?
        if (angularVelocityDegrees == 0f) {
            yield break;
        }
        float currentAngle = startingAngleDegrees + (angularVelocityDegrees > 0f? 90f : -90f);
        for (int i = 0; i < durationFrames; i++) {
            Quaternion rot = Quaternion.Euler(0f, 0f, currentAngle);
            transform.position += rot * Vector3.right * velocity;
            currentAngle += angularVelocityDegrees;
            yield return Timing.WaitForOneFrame;
        }
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
        isInvulnerable = true;
        MaxHP = maxHP;
        float hpStep = (float)maxHP / (durationInFrames - 1);
        for (int i = 0; i < durationInFrames; i++)
        {
            HP = Mathf.RoundToInt(hpStep * i);

            yield return 1;
        }
        isInvulnerable = false;
        hasRefilledHP = true;
    }

    /// <summary>
    /// Non-coroutine version of function to refill/set max HP, best used on enemies without a visible HP ring
    /// </summary>
    /// <param name="maxHP"></param>
    public void RefillHP(int maxHP)
    {
        MaxHP = maxHP;
        HP = maxHP;
        isInvulnerable = false;
        hasRefilledHP = true;
    }

    public void ChangeSprites(EnemyBossData enemyBossData)
    {
        AnimatorOverrideController aoc = new(animator.runtimeAnimatorController);
        aoc["enemy_front"] = enemyBossData.idleAnimation;
        aoc["enemy_side"] = enemyBossData.sideAnimation;
        aoc["enemy_attack"] = enemyBossData.attackAnimation;
        animator.runtimeAnimatorController = aoc;
    }

    public void ChangeSprites(EnemyData enemyData)
    {
        AnimatorOverrideController aoc = new(animator.runtimeAnimatorController);
        aoc["enemy_front"] = enemyData.idleAnimation;
        aoc["enemy_side"] = enemyData.sideAnimation;
        aoc["enemy_attack"] = enemyData.idleAnimation;
        animator.runtimeAnimatorController = aoc;
    }

    public void DropRewards()
    {
        if (customItemDrops.Count == 0)
        {
            RuntimeGameData.EnemyNaturalRewardDropCount = (RuntimeGameData.EnemyNaturalRewardDropCount + 1) % 22;
            switch (RuntimeGameData.EnemyNaturalRewardDropCount)
            {
                // small power item if player not at max power, else point item
                case 0:
                case 1:
                case 2:
                case 5:
                case 7:
                case 9:
                case 11:
                case 12:
                case 17:
                case 18:
                case 19:
                    ECSEntitySpawner.SpawnItemI1(gameObject, Player.instance.Power < 128 ? STG.ItemType.POWER_ITEM : STG.ItemType.POINT_ITEM);
                    break;
                // point item
                case 3:
                case 6:
                case 8:
                case 10:
                case 14:
                case 15:
                    ECSEntitySpawner.SpawnItemI1(gameObject, STG.ItemType.POINT_ITEM);
                    break;
                // small power item or point item, randomly
                case 4:
                case 16:
                    ECSEntitySpawner.SpawnItemI1(gameObject, Random.Range(0, 2) == 0 ? STG.ItemType.POWER_ITEM : STG.ItemType.POINT_ITEM);
                    break;
                // big power item
                case 21:
                    ECSEntitySpawner.SpawnItemI1(gameObject, STG.ItemType.BIG_POWER_ITEM);
                    break;
                // the rest is no drop
                default:
                    break;
            }
        }
        for (int i = 0; i < customItemDrops.Count; i++)
        {
            ItemStack reward = customItemDrops[i];
            for (int j = 0; j < reward.Count; j++)
            {
                ECSEntitySpawner.SpawnItemI1(
                gameObject.transform.position.x + Random.Range(-50f, 50f),
                gameObject.transform.position.y + Random.Range(-30f, 30f),
                reward.ItemType);
            }
        }
        customItemDrops.Clear();
    }
}
