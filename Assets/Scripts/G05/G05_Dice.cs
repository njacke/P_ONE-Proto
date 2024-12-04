using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class G05_Dice : MonoBehaviour
{
    public static Action<G05_Dice> OnValueUpdated;
    public static Action<G05_Dice> OnDiceRoll;
    public int BaseValue { get ; private set; }
    public int BonusValue { get ; private set; }
    public int SpecialValue { get ; private set; } // player token specials & field speed
    public int BaseBonus { get ; private set; }
    public int TotalValue { get { return CalculateTotalValue(); } }
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
        BaseValue = UnityEngine.Random.Range(1, _size + 1);
        BonusValue = 0;
        SpecialValue = 0;

        Debug.Log("Base multi pre-roll: " + BaseBonus);

        if (BaseBonus != 0) {
            BonusValue += BaseBonus / 100 * BaseValue;
            BaseBonus = 0;
        }

        OnDiceRoll?.Invoke(this);
        OnValueUpdated?.Invoke(this);
    }

    public void UpdateRollValue(int value) {
        BaseValue = value;
        Debug.Log("Roll value updated to: " + BaseValue);
        OnValueUpdated?.Invoke(this);
    }

    public void AddBonusValue(int value) {
        BonusValue += value;
        Debug.Log("Bonus value updated to: " + BonusValue);
        OnValueUpdated?.Invoke(this);
    }

    public void UpdateSpecialValue(int value) {
        SpecialValue = value;
        OnValueUpdated?.Invoke(this);
    }

    public void AddBaseBonus(int value) {
        BaseBonus += value;
    }

    private int CalculateTotalValue() {
        var totalValue = BaseValue + BonusValue + SpecialValue;
        if (totalValue < 1) {
            totalValue = 1;
        }

        return totalValue;
    }
}
