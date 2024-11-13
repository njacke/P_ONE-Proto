using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_BuffSpeed : G02_Skill
{
    [SerializeField] private float _baseSpeedBuff = 1.5f;

    protected override void UseSkillOnNpc(G02_NPC npc) {
        StartCoroutine(BuffSpeedRoutine(npc));
    }

    private IEnumerator BuffSpeedRoutine(G02_NPC npc) {
        var ogSpeed = npc.GetMoveSpeed;
        var newSpeed = ogSpeed * (_baseSpeedBuff) * (_skillPower);
        npc.UpdateMoveSpeed(newSpeed);
        yield return new WaitForSeconds(_skillDuration);
        npc.UpdateMoveSpeed(ogSpeed);
    }
}
