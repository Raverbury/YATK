using System;
using System.Collections.Generic;
using MEC;
using STG;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Animator))]
public class Player : PausableMono
{
    public static UnityAction PlayerShoot;
    public static UnityAction<bool> PlayerUseBomb;
    public static UnityAction<bool> PlayerSetFocus;

    public static UnityAction<float> PlayerIsInvulnerable;
    public static UnityAction<int> PlayerSetLife;
    public static UnityAction<int> EVPlayerSetRemainingBombAmount;
    public static UnityAction<int> PlayerSetGraze;
    public static UnityAction<int> PlayerSetPower;
    public static UnityAction PlayerAutoCollectItem;
    public static UnityAction PlayerSetMiss;

    public static UnityAction PlayerCollectItem;
    public static UnityAction PlayerPowerUp;
    public static UnityAction EVPlayerGraze;
    /// <summary>
    /// An event that fires after using a bomb, first int is iframe duration, second int is how many bombs is consumed.
    /// </summary>
    public static UnityAction<int, int> EVBombActivate;

    public static UnityAction ResultPlayerExtend;

    public static Player instance;

    [DoNotSerialize, HideInInspector]
    public PlayerData playerData = null;

    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;
    [SerializeField, HideInInspector]
    private CircleCollider2D circleCollider2D;
    [SerializeField, HideInInspector]
    private Animator animator;

    private int deathBombFrames = 10;

    private Vector2 moveDir = Vector2.zero;

    private int remainingLife = 2;
    public int RemainingLife
    {
        get
        {
            return remainingLife;
        }
        set
        {
            PlayerSetLife?.Invoke(value);
            remainingLife = value;
        }
    }

    private int bomb;
    public int RemainingBomb
    {
        get
        {
            return bomb;
        }
        set
        {
            bomb = value;
            EVPlayerSetRemainingBombAmount?.Invoke(value);
        }
    }

    private int _power = 0;
    public int Power
    {
        get
        {
            return _power;
        }
        set
        {
            PlayerSetPower?.Invoke(value);
            _power = value;
        }
    }

    private int _graze = 0;
    public int Graze
    {
        get
        {
            return _graze;
        }
        set
        {
            PlayerSetGraze?.Invoke(value);
            _graze = value;
        }
    }

    private bool _focus = false;
    public bool Focus
    {
        get
        {
            return _focus;
        }
        set
        {
            if (value != _focus)
            {
                PlayerSetFocus?.Invoke(value);
                _focus = value;
            }
        }
    }

    private bool isInvulnerable = false;

    private float pocLine = Constant.GAME_HEIGHT * -0.25f;

    private int initialBomb = 3;

    private int framesLeftToDeathBomb = -1;

    private bool shouldMiss = false;
    private bool shouldCheckMovement = true;
    private bool canBomb = true;

    private bool autofire = false;

