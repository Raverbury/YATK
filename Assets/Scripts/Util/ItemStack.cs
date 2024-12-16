using STG;

public struct ItemStack
{
    public ItemStack(ItemType itemType, uint count)
    {
        ItemType = itemType;
        Count = count;
    }

    public ItemType ItemType;
    public uint Count;
}