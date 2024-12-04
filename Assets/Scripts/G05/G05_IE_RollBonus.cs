using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_IE_RollBonus : G05_ItemEffect
{
    private int[] _values = {100, 100, 100, 200};
    public G05_IE_RollBonus() {
        EffectCategory = EffectCat.PreRoll;
        EffectName = "Roll Bonus";
        EffectValue = _values[Random.Range(0, _values.Length)];

        string prefix = "";
        if (EffectValue > 0) {
            prefix = "+";
        }

        EffectValueText = prefix + EffectValue.ToString() + "%";
    }

    public override void ResolveEffect() {
        var dice = G05_GameManager.Instance.GetDice;
        dice.AddBaseBonus(EffectValue);
    }
}
