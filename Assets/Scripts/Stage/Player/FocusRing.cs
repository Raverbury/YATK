using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(AutoRotate))]
public class FocusRing : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Player.PlayerSetFocus += SetFocusVisible;
    }

    private void OnDisable()
    {
        Player.PlayerSetFocus -= SetFocusVisible;
    }

    private void SetFocusVisible(bool enable)
    {
        spriteRenderer.enabled = enable;
    }
}
