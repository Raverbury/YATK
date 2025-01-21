using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class FantasyOrbBomb : AbstractBombWeapon
{
    const int ORBS_PER_RING = 5;
    const float ORB_ORBIT_RADIUS = 70f;
    const int ORB_EXPAND_DURATION = 45;
    const int ORB_ORBIT_DURATION = 160;
    const int ORB_HOME_DURATION = 120;
    const int TOTAL_ORB_DURATION = ORB_EXPAND_DURATION + ORB_ORBIT_DURATION + ORB_HOME_DURATION + ORB_EXPLOSION_DURATION;
    const float RADIUS_STEP = ORB_ORBIT_RADIUS / ORB_EXPAND_DURATION;

    const float COLLIDER_RADIUS = 0.4f;

    const float ORB_DAMAGE = 1f;
    const float ORB_EXPLOSION_DAMAGE = 200f;
    const int ORB_EXPLOSION_DURATION = 15;

    public override void Bomb(Player player, bool isDeathBomb, BombPrefabs bombPrefabs)
    {
        Timing.RunCoroutine(_Bomb(player, isDeathBomb, bombPrefabs));
        SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_GUN00, 0.6f);
    }

    private IEnumerator<float> _Bomb(Player player, bool isDeathBomb, BombPrefabs bombPrefabs)
    {
        float rotation = 0f;
        float rotateSpeed = 3f;
        List<GameObject> orbs1 = new();
        List<GameObject> orbs2 = new();
        for (int i = 0; i < ORBS_PER_RING; i++)
        {
            GameObject orb = GameObject.Instantiate(bombPrefabs.CircularFDA);
            orb.transform.eulerAngles = Vector3.zero;
            orb.transform.localScale = new(100f, 100f, 100f);
            if (orb.TryGetComponent(out FriendlyDamageArea bomb))
            {
                bomb.SetBombData(BombType.FANTASY_ORB, ORB_DAMAGE);
            }
            if (orb.TryGetComponent(out CircleCollider2D collider))
            {
                collider.radius = COLLIDER_RADIUS;
            }
            GameObject orb2 = GameObject.Instantiate(orb);
            orbs1.Add(orb);
            orbs2.Add(orb2);
        }

        float orbRadius = 0f;
        Vector3 orbitPos = player.transform.position;
        for (int framesElapsed = 0; framesElapsed < TOTAL_ORB_DURATION; framesElapsed++)
        {
            if (framesElapsed < ORB_EXPAND_DURATION)
            {
                orbRadius = RADIUS_STEP * framesElapsed;
            }
            if (framesElapsed < ORB_EXPAND_DURATION + ORB_ORBIT_DURATION)
            {
                orbitPos = player.transform.position;

                OrbitOrbs(orbs1, orbitPos, orbRadius * (1.1f + 0.3f * Mathf.Cos(framesElapsed / 20f)), rotation, 0f);
                OrbitOrbs(orbs2, orbitPos, orbRadius * (1.1f + 0.3f * Mathf.Sin(framesElapsed / 20f)), -rotation, 0f);
            }
            else
            {
                if (framesElapsed < ORB_EXPAND_DURATION + ORB_ORBIT_DURATION + 0.7f * ORB_HOME_DURATION)
                {
                    orbRadius *= 0.98f;
                }
                orbitPos = GetTargetPositionToOrbit(orbitPos, player);
                OrbitOrbs(orbs1, orbitPos, orbRadius, rotation, 0f);
                OrbitOrbs(orbs2, orbitPos, orbRadius, -rotation, 0f);
            }
            if (framesElapsed == ORB_EXPAND_DURATION + ORB_ORBIT_DURATION + ORB_HOME_DURATION)
            {
                SFXPlayer.RequestPlaySound?.Invoke(RuntimeGameData.Registry.SFX_TAN00, 1f);
                orbs1.ForEach(orb =>
                {
                    if (orb.TryGetComponent(out FriendlyDamageArea bombArea))
                    {
                        bombArea.SetBombData(BombType.FANTASY_ORB, ORB_EXPLOSION_DAMAGE / ORB_EXPLOSION_DURATION);
                    }
                });
                orbs2.ForEach(orb =>
                {
                    if (orb.TryGetComponent(out FriendlyDamageArea bombArea))
                    {
                        bombArea.SetBombData(BombType.FANTASY_ORB, ORB_EXPLOSION_DAMAGE / ORB_EXPLOSION_DURATION);
                    }
                });
            }
            rotation = (rotation + rotateSpeed) % 360f;
            rotateSpeed = Mathf.Min(9f, rotateSpeed * 1.015f);
            yield return Timing.WaitForOneFrame;
        }

        orbs1.ForEach(orb => GameObject.Destroy(orb));
        orbs2.ForEach(orb => GameObject.Destroy(orb));
    }

    private static void OrbitOrbs(List<GameObject> orbs, Vector3 orbitTarget, float radius, float rotation, float randomRange)
    {
        int count = orbs.Count;
        float spread = 360f / count;
        float lowerBound = 1f - randomRange;
        float upperBound = 1f + randomRange;
        for (int i = 0; i < count; i++)
        {
            orbs[i].transform.position = orbitTarget + new Vector3(
                radius * Random.Range(lowerBound, upperBound) * Mathf.Cos(Mathf.Deg2Rad * (rotation + spread * i) * Random.Range(lowerBound, upperBound)),
                radius * Random.Range(lowerBound, upperBound) * Mathf.Sin(Mathf.Deg2Rad * (rotation + spread * i) * Random.Range(lowerBound, upperBound)),
                0f
            );
        }
    }

    private static Vector3 GetTargetPositionToOrbit(Vector3 currentOrbitPos, Player fallback)
    {
        GameObject enemy = StageManager.instance.GetFirstEnemy();
        Vector3 targetPos = fallback.transform.position;
        if (enemy)
        {
            targetPos = enemy.transform.position;
        }
        return Vector3.MoveTowards(currentOrbitPos, targetPos, STG.Constant.GAME_HEIGHT / (0.5f * ORB_HOME_DURATION));
    }

    public override int IFrameDuration()
    {
        return TOTAL_ORB_DURATION + 20;
    }

    public override AbstractBombWeapon Clone()
    {
        return new FantasyOrbBomb();
    }

    public override string BombName()
    {
        return "Fantasy Orb";
    }

    public override string BombDescription()
    {
        return "Orbs that orbit player before homing in on an enemy";
    }

    public override Color BombColor()
    {
        return new Color(255 / 255, 51f / 255f, 51f / 255);
    }
}