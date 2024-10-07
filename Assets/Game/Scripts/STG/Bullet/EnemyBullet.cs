using UnityEngine;

public class EnemyBullet : Bullet
{
    private ushort flags;
    public bool HasGrazed
    {
        private set
        {
            ushort bit = STG.Constant.ENEMY_BULLET_GRAZED_BIT;
            flags = (ushort)(value ? (flags | 1 << bit) : (flags & ~(1 << bit)));
        }
        get
        {
            ushort bit = STG.Constant.ENEMY_BULLET_GRAZED_BIT;
            return 1 == (flags & (1 << bit)) >> bit;
        }
    }
    private bool Deletable
    {
        set
        {
            ushort bit = STG.Constant.ENEMY_BULLET_DELETABLE_BIT;
            flags = (ushort)(value ? (flags | 1 << bit) : (flags & ~(1 << bit)));
        }
        get
        {
            ushort bit = STG.Constant.ENEMY_BULLET_DELETABLE_BIT;
            return 1 == (flags & (1 << bit)) >> bit;
        }
    }

    private void OnEnable()
    {
        HasGrazed = false;
        StageManager.ClearBullet += ClearBullet;
    }

    private void OnDisable()
    {
        StageManager.ClearBullet -= ClearBullet;
    }

    public void ClearBullet(bool shouldDropStarItem, bool forceClear = false)
    {
        if (!Deletable && !forceClear) {
            return;
        }
        gameObject.SetActive(false);
        if (shouldDropStarItem)
        {
            ItemPool.SpawnItemI1(gameObject, STG.ItemType.STAR_ITEM, true);
        }
    }

    public void Graze()
    {
        HasGrazed = true;
    }

    public void SetClearable(bool clearable) {
        Deletable = clearable;
    }

    public override void CrossPlayableBoundary(Collider2D other)
    {
        ClearBullet(false);
    }

    public override void CrossCameraBoundary(Collider2D other)
    {
        // throw new System.NotImplementedException();
    }
}
