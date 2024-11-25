using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G04_BE_Multiplier : G04_BlockEffect
{
    [SerializeField] private float _basePower = .2f;

    public override void ResolveEffect(G04_CombinedBlock target) {
        float newCurrentValue = target.GetBlockCurrentValue * (1 + _basePower * _myBlock.GetBlockLevel);
        target.SetCurrentValue(newCurrentValue);
    }
}
