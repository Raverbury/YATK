using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PowerCounter : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private TMP_Text text;

    private void OnValidate() {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable() {
        Player.PlayerSetPower += UpdateGrazeCounter;
    }

    private void OnDisable() {
        Player.PlayerSetPower -= UpdateGrazeCounter;
    }

    private void UpdateGrazeCounter(int power) {
        text.text = power.ToString();
    }
}
