using STG;
using Unity.Collections;
using Unity.Entities;

public struct ItemComponent : ISharedComponentData
{
    public BitField32 Data;
    public float Speed;

    public int GetItemType()
    {
        return (int)Data.GetBits(0, 30);
    }

    public bool ShouldAutoCollect()
    {
        return Data.IsSet(31);
    }

    public void SetShouldAutoCollect(bool shouldAutoCollect = true)
    {
        Data.SetBits(31, shouldAutoCollect);
    }

    public void SetNewItemType(ItemType itemType, bool shouldAutoCollect = false)
    {
        Data.Value = (uint)itemType;
        Data.SetBits(31, shouldAutoCollect);
    }
}