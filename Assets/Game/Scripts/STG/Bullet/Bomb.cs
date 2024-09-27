using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class Bomb : MonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (STG.Constant.LAYER_ENEMY_BULLET == other.gameObject.layer)
        {
            if (other.TryGetComponent(out EnemyBullet enemyBullet))
            {
                enemyBullet.ClearBullet(true);
            }
        }
    }

    private void Update() {
        Player.PlayerAutoCollectItem?.Invoke();
    }
}