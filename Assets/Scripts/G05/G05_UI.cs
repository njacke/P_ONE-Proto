using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class G05_UI : MonoBehaviour
{
    public static Action OnDiceRoll;
    [SerializeField] private TextMeshProUGUI _diceValue;

    private void Awake() {
        _diceValue.text = "?";
    }

    private void OnEnable() {
        G05_Dice.OnValueUpdated += G05_Dice_OnValueUpdated;        
        G05_BoardManager.OnPlayerMoved += G05_BoardManager_OnPlayerMoved;        
    }

    private void OnDisable() {
        G05_Dice.OnValueUpdated -= G05_Dice_OnValueUpdated;
        G05_BoardManager.OnPlayerMoved -= G05_BoardManager_OnPlayerMoved;
    }

    private void G05_BoardManager_OnPlayerMoved(G05_BoardManager sender) {
        _diceValue.text = "?";
    }

    private void G05_Dice_OnValueUpdated(G05_Dice sender) {
        _diceValue.text = sender.CurrentValue.ToString();
    }

    public void RollDiceOnClick() {
        EventSystem.current.SetSelectedGameObject(null);

        if (G05_GameManager.Instance.GetTurnState != G05_GameManager.TurnState.Roll) {
            Debug.Log("Move player token to end turn.");
        } else {
            OnDiceRoll?.Invoke();
        }

    }
}
