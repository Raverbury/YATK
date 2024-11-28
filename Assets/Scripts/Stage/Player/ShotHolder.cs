using UnityEngine;

[RequireComponent(typeof(Player))]
public class ShotHolder : PausableMono
{
    [SerializeField, HideInInspector]
    private Player player;
    private AbstractShot shot;

    public GameObject weaponOrbPrefab;

    private void OnValidate() {
        player = GetComponent<Player>();
    }

    private void Awake()
    {
        shot = RuntimeGameData.SelectedShot.Clone();
    }

    private void OnEnable()
    {
        Player.PlayerShoot += Shoot;
        Player.PlayerSetFocus += SetFocus;
        Player.PlayerSetPower += SetPower;
    }

    private void OnDisable()
    {
        Player.PlayerShoot -= Shoot;
        Player.PlayerSetFocus -= SetFocus;
        Player.PlayerSetPower -= SetPower;
    }

    private void Shoot()
    {
        shot.Shoot();
    }

    private void SetFocus(bool isFocused)
    {
        shot.SetFocus(isFocused);
    }

    private void SetPower(int power)
    {
        shot.SetPower(power, player, weaponOrbPrefab);
    }

    protected override void PausableUpdate()
    {
        shot.Tick(player);
    }
}