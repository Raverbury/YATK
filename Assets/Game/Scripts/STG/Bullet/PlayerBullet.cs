using UnityEngine;

public class PlayerBullet : Bullet
{
    public ushort damage = 1;

    public override void CrossCameraBoundary(Collider2D other)
    {
        // throw new System.NotImplementedException();
    }
}
