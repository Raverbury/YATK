using Unity.Entities;

public struct BulletSpawnerComponent : IComponentData {
    public Entity BulletPrefab;
    public int AmountToAdd;
    public bool HasInitialized;
    public bool DoMath;
}