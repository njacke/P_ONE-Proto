using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_Dice : MonoBehaviour
{
    public static Action<G05_Dice> OnValueUpdated;
    public int CurrentValue { get ; private set; }
    [SerializeField] private int _size = 6;

    private void OnEnable() {
        G05_UI.OnDiceRoll += G05_UI_OnDiceRoll;
    }

    private void OnDisable() {
        G05_UI.OnDiceRoll -= G05_UI_OnDiceRoll;        
    }

    private void G05_UI_OnDiceRoll() {
        RollDice();
    }

    public void RollDice() {
        Debug.Log("Roll dice called");
        CurrentValue = UnityEngine.Random.Range(1, _size + 1);
        OnValueUpdated?.Invoke(this);
    }
}
