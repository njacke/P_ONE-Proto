using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_BE_HpInstant : G03_BombEffect
{
    private int _healAmount = 1;

    public override bool SetEffectPower(int power) {
        _healAmount += power;
        return true;
    }

    public override bool ApplyEffect(G03_NPC npc) {
        npc.UpdateHP(npc.GetCurrentHP + _healAmount);
        return true;
    }

    public override bool RemoveEffect(G03_NPC npc) {
        return true;
    }
}
