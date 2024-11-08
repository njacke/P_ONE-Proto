using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class G01_DisplayUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _displayHP;
    [SerializeField] TextMeshProUGUI _displayScore;

    private void OnEnable() {
        G01_GameManager.OnGameStateChanged += G01_GameManager_OnGameStateChanged;
    }

    private void OnDisable() {
        G01_GameManager.OnGameStateChanged -= G01_GameManager_OnGameStateChanged;        
    }

    private void G01_GameManager_OnGameStateChanged() {
        _displayHP.text = "HP " + G01_GameManager.Instance.GetCurrentHP.ToString();
        _displayScore.text = "SCORE " + G01_GameManager.Instance.GetCurrentScore.ToString();
    }
}
