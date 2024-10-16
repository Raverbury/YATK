using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class MasterSparkBomb : AbstractBombWeapon
{
    [SerializeField]
    private GameObject bombZone;

    public override void Bomb(bool isDeathBomb)
    {
        Timing.RunCoroutine(_FireMasterSpark(isDeathBomb));
        Player.EVBombActivate?.Invoke(IFrameDuration(), isDeathBomb? 2 : 1);
        SFXPlayer.EVPlayMasterSparkSound?.Invoke();
    }

    private IEnumerator<float> _FireMasterSpark(bool isDeathBomb)
    {
        if (!isDeathBomb)
        {
            player.playerData.focusedSpeed.AddMultiplier("bomb", -0.5f);
            player.playerData.unfocusedSpeed.AddMultiplier("bomb", -0.5f);
            GameObject masterSparkLaser = Instantiate(bombZone, transform);
            masterSparkLaser.transform.localPosition = new(0f, 0.25f, 0f);
            masterSparkLaser.transform.localEulerAngles = new(0f, 0f, 90f);
            masterSparkLaser.transform.localScale = new(0f, 0.05f, 0f);
            if (masterSparkLaser.TryGetComponent(out Bomb bomb))
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

            Destroy(masterSparkLaser);

            // yield return WaitForFrames.WaitWrapper(60);
            player.playerData.focusedSpeed.RemoveMultiplier("bomb");
            player.playerData.unfocusedSpeed.RemoveMultiplier("bomb");
        }
        else
        {
            player.playerData.focusedSpeed.AddMultiplier("bomb", -0.5f);
            player.playerData.unfocusedSpeed.AddMultiplier("bomb", -0.5f);
            GameObject masterSparkLaserLeft = Instantiate(bombZone, transform);
            masterSparkLaserLeft.transform.localPosition = new(-0.15f, 0.25f, 0f);
            masterSparkLaserLeft.transform.localEulerAngles = new(0f, 0f, 95f);
            masterSparkLaserLeft.transform.localScale = new(0f, 0.05f, 0f);
            if (masterSparkLaserLeft.TryGetComponent(out Bomb bombLeft))
            {
                bombLeft.SetBombData(BombType.MASTER_SPARK_LASER_RAGE, 10f);
            }

            GameObject masterSparkLaserRight = Instantiate(bombZone, transform);
            masterSparkLaserRight.transform.localPosition = new(0.15f, 0.25f, 0f);
            masterSparkLaserRight.transform.localEulerAngles = new(0f, 0f, 85f);
            masterSparkLaserRight.transform.localScale = new(0f, 0.05f, 0f);
            if (masterSparkLaserRight.TryGetComponent(out Bomb bombRight))
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

            Destroy(masterSparkLaserLeft);
            Destroy(masterSparkLaserRight);

            // yield return WaitForFrames.WaitWrapper(60);
            player.playerData.focusedSpeed.RemoveMultiplier("bomb");
            player.playerData.unfocusedSpeed.RemoveMultiplier("bomb");
        }
    }

    public override int IFrameDuration()
    {
        return 270;
    }

    protected override void PausableUpdate()
    {

    }
}