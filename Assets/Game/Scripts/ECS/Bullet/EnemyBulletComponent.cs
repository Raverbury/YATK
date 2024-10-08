using Unity.Entities;

public struct EnemyBulletComponent : IComponentData
{
    public int Flags;

    public readonly bool GetHasGrazed()
    {
        return 1 == (Flags & (1 << 1)) >> 1;
    }

    public void SetHasGrazed(bool value)
    {
        Flags = (ushort)(value ? (Flags | 1 << 1) : (Flags & ~(1 << 1)));
    }
}
