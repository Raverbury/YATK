using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
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
        Player.PlayerChangeFocus += SetFocusVisible;
    }

    private void OnDisable()
    {
        Player.PlayerChangeFocus -= SetFocusVisible;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, -1.5f);
    }

    private void SetFocusVisible(bool enable)
    {
        spriteRenderer.enabled = enable;
    }
}
