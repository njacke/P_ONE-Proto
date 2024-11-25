using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G04_BE_Level : G04_BlockEffect
{
    [SerializeField] private int _basePower = 1;

    public override void ResolveEffect(G04_CombinedBlock target) {
        var newLvl = target.GetBlockLevel + _basePower * _myBlock.GetBlockLevel;
        target.SetLevel(newLvl);
    }
}
