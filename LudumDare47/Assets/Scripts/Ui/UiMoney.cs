﻿using UnityEngine;
using TMPro;
using Manager;

public class UiMoney : MonoBehaviour
{
    private TextMeshProUGUI _textField;

    private void Start()
    {
        _textField = GetComponent<TextMeshProUGUI>();
        UpdateUI(GameManager.Instance.startMoney,0);
        GameManager.Instance.OnMoneyChanged += UpdateUI;
    }

    private void UpdateUI(int money, int sumToAdd)
    {
        _textField.text = $"{money}$";
    }
}