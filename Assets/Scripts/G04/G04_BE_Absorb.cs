using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class G04_BE_Absorb : G04_BlockEffect
{
    [SerializeField] private float _basePower = .01f;
    public override void ResolveEffect(G04_CombinedBlock target, int effectLevel) {
        G04_CombinedBlock[] adjComboBlocks = _grid.GetAdjacentCombinedBlocks(target);
        float totalBaseValue = 0f;
        foreach (var adjComboBlock in adjComboBlocks) {
            totalBaseValue += adjComboBlock.GetBlockBaseValue;
        }

        Debug.Log("Absorbed total base value: " + totalBaseValue);
        Debug.Log("Absorbed current bonus value: " + target.GetBlockBonusValue);        

        var newBonusValue = target.GetBlockBonusValue + totalBaseValue * _basePower * effectLevel;

        Debug.Log("New bonus value: " + newBonusValue);
        target.SetBonusValue(newBonusValue);
    }
}
