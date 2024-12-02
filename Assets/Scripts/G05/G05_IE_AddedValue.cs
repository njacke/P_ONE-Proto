using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_IE_AddedValue : G05_ItemEffect
{
    private int[] _values = {-3, -2, -1, 1, 2, 3};
    public G05_IE_AddedValue() {
        EffectCategory = EffectCat.PostRoll;
        EffectName = "Add Bonus";
        EffectValue = _values[Random.Range(0, _values.Length)];
    }

    public override void ResolveEffect() {
        var dice = G05_GameManager.Instance.GetDice;
        dice.AddBonusValue(EffectValue);
    }
}
