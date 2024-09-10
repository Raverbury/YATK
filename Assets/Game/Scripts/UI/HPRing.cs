using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HPRing : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Image image;
    [SerializeField, HideInInspector]
    private Enemy enemy;

    private void OnValidate()
    {
        image = GetComponent<Image>();
        enemy = GetComponentInParent<Enemy>();
    }

    private void Awake() {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnEnable()
    {
        enemy.EntitySetHP += UpdateHPRing;
    }

    private void OnDisable()
    {
        enemy.EntitySetHP -= UpdateHPRing;
    }

    private void UpdateHPRing(int currentHP, int maxHP)
    {
        image.enabled = enemy.showHP;
        image.fillAmount = (float)currentHP / maxHP;
    }
}
