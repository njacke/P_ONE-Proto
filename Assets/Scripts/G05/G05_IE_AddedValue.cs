using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_IE_AddedValue : G05_ItemEffect
{
    private int[] _values = {-3, -2, -1, 1, 2, 3};
    public G05_IE_AddedValue() {
        EffectCategory = EffectCat.PostRoll;
        EffectName = "Bonus Value";
        EffectValue = _values[Random.Range(0, _values.Length)];

        string prefix = "";
        if (EffectValue > 0) {
            prefix = "+";
        }

        EffectValueText = prefix + EffectValue.ToString();
    }

    public override void ResolveEffect() {
        var dice = G05_GameManager.Instance.GetDice;
        dice.AddBonusValue(EffectValue);
    }
}
