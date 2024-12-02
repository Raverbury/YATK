using UnityEngine;

public abstract class AbstractShot
{
    protected bool isFocused;

    public abstract AbstractShot Clone();
    public abstract void Tick(Player player);
    public abstract void Shoot();
    public abstract void SetPower(int power, Player player, GameObject weaponOrbPrefab);

    public virtual void SetFocus(bool isFocused)
    {
        this.isFocused = isFocused;
    }

    protected struct PositionPair
    {
        public Vector2 First { get; private set; }
        public Vector2 Second { get; private set; }

        public PositionPair(Vector2 first, Vector2 second)
        {
            First = first;
            Second = second;
        }
    }

    public abstract string ShotName();
    public abstract string ShotDescription();
    public abstract Color ShotColor();
}
