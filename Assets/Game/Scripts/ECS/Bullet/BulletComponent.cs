using Unity.Entities;

public struct BulletComponent : IComponentData
{
    public float Speed;
    public int FramesToLive;
}
