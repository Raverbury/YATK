using UnityEngine;

public class PlayerBullet : Bullet
{
    public float damage = 1f;

    public override void CrossCameraBoundary(Collider2D other)
    {
        // throw new System.NotImplementedException();
    }
}
