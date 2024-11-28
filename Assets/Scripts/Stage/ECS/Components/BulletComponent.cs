using Unity.Entities;

public struct BulletComponent : IComponentData
{
    public float Speed;
    public bool ShouldMove;
}
