using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G04_BE_Value : G04_BlockEffect
{
    [SerializeField] private int _basePower = 50;

    public override void ResolveEffect(G04_CombinedBlock target, int effectLevel) {
        float newBonusValue = target.GetBlockBonusValue + _basePower * effectLevel;
        target.SetBonusValue(newBonusValue);
    }
}
