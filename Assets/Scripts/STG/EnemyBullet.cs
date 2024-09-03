using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
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
        if (STG.Constant.LAYER_PLAYABLE_AREA == other.gameObject.layer)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetGraphic(ShotData graphic)
    {
        spriteRenderer.sprite = graphic.sprite;
        gameObject.transform.localScale = 1f / graphic.hitboxRadius * graphic.size * Vector3.one;
        circleCollider2D.radius = graphic.hitboxRadius;
    }
}
