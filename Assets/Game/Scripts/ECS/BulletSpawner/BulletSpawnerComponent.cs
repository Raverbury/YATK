using Unity.Entities;

public struct BulletSpawnerComponent : IComponentData {
    public Entity BulletPrefab;
    public int TimeBetweenShot;
    public int ShotInterval;
    public int Branches;
}