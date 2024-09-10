using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D))]
public abstract class Bullet : MonoBehaviour
{
    /// <summary>
    /// The speed at which the object moves each frame
    /// </summary>
    public float speed;

    /// <summary>
    /// The speed at which the object rotates each frame (+ for counter-clockwise, - for clockwise)
    /// </summary>
    public float rotationSpeed;

    /// <summary>
    /// The rate at which the bullet accelerates each frame
    /// </summary>
    public float acceleration;


    public float speedCap;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider2D;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * speed);
        transform.eulerAngles += Vector3.forward * rotationSpeed;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (STG.Constant.LAYER_SCREEN_EDGE == other.gameObject.layer)
        {
            CrossCameraBoundary(other);
        }
        if (STG.Constant.LAYER_PLAYABLE_AREA == other.gameObject.layer)
        {
            CrossPlayableBoundary(other);
        }
    }

    public abstract void CrossCameraBoundary(Collider2D other);
    public virtual void CrossPlayableBoundary(Collider2D other)
    {
        gameObject.SetActive(false);
    }

    public void SetGraphic(Sprite sprite, float size, float hitboxRadius)
    {
        spriteRenderer.sprite = sprite;
        gameObject.transform.localScale = 12.5f * size * Vector3.one;
        circleCollider2D.radius = hitboxRadius;
    }
}
