using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Animator))]
[RequireComponent(typeof(BulletPool))]
public class Player : MonoBehaviour
{
    public static UnityAction PlayerShoot;
    public static UnityAction PlayerBomb;
    public static UnityAction<bool> PlayerSetFocus;

    public static UnityAction<float> PlayerIsInvulnerable;
    public static UnityAction<int> PlayerSetLife;
    public static UnityAction<int> PlayerSetGrazeboxRadius;
    public static UnityAction<int> PlayerSetGraze;
    public static UnityAction<int> PlayerSetPower;
    public static UnityAction PlayerSetMiss;

    public static Player instance;

    [SerializeField]
    private PlayerData playerData = null;

    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;
    [SerializeField, HideInInspector]
    private CircleCollider2D circleCollider2D;
    [SerializeField, HideInInspector]
    private Animator animator;
    [SerializeField, HideInInspector]
    private BulletPool bulletPool;

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

    private int initialBomb = 3;

    private int framesLeftToDeathBomb = -1;

    private bool shouldMiss = false;
    private bool shouldCheckMovement = true;

    private void OnValidate()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        bulletPool = GetComponent<BulletPool>();
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

        if (null == playerData)
        {
            throw new System.Exception("No default PlayerData attached to Player");
        }
        // set player data/stats
        // TODO: retrieve player data from char select menu or something
        speed = playerData.UNFOCUSED_SPEED;
        focusSpeed = playerData.FOCUSED_SPEED;
        deathBombFrames = playerData.DEATH_BOMB_WINDOW;
        circleCollider2D.radius = (float)playerData.HITBOX_RADIUS / 100;
        RemainingLife = playerData.INITIAL_LIFE;
        initialBomb = playerData.INITIAL_BOMB; // TODO: also add from bomb's stat if implemented
        PlayerSetGrazeboxRadius(25);
        Focus = false;
        // change anims
        AnimatorOverrideController aoc = new(animator.runtimeAnimatorController);
        aoc["front"] = playerData.FRONT_ANIMATION;
        aoc["side"] = playerData.SIDE_ANIMATION;
        animator.runtimeAnimatorController = aoc;

        // TODO: register passive/ability or smth
        // playerData.Register(this);
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
            pos.x = (newX < STG.Constant.GAME_BORDER_LEFT) ?
                STG.Constant.GAME_BORDER_LEFT : (newX > STG.Constant.GAME_BORDER_RIGHT) ?
                    STG.Constant.GAME_BORDER_RIGHT : newX;
            pos.y = (newY > STG.Constant.GAME_BORDER_TOP) ?
                STG.Constant.GAME_BORDER_TOP : (newY < STG.Constant.GAME_BORDER_BOTTOM) ?
                    STG.Constant.GAME_BORDER_BOTTOM : newY;
            transform.position = pos;
            animator.Play(0 == moveDir.x ? "front" : "side");
            spriteRenderer.flipX = (moveDir.x > 0) || moveDir.x >= 0 && spriteRenderer.flipX;

            // shoot
            if (Input.GetButton("Shoot"))
            {
                PlayerShoot?.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Equals)) {
            Power += 32;
        }
        if (Input.GetKeyDown(KeyCode.Minus)) {
            Power -= 32;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerGetHit();
    }

    private void PlayerGetHit()
    {
        // any invuln check
        shouldMiss = true;
        circleCollider2D.enabled = false;
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
            // dispatch invulnerable timer
            PlayerIsInvulnerable.Invoke((float)__delay / duration);
            yield return Timing.WaitForOneFrame;
        }
        circleCollider2D.enabled = true;
        spriteRenderer.enabled = true;
    }

    IEnumerator<float> _DoRespawn()
    {
        animator.Play("front");
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

    public GameObject SpawnBulletP1(float x, float y, float speed, float angle, ShotData graphic, int delay)
    {
        GameObject bullet = instance.bulletPool.RequestBullet();
        bullet.transform.position = new Vector3(x, y, 0);
        bullet.transform.eulerAngles = new Vector3(0, 0, angle);
        bullet.TryGetComponent(out PlayerBullet playerBullet);
        playerBullet.speed = speed;
        playerBullet.SetGraphic(graphic);
        Timing.RunCoroutine(_SpawnBulletWithDelay(bullet, delay));
        return bullet;
    }

    private IEnumerator<float> _SpawnBulletWithDelay(GameObject bullet, int delay)
    {
        for (int __delay = 0; __delay < delay; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }

        bullet.SetActive(true);
    }
}
