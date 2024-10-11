using Unity.Entities;

public struct EntitySpawnerComponent : IComponentData {
    public Entity BulletPrefab;
    public int AmountOfEnemyBulletsToAdd;
    public int AmountOfPlayerBulletsToAdd;
    public int AmountOfItemsToAdd;
    public bool HasInitialized;
    public bool DoMath;
}