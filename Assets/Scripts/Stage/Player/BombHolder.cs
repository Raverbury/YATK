using UnityEngine;

[RequireComponent(typeof(Player))]
public class BombHolder : PausableMono
{
    private AbstractBombWeapon bomb;

    public GameObject polygonalFDA;
    public GameObject circularFDA;

    private AbstractBombWeapon.BombPrefabs bombPrefabs;

    private void Awake()
    {
        bomb = RuntimeGameData.SelectedBomb.Clone();
        bombPrefabs = new(polygonalFDA, circularFDA);
    }

    private void OnEnable()
    {
        Player.PlayerUseBomb += UseBomb;
    }

    private void OnDisable()
    {
        Player.PlayerUseBomb -= UseBomb;
    }

    private void UseBomb(bool isDeathBomb)
    {
        Player player = GetComponent<Player>();
        Player.EVBombActivate?.Invoke(bomb.IFrameDuration(), isDeathBomb ? bomb.BombConsumedOnDeathBomb() : bomb.BombConsumedOnNormalBomb());
        SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_SPELL_START, 1f);
        bomb.Bomb(player, isDeathBomb, bombPrefabs);
    }

    protected override void PausableUpdate()
    {
        // TODO: possibly bomb tick here feature, idk why you would ever need that but it's there just in case?
    }
}