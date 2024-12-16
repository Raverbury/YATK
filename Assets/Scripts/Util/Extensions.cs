
using System;
using Unity.Entities;
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

        public static float AngleTo(this Vector2 from, Vector2 to)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(
                to.y - from.y,
                to.x - from.x
            );
        }
    }
}