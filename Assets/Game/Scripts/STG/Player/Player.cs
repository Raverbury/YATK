using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    public static UnityAction<bool> PlayerChangeFocus;
    public static UnityAction PlayerShoot;
    public static UnityAction PlayerBomb;
    public static UnityAction<float> PlayerIsInvulnerable;
    public static UnityAction<int> PlayerSetLife;

    public static Player instance;

    [SerializeField]
    private PlayerData playerData = null;

    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;
    [SerializeField, HideInInspector]
    private CircleCollider2D circleCollider2D;
    [SerializeField, HideInInspector]
    private Animator animator;

    private float speed = 4f;
    private float focusSpeed = 2f;
    private int deathBombFrames = 10;

    private Vector2 moveDir = Vector2.zero;

    private int _remainingLife = 2;
    public int remainingLife
    {
        get
        {
            return _remainingLife;
        }
        set
        {
            PlayerSetLife.Invoke(value);
            _remainingLife = value;
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
    }

    // Start is called before the first frame update
    void Awake()
    {
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
        remainingLife = playerData.INITIAL_LIFE;
        initialBomb = playerData.INITIAL_BOMB; // also add from bomb's stat if implemented
        AnimatorOverrideController aoc = new(animator.runtimeAnimatorController);
        aoc["front"] = playerData.FRONT_ANIMATION;
        aoc["side"] = playerData.SIDE_ANIMATION;
        animator.runtimeAnimatorController = aoc;
        // playerData.Register(this);

        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
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
            animator.Play(0 == moveDir.x ? "front" : "side");
            spriteRenderer.flipX = (moveDir.x > 0) || moveDir.x >= 0 && spriteRenderer.flipX;

            // dispatch focus
            PlayerChangeFocus?.Invoke(Input.GetButton("Focus"));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (STG.Constant.LAYER_ENEMY_BULLET == other.gameObject.layer || STG.Constant.LAYER_ENEMY == other.gameObject.layer)
        {
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
        remainingLife -= 1;
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
}
