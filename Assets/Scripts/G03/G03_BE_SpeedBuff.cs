using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class G03_BE_SpeedBuff : G03_BombEffect
{
    [SerializeField] private float _speedBuff = 1.25f;
    [SerializeField] private float _powerFactor = .25f;
    [SerializeField] private float _buffDuration = 5f;

    public override bool SetEffectPower(int power) {
        _speedBuff += _powerFactor * power;
        return true;
    }

    public override bool ApplyEffect(G03_NPC npc) {
        npc.UpdateMoveSpeed(npc.GetStartMoveSpeed * _speedBuff, _buffDuration);
        return true;
    }

    public override bool RemoveEffect(G03_NPC npc) {
        npc.UpdateMoveSpeed(npc.GetStartMoveSpeed);
        return true;
    }
}
