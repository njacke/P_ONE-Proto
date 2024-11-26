using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G04_BE_Multiplier : G04_BlockEffect
{
    [SerializeField] private float _basePower = .1f;

    public override void ResolveEffect(G04_CombinedBlock target, int effectLevel) {
        float newValue = target.GetBlockMultiplier + _basePower * effectLevel;
        target.SetMultiplier(newValue);
    }
}