    private void OnValidate()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EVBombActivate += BombActivated;
    }

    private void OnDisable()
    {
        EVBombActivate -= BombActivated;
    }

    // Start is called before the first frame update
    void Awake()
    {
        // singleton check
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        if (null == RuntimeGameData.SelectedPlayerData)
        {
            throw new Exception("Selected player data is not set!");
        }
        // set player data/stats
        SetPlayerData(RuntimeGameData.SelectedPlayerData);

        // PLAN: register passive/ability or smth
    }

    private void SetPlayerData(PlayerData playerData)
    {
        this.playerData = Instantiate(playerData);
        // set stats
        pocLine = (short)(Constant.GAME_HEIGHT * -playerData.ItemCollectionLine.GetFinalStat());
        deathBombFrames = (int)playerData.deathBombWindow.GetFinalStat();
        circleCollider2D.radius = (float)playerData.hitboxRadius.GetFinalStat() / 100;
        RemainingLife = playerData.initialLife;
        initialBomb = playerData.initialBomb; // PLAN: also add from bomb's stat if implemented
        RemainingBomb = initialBomb;
        Focus = false;
        // change anims
        AnimatorOverrideController aoc = new(animator.runtimeAnimatorController);
        aoc["front"] = playerData.frontAnimation;
        aoc["side"] = playerData.sideAnimation;
        animator.runtimeAnimatorController = aoc;
    }

    protected override void PausableUpdate()
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
            // change focus
            Focus = Input.GetButton("Focus");

            // movement
            moveDir.x = Input.GetAxisRaw("Horizontal");
            moveDir.y = Input.GetAxisRaw("Vertical");
            moveDir.Normalize();
            Vector3 pos = transform.position;
            moveDir *= Focus ? playerData.focusedSpeed.GetFinalStat() : playerData.unfocusedSpeed.GetFinalStat();
            float newX = pos.x + moveDir.x;
            float newY = pos.y + moveDir.y;
            pos.x = (newX < Constant.GAME_BORDER_LEFT + 5) ?
                Constant.GAME_BORDER_LEFT + 5 : (newX > Constant.GAME_BORDER_RIGHT - 5) ?
                    Constant.GAME_BORDER_RIGHT - 5 : newX;
            pos.y = (newY > Constant.GAME_BORDER_TOP - 10) ?
                Constant.GAME_BORDER_TOP - 10 : (newY < Constant.GAME_BORDER_BOTTOM + 10) ?
                    Constant.GAME_BORDER_BOTTOM + 10 : newY;
            transform.position = pos;
            animator.Play(0 == moveDir.x ? "front" : "side");
            spriteRenderer.flipX = (moveDir.x > 0) || moveDir.x >= 0 && spriteRenderer.flipX;

            // shoot
            if (autofire || Input.GetButton("Shoot"))
            {
                PlayerShoot?.Invoke();
            }
        }


        // bomb
        if (Input.GetButton("Bomb"))
        {
            if (RemainingBomb > 0)
            {
                if (canBomb)
                {
                    PlayerUseBomb?.Invoke(shouldMiss);
                }
            }
        }

        // toggle autofire
        if (Input.GetButtonDown("Autofire"))
        {
            autofire = !autofire;
        }

        // check above poc line
        if (transform.position.y >= pocLine)
        {
            PlayerAutoCollectItem?.Invoke();
        }
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (Constant.LAYER_ENEMY_BULLET == other.gameObject.layer)
    //     {
    //         PlayerGetHit();
    //         if (other.gameObject.TryGetComponent(out EnemyBullet enemyBullet)) {
    //             enemyBullet.ClearBullet(false);
    //         }
    //         else {
    //             other.gameObject.SetActive(false);
    //         }
    //     }
    //     else if (Constant.LAYER_ITEM == other.gameObject.layer)
    //     {
    //         if (other.gameObject.TryGetComponent(out Item item))
    //         {
    //             item.CollectItem(this);
    //         }
    //         other.gameObject.SetActive(false);
    //     }
    //     else if (Constant.LAYER_ENEMY == other.gameObject.layer)
    //     {
    //         PlayerGetHit();
    //     }
    // }

    public void PlayerGetHit()
    {
        // any invuln check
        if (isInvulnerable)
        {
            return;
        }
        shouldMiss = true;
        isInvulnerable = true;
        PlayerSetMiss?.Invoke();
        framesLeftToDeathBomb = deathBombFrames;
        if (RemainingBomb == 0)
        {
            framesLeftToDeathBomb = 1;
        }
        shouldCheckMovement = false;
    }

    private void PlayerMiss()
    {
        if (0 == RemainingLife)
        {
            Vector3 pos = transform.position;
            pos.x = Constant.GAME_CENTER_X;
            pos.y = -500f;
            transform.position = pos;
            StageManager.RequestPlayerRunOutOfLife?.Invoke();
            return;
        }
        RemainingLife -= 1;
        RemainingBomb = initialBomb;
        shouldMiss = false;
        CoroutineUtil.RunPlayerInvulnerableCoroutine(_SetInvulnerable(300));
        Timing.RunCoroutine(_DoRespawn());
    }

    public void PlayerGraze()
    {
        Graze += 1;
        EVPlayerGraze?.Invoke();
    }

    public void PlayerExtend()
    {
        RemainingLife += 1;
        ResultPlayerExtend?.Invoke();
    }

    private void BombActivated(int invulnerableDuration, int bombCost)
    {
        shouldMiss = false;
        shouldCheckMovement = true;
        RemainingBomb = Mathf.Max(0, RemainingBomb - bombCost);
        CoroutineUtil.RunPlayerInvulnerableCoroutine(_SetBombInvulnerable(invulnerableDuration));
    }

    private IEnumerator<float> _SetBombInvulnerable(int duration)
    {
        canBomb = false;
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(_SetInvulnerable(duration)));
        canBomb = true;
    }

    private IEnumerator<float> _SetInvulnerable(int duration)
    {
        // circleCollider2D.enabled = false;
        isInvulnerable = true;
        for (int __delay = duration; __delay > 0; __delay--)
        {
            if (__delay % 4 == 0)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            // dispatch invulnerable timer
            PlayerIsInvulnerable.Invoke((float)__delay / duration);
            yield return Timing.WaitForOneFrame;
        }
        // circleCollider2D.enabled = true;
        isInvulnerable = false;
        spriteRenderer.enabled = true;
    }

    IEnumerator<float> _DoRespawn()
    {
        canBomb = false;
        animator.Play("front");
        const int REVIVE_DURATION = 60;
        const int REVIVE_STEP_DISTANCE = 7;
        shouldCheckMovement = false;
        Vector3 pos = transform.position;
        pos.x = Constant.GAME_CENTER_X;
        pos.y = -360 - REVIVE_STEP_DISTANCE * REVIVE_DURATION;
        transform.position = pos;
        for (int __delay = 0; __delay < REVIVE_DURATION; __delay++)
        {
            pos.y += REVIVE_STEP_DISTANCE;
            transform.position = pos;
            StageManager.ClearEnemyBullet?.Invoke(false, false);
            yield return Timing.WaitForOneFrame;
        }
        shouldCheckMovement = true;
        canBomb = true;
    }
}
