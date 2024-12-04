using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_IE_RollValue : G05_ItemEffect
{
    private int[] _values = {1, 6};
    public G05_IE_RollValue() {
        EffectCategory = EffectCat.PostRoll;
        EffectName = "Change Base";
        EffectValue = _values[Random.Range(0, _values.Length)];
        EffectValueText = EffectValue.ToString();
    }

    public override void ResolveEffect() {
        var dice = G05_GameManager.Instance.GetDice;
        dice.UpdateRollValue(EffectValue);
    }
}