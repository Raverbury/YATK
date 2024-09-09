using System.Collections.Generic;
using MEC;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField]
    private GameObject baseObj;
    [SerializeField]
    private int capacity;

    private List<GameObject> pool;
    private int currentIndex = 0;

    private void Awake()
    {
        pool = new List<GameObject>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            GameObject clone = Object.Instantiate(baseObj);
            clone.SetActive(false);
            pool.Add(clone);
        }
    }

    public GameObject RequestBullet()
    {
        return pool[GetNextInteger()];
    }

    private int GetNextInteger()
    {
        currentIndex = ((currentIndex + 1) >= capacity) ? 0 : (currentIndex + 1);
        return currentIndex;
    }
}
