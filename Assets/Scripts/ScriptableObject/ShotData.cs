using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotData", menuName = "ScriptableObjects/ShotData", order = 0)]
public class ShotData : ScriptableObject
{
    [SerializeField]
    private List<Sprite> sprites;
    public List<Sprite> SPRITES
    {
        get
        {
            return sprites;
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