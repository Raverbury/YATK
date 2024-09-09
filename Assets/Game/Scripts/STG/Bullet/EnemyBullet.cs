using UnityEngine;
using UnityEngine.Animations;

public class EnemyBullet : Bullet
{
    private int flags;
    public bool HasGrazed
    {
        private set
        {
            int bit = STG.Constant.ENEMY_BULLET_GRAZED_BIT;
            flags = value ? (flags | 1 << bit) : (flags & ~(1 << bit));
        }
        get
        {
            int bit = STG.Constant.ENEMY_BULLET_GRAZED_BIT;
            return 1 == (flags & (1 << bit)) >> bit;
        }
    }
    private bool Deletable
    {
        set
        {
            int bit = STG.Constant.ENEMY_BULLET_DELETABLE_BIT;
            flags = value ? (flags | 1 << bit) : (flags & ~(1 << bit));
        }
        get
        {
            int bit = STG.Constant.ENEMY_BULLET_DELETABLE_BIT;
            return 1 == (flags & (1 << bit)) >> bit;
        }
    }

    private void OnEnable()
    {
        flags = 0;
    }

    public void Graze()
    {
        HasGrazed = true;
    }

    public override void CrossCameraBoundary(Collider2D other)
    {
        // throw new System.NotImplementedException();
    }
}
