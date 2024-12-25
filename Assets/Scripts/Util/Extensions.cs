
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class Extensions
    {
        public static string Repeat(this string str, int times)
        {
            string result = "";
            for (int i = 0; i < times; i++)
            {
                result += str;
            }
            return result;
        }

        public static void SetSpeed(this Entity entity, float speed)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            try
            {
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
                bulletComponent.Speed = speed;
                entityManager.SetComponentData(entity, bulletComponent);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void SetFacing(this Entity entity, float angleDegrees)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            try
            {
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
                bulletTransform.Rotation = quaternion.EulerXYZ(new float3(0f, 0f, math.radians(angleDegrees)));
                entityManager.SetComponentData(entity, bulletTransform);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static float AngleTo(this Vector3 from, Vector3 to)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(
                to.y - from.y,
                to.x - from.x
            );
        }

        public static Vector3 RotateBy(this Vector3 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            return new Vector3(
                v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
                v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
            );
        }

        public static Vector3 Position(this Entity entity)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            try
            {
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
                return new Vector3(bulletTransform.Position.x, bulletTransform.Position.y);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
        }
    }
}