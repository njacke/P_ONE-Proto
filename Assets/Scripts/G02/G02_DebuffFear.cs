using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_DebuffFear : G02_Skill
{
    [SerializeField] private float _baseSpeedDebuff = 1f;

    protected override void UseSkillOnNpc(G02_NPC npc) {
        StartCoroutine(BuffSpeedRoutine(npc));
    }

    private IEnumerator BuffSpeedRoutine(G02_NPC npc) {
        var ogSpeed = npc.GetMoveSpeed;
        var newSpeed = ogSpeed * _baseSpeedDebuff * _skillPower;
        npc.SetFear(true, newSpeed);
        yield return new WaitForSeconds(_skillDuration);
        npc.SetFear(false, ogSpeed);
    }
}
