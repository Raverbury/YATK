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
    public static UnityAction PlayerBomb;
    public static UnityAction<bool> PlayerSetFocus;

    public static UnityAction<float> PlayerIsInvulnerable;
    public static UnityAction<int> PlayerSetLife;
    public static UnityAction<int> PlayerSetGrazeboxRadius;
    public static UnityAction<int> PlayerSetGraze;
    public static UnityAction<int> PlayerSetPower;
    public static UnityAction PlayerAutoCollectItem;
    public static UnityAction PlayerSetMiss;

    public static UnityAction PlayerCollectItem;
    public static UnityAction PlayerPowerUp;
    public static UnityAction EVPlayerGraze;

    public static Player instance;

    [SerializeField]
    private PlayerData editorPlayerData = null;
    [DoNotSerialize, HideInInspector]
    public PlayerData playerData = null;

    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;
    [SerializeField, HideInInspector]
    private CircleCollider2D circleCollider2D;
    [SerializeField, HideInInspector]
    private Animator animator;
    [SerializeField, HideInInspector]
    private ObjectPool bulletPool;

    public GameObject weaponOrb;

    private float speed = 4f;
    private float focusSpeed = 2f;
    private int deathBombFrames = 10;

    private Vector2 moveDir = Vector2.zero;

    private int _remainingLife = 2;
    public int RemainingLife
    {
        get
        {
            return _remainingLife;
        }
        set
        {
            PlayerSetLife?.Invoke(value);
            _remainingLife = value;
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

    private short _pocLine = (short)(Constant.GAME_HEIGHT * -0.25);

    private int initialBomb = 3;

    private int framesLeftToDeathBomb = -1;

    private bool shouldMiss = false;
    private bool shouldCheckMovement = true;

    private bool pause = false;

    private void OnValidate()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        bulletPool = GetComponent<ObjectPool>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        // singleton check
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;

        if (null == editorPlayerData)
        {
            throw new System.Exception("No default PlayerData attached to Player");
        }
        // set player data/stats
        // TODO: retrieve player data from char select menu or something
        SetPlayerData(editorPlayerData);
        speed = playerData.unfocusedSpeed.GetFinalStat();
        focusSpeed = playerData.focusedSpeed.GetFinalStat();
        deathBombFrames = (int)playerData.deathBombWindow.GetFinalStat();
        circleCollider2D.radius = (float)playerData.hitboxRadius.GetFinalStat() / 100;
        RemainingLife = playerData.initialLife;
        initialBomb = playerData.initialBomb; // TODO: also add from bomb's stat if implemented
        PlayerSetGrazeboxRadius(25);
        Focus = false;
        // change anims
        AnimatorOverrideController aoc = new(animator.runtimeAnimatorController);
        aoc["front"] = playerData.frontAnimation;
        aoc["side"] = playerData.sideAnimation;
        animator.runtimeAnimatorController = aoc;

        // TODO: register passive/ability or smth
        // playerData.Register(this);
    }

    private void SetPlayerData(PlayerData playerData)
    {
        this.playerData = Instantiate(playerData);
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
            moveDir *= Focus ? focusSpeed : speed;
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
            if (Input.GetButton("Shoot"))
            {
                PlayerShoot?.Invoke();
            }
        }

        // check above poc line
        if (transform.position.y >= _pocLine)
        {
            PlayerAutoCollectItem?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Constant.LAYER_ENEMY_BULLET == other.gameObject.layer)
        {
            PlayerGetHit();
            other.gameObject.SetActive(false);
        }
        else if (Constant.LAYER_ITEM == other.gameObject.layer)
        {
            if (other.gameObject.TryGetComponent(out Item item))
            {
                item.CollectItem(this);
            }
            other.gameObject.SetActive(false);
        }
        else if (Constant.LAYER_ENEMY == other.gameObject.layer)
        {
            PlayerGetHit();
        }
    }

    private void PlayerGetHit()
    {
        // any invuln check
        if (isInvulnerable)
        {
            return;
        }
        shouldMiss = true;
        isInvulnerable = true;
        // circleCollider2D.enabled = false;
        framesLeftToDeathBomb = deathBombFrames;
        shouldCheckMovement = false;
        // play miss sound
        // MasterHolder.PlayerMiss?.Invoke();
    }

    private void PlayerMiss()
    {
        if (0 == RemainingLife)
        {
            // TODO: gameover
        }
        RemainingLife -= 1;
        PlayerSetMiss?.Invoke();
        shouldMiss = false;
        Timing.RunCoroutine(_SetInvulnerable(300));
        Timing.RunCoroutine(_DoRespawn());
    }

    public void PlayerGraze()
    {
        Graze += 1;
        EVPlayerGraze?.Invoke();
    }

    IEnumerator<float> _SetInvulnerable(int duration)
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
            StageManager.ClearBullet?.Invoke(false);
            yield return Timing.WaitForOneFrame;
        }
        shouldCheckMovement = true;
    }
}
