using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_BuffAttack : G02_Skill
{
    [SerializeField] private float _baseAttackBuff = 1.5f;

    protected override void UseSkillOnNpc(G02_NPC npc) {
        StartCoroutine(BuffAttackRoutine(npc));
    }

    private IEnumerator BuffAttackRoutine(G02_NPC npc) {
        var ogAttack = npc.GetStartAttackCD;
        var newAttack = ogAttack * + _baseAttackBuff * _skillPower;
        npc.UpdateAttackCD(newAttack);
        yield return new WaitForSeconds(_skillDuration);
        npc.UpdateAttackCD(ogAttack);
    }
}
