public abstract class AbstractBombWeapon : PausableMono
{
    protected Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        Player.PlayerBomb += Bomb;
    }

    private void OnDisable()
    {
        Player.PlayerBomb -= Bomb;
    }

    public abstract void Bomb();
    public abstract int IFrameDuration();
}