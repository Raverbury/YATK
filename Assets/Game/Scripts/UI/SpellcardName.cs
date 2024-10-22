using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_Text))]
public class SpellcardName : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private TMP_Text text;

    public static UnityAction<string> RequestSetSpellcardName;

    private void OnValidate()
    {
        text = gameObject.GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        RequestSetSpellcardName += SetSpellcardName;
        AbstractSingle.SingleExplode += HideSpellcardName;
    }

    private void OnDisable()
    {
        RequestSetSpellcardName -= SetSpellcardName;
        AbstractSingle.SingleExplode -= HideSpellcardName;
    }

    private void HideSpellcardName()
    {
        text.enabled = false;
    }

    private void SetSpellcardName(string spellcardName)
    {
        text.enabled = true;
        text.text = spellcardName;
        Timing.RunCoroutine(_SpellcardNameStartAnim());
    }

    private IEnumerator<float> _SpellcardNameStartAnim()
    {
        // RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 a = new Vector2(192, -100);
        Vector2 b = new Vector2(-192, -100);
        Vector3 c = (b - a) / 40f;
        // for (int i = 0; i < 40; i++)
        // {
        //     transform.localPosition += c;
        //     yield return Timing.WaitForOneFrame;
        // }
        a = new Vector2(-192, -100);
        b = new Vector2(-192, 184);
        const int MOVE_UP_OVER_FRAMES = 30;
        c = (b - a) / MOVE_UP_OVER_FRAMES;
        transform.localPosition = a;
        yield return WaitForFrames.WaitWrapper(60);
        for (int i = 0; i < MOVE_UP_OVER_FRAMES; i++)
        {
            transform.localPosition += c;
            yield return Timing.WaitForOneFrame;
        }
    }
}