using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class MasterSparkBomb : AbstractBombWeapon
{
    public override void Bomb(Player player, bool isDeathBomb, BombPrefabs bombPrefabs)
    {
        Timing.RunCoroutine(_FireMasterSpark(player, isDeathBomb, bombPrefabs));
        SFXPlayer.EVPlayMasterSparkSound?.Invoke();
    }

    private IEnumerator<float> _FireMasterSpark(Player player, bool isDeathBomb, BombPrefabs bombPrefabs)
    {
        if (!isDeathBomb)
        {
            player.playerData.focusedSpeed.AddMultiplier("bomb", -0.5f);
            player.playerData.unfocusedSpeed.AddMultiplier("bomb", -0.5f);
            GameObject masterSparkLaser = GameObject.Instantiate(bombPrefabs.PolygonalFDA, player.transform);
            masterSparkLaser.transform.localPosition = new(0f, 0.25f, 0f);
            masterSparkLaser.transform.localEulerAngles = new(0f, 0f, 90f);
            masterSparkLaser.transform.localScale = new(0f, 0.05f, 0f);
            if (masterSparkLaser.TryGetComponent(out FriendlyDamageArea bomb))
            {
                bomb.SetBombData(BombType.MASTER_SPARK_LASER, 10f);
            }

            const int LASER_SHOOT_DURATION = 15;
            const float X_GROWTH = 2f / LASER_SHOOT_DURATION;
            for (int i = 0; i < LASER_SHOOT_DURATION; i++)
            {
                masterSparkLaser.transform.localScale += new Vector3(X_GROWTH, 0f, 0f);
                yield return Timing.WaitForOneFrame;
            }

            const int LASER_EXPAND_DURATION = 10;
            const float Y_GROWTH = 1.95f / LASER_EXPAND_DURATION;
            for (int i = 0; i < LASER_EXPAND_DURATION; i++)
            {
                masterSparkLaser.transform.localScale += new Vector3(0f, Y_GROWTH, 0f);
                yield return Timing.WaitForOneFrame;
            }

            yield return WaitForFrames.WaitWrapper(IFrameDuration() - LASER_SHOOT_DURATION - 2 * LASER_EXPAND_DURATION);

            const float Y_SHRINK = 2f / LASER_EXPAND_DURATION;
            for (int i = 0; i < LASER_EXPAND_DURATION; i++)
            {
                masterSparkLaser.transform.localScale -= new Vector3(0f, Y_SHRINK, 0f);
                yield return Timing.WaitForOneFrame;
            }

            GameObject.Destroy(masterSparkLaser);

            // yield return WaitForFrames.WaitWrapper(60);
            player.playerData.focusedSpeed.RemoveMultiplier("bomb");
            player.playerData.unfocusedSpeed.RemoveMultiplier("bomb");
        }
        else
        {
            player.playerData.focusedSpeed.AddMultiplier("bomb", -0.5f);
            player.playerData.unfocusedSpeed.AddMultiplier("bomb", -0.5f);
            GameObject masterSparkLaserLeft = GameObject.Instantiate(bombPrefabs.PolygonalFDA, player.transform);
            masterSparkLaserLeft.transform.localPosition = new(-0.15f, 0.25f, 0f);
            masterSparkLaserLeft.transform.localEulerAngles = new(0f, 0f, 95f);
            masterSparkLaserLeft.transform.localScale = new(0f, 0.05f, 0f);
            if (masterSparkLaserLeft.TryGetComponent(out FriendlyDamageArea bombLeft))
            {
                bombLeft.SetBombData(BombType.MASTER_SPARK_LASER_RAGE, 10f);
            }

            GameObject masterSparkLaserRight = GameObject.Instantiate(bombPrefabs.PolygonalFDA, player.transform);
            masterSparkLaserRight.transform.localPosition = new(0.15f, 0.25f, 0f);
            masterSparkLaserRight.transform.localEulerAngles = new(0f, 0f, 85f);
            masterSparkLaserRight.transform.localScale = new(0f, 0.05f, 0f);
            if (masterSparkLaserRight.TryGetComponent(out FriendlyDamageArea bombRight))
            {
                bombRight.SetBombData(BombType.MASTER_SPARK_LASER_RAGE, 10f);
            }

            const int LASER_SHOOT_DURATION = 15;
            const float X_GROWTH = 2f / LASER_SHOOT_DURATION;
            for (int i = 0; i < LASER_SHOOT_DURATION; i++)
            {
                masterSparkLaserLeft.transform.localScale += new Vector3(X_GROWTH, 0f, 0f);
                masterSparkLaserRight.transform.localScale += new Vector3(X_GROWTH, 0f, 0f);
                yield return Timing.WaitForOneFrame;
            }

            const int LASER_EXPAND_DURATION = 10;
            const float Y_GROWTH = 1.95f / LASER_EXPAND_DURATION;
            for (int i = 0; i < LASER_EXPAND_DURATION; i++)
            {
                masterSparkLaserLeft.transform.localScale += new Vector3(0f, Y_GROWTH, 0f);
                masterSparkLaserRight.transform.localScale += new Vector3(0f, Y_GROWTH, 0f);
                yield return Timing.WaitForOneFrame;
            }

            yield return WaitForFrames.WaitWrapper(IFrameDuration() - LASER_SHOOT_DURATION - 2 * LASER_EXPAND_DURATION);

            const float Y_SHRINK = 2f / LASER_EXPAND_DURATION;
            for (int i = 0; i < LASER_EXPAND_DURATION; i++)
            {
                masterSparkLaserLeft.transform.localScale -= new Vector3(0f, Y_SHRINK, 0f);
                masterSparkLaserRight.transform.localScale -= new Vector3(0f, Y_SHRINK, 0f);
                yield return Timing.WaitForOneFrame;
            }

            GameObject.Destroy(masterSparkLaserLeft);
            GameObject.Destroy(masterSparkLaserRight);

            // yield return WaitForFrames.WaitWrapper(60);
            player.playerData.focusedSpeed.RemoveMultiplier("bomb");
            player.playerData.unfocusedSpeed.RemoveMultiplier("bomb");
        }
    }

    public override int IFrameDuration()
    {
        return 270;
    }

    public override AbstractBombWeapon Clone()
    {
        return new MasterSparkBomb();
    }

    public override string BombName()
    {
        return "Master Spark";
    }

    public override string BombDescription()
    {
        return "A giant laser short of anything but power";
    }

    public override Color BombColor()
    {
        return new Color(255f / 255, 204f / 255f, 0f);
    }
}