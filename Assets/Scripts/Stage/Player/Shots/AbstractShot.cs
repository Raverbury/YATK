using UnityEngine;

[RequireComponent(typeof(Player))]
public abstract class AbstractShot : PausableMono
{
    protected int currentPower = 0;
    protected Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
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

    public abstract void Shoot();
    public abstract void SetFocus(bool isFocused);

    public abstract void SetPower(int power);

    protected abstract void CalcShotInterval();
    protected abstract void CalcShotDamage();
}
