using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_BE_DmgInstant : G03_BombEffect
{
    private int _damageAmount = 1;

    public override bool SetEffectPower(int power) {
        _damageAmount += power;
        return true;
    }

    public override bool ApplyEffect(G03_NPC npc) {        
        var dmgType = G03_NPC.NpcStatus.None;
        if (npc.CurrentNpcStatus == G03_NPC.NpcStatus.Hostile) {
            dmgType = G03_NPC.NpcStatus.Friendly;
        } else if (npc.CurrentNpcStatus == G03_NPC.NpcStatus.Friendly) {
            dmgType = G03_NPC.NpcStatus.Hostile;
        }
        npc.TakeDamage(dmgType, _damageAmount);
        return true;
    }

    public override bool RemoveEffect(G03_NPC npc) {
        return true;
    }
}
