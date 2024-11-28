using UnityEngine;

[RequireComponent(typeof(Canvas))]
public abstract class AbstractHomeSelector : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Canvas canvas;
    public Canvas Canvas
    {
        get
        {
            return canvas;
        }
    }

    public RectTransform RectTransform
    {
        get
        {
            return (RectTransform)transform;
        }
    }

    protected virtual void OnValidate()
    {
        canvas = GetComponent<Canvas>();
    }
}