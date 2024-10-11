using Unity.Collections;
using Unity.Entities;

public struct EnemyBulletComponent : IComponentData
{
    public BitField32 bits;

    public const int GRAZE_BIT = 0;
    public const int CLEARABLE_BIT = 1;

    public bool HasGrazed() {
        return bits.IsSet(GRAZE_BIT);
    }

    public bool CanClear() {
        return bits.IsSet(CLEARABLE_BIT);
    }

    public void SetBits(bool hasGrazed, bool isClearable) {
        bits.SetBits(GRAZE_BIT, hasGrazed);
        bits.SetBits(CLEARABLE_BIT, isClearable);
    }
}
