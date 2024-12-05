using UnityEngine;

public abstract class AbstractBombWeapon
{
    public abstract AbstractBombWeapon Clone();
    public abstract void Bomb(Player player, bool isDeathBomb, BombPrefabs bombPrefabs);
    public abstract int IFrameDuration();
    public virtual int BombConsumedOnNormalBomb()
    {
        return 1;
    }

    public virtual int BombConsumedOnDeathBomb()
    {
        return 2;
    }

    public struct BombPrefabs
    {
        public GameObject PolygonalFDA { get; private set; }
        public GameObject CircularFDA { get; private set; }

        public BombPrefabs(GameObject polygonalFDA, GameObject circularFDA)
        {
            PolygonalFDA = polygonalFDA;
            CircularFDA = circularFDA;
        }
    }

    public abstract string BombName();
    public abstract string BombDescription();
    public abstract Color BombColor();
}