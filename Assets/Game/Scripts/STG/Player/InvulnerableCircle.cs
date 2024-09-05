using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class InvulnerableCircle : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private SpriteRenderer spriteRenderer;

    private void OnValidate() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, 1f);
    }

    private void SetMagicCircleRadius(float scale) {
        scale = Mathf.Clamp01(scale);
        spriteRenderer.transform.localScale = scale * Vector3.one;
        spriteRenderer.enabled = scale > 0f;
    }
}
