using STG;
using UnityEngine;

public class ItemPool : ObjectPool
{
    private static ItemPool instance = null;

    protected override void AltAwake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    public static void SpawnItemI1(GameObject gameObject, ItemType itemType, bool shouldAutoCollect = false, float initialYVelocity = 1.5f)
    {
        SpawnItemI1(gameObject.transform.position.x, gameObject.transform.position.y, itemType, shouldAutoCollect, initialYVelocity);
    }

    public static void SpawnItemI1(Vector2 pos, ItemType itemType, bool shouldAutoCollect = false, float initialYVelocity = 1.5f)
    {
        SpawnItemI1(pos.x, pos.y, itemType, shouldAutoCollect, initialYVelocity);
    }

    public static void SpawnItemI1(float x, float y, ItemType itemType, bool shouldAutoCollect = false, float initialYVelocity = 1.5f)
    {
        GameObject itemObject = instance.RequestObject();
        itemObject.transform.position = new Vector2(x, y);
        if (itemObject.TryGetComponent(out Item item))
        {
            item.SpawnItem(itemType, shouldAutoCollect, initialYVelocity);
        }
        itemObject.SetActive(true);
    }
}