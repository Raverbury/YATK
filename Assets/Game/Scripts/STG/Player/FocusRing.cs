using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FocusRing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.enabled = Input.GetButton("Focus");
        transform.eulerAngles += new Vector3(0, 0, -1.5f);
    }
}
