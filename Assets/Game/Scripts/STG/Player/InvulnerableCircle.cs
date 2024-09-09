using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(AutoRotate))]
public class InvulnerableCircle : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Player.PlayerIsInvulnerable += SetMagicCircleRadius;
    }

    private void OnDisable()
    {
        Player.PlayerIsInvulnerable -= SetMagicCircleRadius;
    }

    private void SetMagicCircleRadius(float scale)
    {
        scale = Mathf.Clamp01(scale);
        spriteRenderer.transform.localScale = scale * Vector3.one;
        spriteRenderer.enabled = scale > 0f;
    }
}
