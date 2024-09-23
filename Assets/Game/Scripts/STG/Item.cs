using System.Collections.Generic;
using STG;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class Item : PausableMono
{
    private bool shouldFlyToPlayer = false;

    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;

    private delegate void ItemCollect(Player player);
    private ItemCollect collectItemCallback;

    public float yVelocity = 1.5f;

    private void OnEnable()
    {
        Player.PlayerAutoCollectItem += OnPlayerAutoCollect;
    }

    private void OnDisable()
    {
        Player.PlayerAutoCollectItem -= OnPlayerAutoCollect;
    }

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        collectItemCallback = CollectPowerItem;
    }

    protected override void PausableUpdate()
    {
        if (shouldFlyToPlayer)
        {
            if (Player.instance != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, Player.instance.transform.position, 12f);
            }
        }
        else
        {
            Vector2 pos = transform.position;
            pos.y += yVelocity;
            transform.position = pos;
            yVelocity = Mathf.Max(-1.5f, yVelocity - 0.03f);
        }
    }

    public void SpawnItem(ItemType itemType, bool shouldAutoCollect = false, float initialYVelocity = 1.5f)
    {
        spriteRenderer.sprite = ShotSheet.GetItemSprite(itemType);
        collectItemCallback = itemType switch
        {
            ItemType.POWER_ITEM => CollectPowerItem,
            ItemType.POINT_ITEM => CollectPointItem,
            ItemType.BIG_POWER_ITEM => CollectBigPowerItem,
            ItemType.BOMB_ITEM => CollectBombItem,
            ItemType.LIFE_ITEM => CollectLifeItem,
            ItemType.FULL_POWER_ITEM => CollectFullPowerItem,
            _ => CollectTimeOrbItem,
        };
        shouldFlyToPlayer = shouldAutoCollect;
        yVelocity = initialYVelocity;
    }

    public void CollectItem(Player player)
    {
        collectItemCallback.Invoke(player);
    }

    private void OnPlayerAutoCollect()
    {
        shouldFlyToPlayer = true;
    }

    private void CollectPowerItem(Player player)
    {
        player.Power += 1;
        Player.PlayerCollectItem?.Invoke();
    }

    private void CollectPointItem(Player player)
    {
        Player.PlayerCollectItem?.Invoke();
    }

    private void CollectBigPowerItem(Player player)
    {
        player.Power += 10;
        Player.PlayerCollectItem?.Invoke();
    }

    private void CollectBombItem(Player player)
    {
    }

    private void CollectLifeItem(Player player)
    {
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
