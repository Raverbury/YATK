using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotData", menuName = "ScriptableObjects/ShotData", order = 0)]
public class ShotData : ScriptableObject
{
    [SerializeField]
    private Sprite _sprite;
    public Sprite sprite
    {
        get
        {
            return _sprite;
        }
    }
    [SerializeField]
    private float _size;
    public float size
    {
        get
        {
            return _size;
        }
    }
    [SerializeField]
    private float _hitboxRadius;
    public float hitboxRadius
    {
        get
        {
            return _hitboxRadius;
        }
    }
}