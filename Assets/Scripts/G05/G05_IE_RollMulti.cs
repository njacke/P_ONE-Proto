using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_IE_RollMulti : G05_ItemEffect
{
    private int[] _values = {2, 2, 2, 3};
    public G05_IE_RollMulti() {
        EffectCategory = EffectCat.PreRoll;
        EffectName = "Base Multi";
        EffectValue = _values[Random.Range(0, _values.Length)];
    }

    public override void ResolveEffect() {
        var dice = G05_GameManager.Instance.GetDice;
        dice.AddBaseMulti(EffectValue);
    }
}
