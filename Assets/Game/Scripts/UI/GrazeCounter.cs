using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class GrazeCounter : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private TMP_Text text;

    private void OnValidate() {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable() {
        Player.PlayerSetGraze += UpdateGrazeCounter;
    }

    private void OnDisable() {
        Player.PlayerSetGraze -= UpdateGrazeCounter;
    }

    private void UpdateGrazeCounter(int grazes) {
        text.text = grazes.ToString();
    }
}
