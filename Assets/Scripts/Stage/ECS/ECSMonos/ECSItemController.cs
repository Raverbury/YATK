using STG;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ECSItemController : PausableMono
{
    private EntityManager entityManager;
    private EntityQuery activeItemsQueryR;
    private EntityQuery activeItemsQueryRW;

    private void OnEnable()
    {
        Player.PlayerAutoCollectItem += OnPlayerAutoCollect;
    }

    private void OnDisable()
    {
        Player.PlayerAutoCollectItem -= OnPlayerAutoCollect;
    }

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        activeItemsQueryR = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ItemComponent>());
        activeItemsQueryRW = entityManager.CreateEntityQuery(ComponentType.ReadWrite<ItemComponent>());
    }

    protected override void PausableUpdate()
    {
        if (Player.instance == null)
        {
            return;
        }
        Player player = Player.instance;
        float3 playerPos = Player.instance.transform.position;
        NativeArray<Entity> allActiveItems = activeItemsQueryR.ToEntityArray(Allocator.Temp);
        if (allActiveItems.Length == 0)
        {
            allActiveItems.Dispose();
            return;
        }
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        for (int i = 0; i < allActiveItems.Length; i++)
        {
            Entity entity = allActiveItems[i];
            ItemComponent itemComponent = entityManager.GetSharedComponent<ItemComponent>(entity);

            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

            float distance = math.distance(localTransform.Position, playerPos);
            if (distance <= 30f)
            {
                CollectItem(player, (ItemType)itemComponent.GetItemType());
                ECSEntitySpawner.DespawnEntity(entity);
            }

            if (itemComponent.ShouldAutoCollect())
            {
                var newPos = Vector2.MoveTowards(
                    new Vector2(localTransform.Position.x, localTransform.Position.y),
                    Player.instance.transform.position,
                    12f
                );
                localTransform.Position = new float3(newPos.x, newPos.y, 0f);
            }
            else
            {
                localTransform.Position.y += itemComponent.Speed;
                itemComponent.Speed = math.max(-1.5f, itemComponent.Speed - 0.03f);
                entityManager.SetSharedComponent(entity, itemComponent);
                if (localTransform.Position.y < Constant.GAME_BORDER_BOTTOM - 100f) {
                    ECSEntitySpawner.DespawnEntity(entity);
                }
            }
            ecb.SetComponent(entity, localTransform);
        }
        ecb.Playback(entityManager);
        ecb.Dispose();
        allActiveItems.Dispose();
    }

    private void CollectItem(Player player, ItemType itemType)
    {
        Player.PlayerCollectItem?.Invoke();
        switch (itemType)
        {
            case ItemType.POWER_ITEM:
                CollectPowerItem(player);
                break;
            case ItemType.POINT_ITEM:
                CollectPointItem(player);
                break;
            case ItemType.BIG_POWER_ITEM:
                CollectBigPowerItem(player);
                break;
            case ItemType.BOMB_ITEM:
                CollectBombItem(player);
                break;
            case ItemType.LIFE_ITEM:
                CollectLifeItem(player);
                break;
            case ItemType.FULL_POWER_ITEM:
                CollectFullPowerItem(player);
                break;
            default:
                CollectTimeOrbItem(player);
                break;
        }
    }

    private void OnPlayerAutoCollect()
    {
        NativeArray<Entity> allActiveItems = activeItemsQueryRW.ToEntityArray(Allocator.Temp);
        for (int i = 0; i < allActiveItems.Length; i++)
        {
            Entity entity = allActiveItems[i];
            ItemComponent itemComponent = entityManager.GetSharedComponent<ItemComponent>(entity);
            itemComponent.SetShouldAutoCollect();
            entityManager.SetSharedComponent(entity, itemComponent);
        }
        allActiveItems.Dispose();
    }

    private void CollectPowerItem(Player player)
    {
        player.Power += 1;
        SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_ITEM_0, 1f);
    }

    private void CollectPointItem(Player player)
    {
        // TODO: point?
        SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_ITEM_0, 1f);
    }

    private void CollectBigPowerItem(Player player)
    {
        player.Power += 10;
        SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_ITEM_0, 1f);
    }

    private void CollectBombItem(Player player)
    {
        player.RemainingBomb += 1;
        // TODO: change bomb collect sfx?
        SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_ITEM_0, 1f);
    }

    private void CollectLifeItem(Player player)
    {
        player.PlayerExtend();
    }

    private void CollectFullPowerItem(Player player)
    {

    }

    private void CollectStarItem(Player player)
    {

    }

    private void CollectTimeOrbItem(Player player)
    {

    }
}