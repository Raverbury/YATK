using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
public class LoadingCanvas : MonoBehaviour
{
    [SerializeField]
    private TMP_Text loadingText;
    [SerializeField]
    private Canvas canvas;

    private int alphaFrames = 0;
    private const int ALPHA_FLUC_DURATION = 40;
    private int alphaChangeVel = 1;
    private bool shown = false;

    private const int SHOW_X = 0;
    private const int HIDE_X = 1000;

    public static UnityAction<bool> EVToggleLoadingCanvas;

    private void OnEnable()
    {
        EVToggleLoadingCanvas += ToggleLoadingCanvas;
    }

    private void OnDisable()
    {
        EVToggleLoadingCanvas += ToggleLoadingCanvas;
    }

    private void ToggleLoadingCanvas(bool show)
    {
        // canvas.enabled = show;

        shown = show;
    }

    private void Update()
    {
        PositionCanvas();
        if (shown)
        {
            FlashLoadingText();
        }
    }

    private void PositionCanvas()
    {
        if (shown)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(1, 1, 1), 0.1f);
        }
        else
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(0, 1, 1), 0.1f);
        }
    }

    private void FlashLoadingText()
    {
        loadingText.alpha = 0.2f + 0.8f * alphaFrames / ALPHA_FLUC_DURATION;
        alphaFrames += alphaChangeVel;
        if (alphaFrames >= ALPHA_FLUC_DURATION - 1)
        {
            alphaChangeVel = -1;
        }
        else if (alphaFrames <= 0)
        {
            alphaChangeVel = 1;
        }
    }
}