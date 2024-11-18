using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_BE_AttackBuff : G03_BombEffect
{
    [SerializeField] private float _attackBuff = 1.25f;
    [SerializeField] private float _powerFactor = .25f;
    [SerializeField] private float _buffDuration = 5f;


    public override bool SetEffectPower(int power) {
        _attackBuff += _powerFactor * power;
        return true;   
    }

    public override bool ApplyEffect(G03_NPC npc) {
        npc.UpdateAttackCd(true, npc.GetStartAttackCD / _attackBuff, _buffDuration);
        return true;
    }

    public override bool RemoveEffect(G03_NPC npc) {
        npc.UpdateAttackCd(false, npc.GetNpcAttack.GetAttackCD);
        return true;
    }

}
