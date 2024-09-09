using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Grazebox : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private CircleCollider2D circleCollider;
    [SerializeField, HideInInspector]
    private Player player;

    private void OnValidate()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        player = GetComponentInParent<Player>();
    }

    private void OnEnable()
    {
        Player.PlayerSetGrazeboxRadius += SetGrazeboxRadius;
    }

    private void OnDisable()
    {
        Player.PlayerSetGrazeboxRadius -= SetGrazeboxRadius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyBullet bullet)) {
            if (!bullet.HasGrazed) {
                player.PlayerGraze();
                bullet.Graze();
            }
        }
    }

    private void SetGrazeboxRadius(int pixels)
    {
        circleCollider.radius = (float)pixels / 100;
    }
}
