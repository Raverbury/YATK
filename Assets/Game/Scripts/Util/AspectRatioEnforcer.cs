using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectRatioUtility : MonoBehaviour
{
    [SerializeField]
    private Vector2 aspectRatio = new(16f, 9f);

    void Start()
    {
        Adjust();
    }

    public void Adjust()
    {
        float targetaspect = aspectRatio.x / aspectRatio.y;

        float windowaspect = (float)Screen.width / (float)Screen.height;

        float scaleheight = windowaspect / targetaspect;

        if (TryGetComponent(out Camera camera))
        {
            if (scaleheight < 1.0f)
            {
                Rect rect = camera.rect;

                rect.width = 1.0f;
                rect.height = scaleheight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) / 2.0f;

                camera.rect = rect;
            }
            else
            {
                float scalewidth = 1.0f / scaleheight;

                Rect rect = camera.rect;

                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;

                camera.rect = rect;
            }
        }
    }
}