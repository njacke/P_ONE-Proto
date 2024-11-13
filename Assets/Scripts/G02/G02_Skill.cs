using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class G02_Skill : MonoBehaviour
{
    [SerializeField] private G02_NPC.NpcStatus _skillTargetType;
    [SerializeField] private float _skillCD = 5f;
    [SerializeField] private float _cdAdjustment = .1f;
    [SerializeField] protected float _skillPower = 1f;
    [SerializeField] private float _powerAdjustment = .1f;
    [SerializeField] protected float _skillDuration = 5f;
    [SerializeField] private float _durationAdjustment = .1f;
    [SerializeField] protected float _skillRadius = .5f;
    [SerializeField] private float _radiusAdjustment = .1f;
    [SerializeField] private SpriteRenderer _indicator;
    [SerializeField] private float _indicatorDuration = .5f;

    public float GetSkillCD {get { return _skillCD; } }

    public void LevelUpCD() {
        _skillCD *= 1 - _cdAdjustment;
    }

    public void LevelUpPower() {
        _skillPower *= 1 + _powerAdjustment;
    }

    public void LevelUpDuration() {
        _skillDuration *= 1 + _durationAdjustment;
    }

    public void LevelUpRadius() {
        _skillRadius *= 1 + _radiusAdjustment;
    }

    public void UseSkill(Vector3 targetPos) {
        StartCoroutine(DisplayIndicator(targetPos));
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, _skillRadius);      

        foreach (var hit in hits) {
            var npc = hit.GetComponent<G02_NPC>();
            if (npc != null && npc.CurrentNpcStatus == _skillTargetType) {
                UseSkillOnNpc(npc);
            }
        }
    }

    private IEnumerator DisplayIndicator(Vector3 targetPos) {
        var newIndicator = Instantiate(_indicator, targetPos, Quaternion.identity);
        newIndicator.transform.localScale = new Vector3(newIndicator.transform.localScale.x * (_skillRadius * 2),
                                                        newIndicator.transform.localScale.y * (_skillRadius * 2),
                                                        newIndicator.transform.localScale.z * (_skillRadius * 2));
        yield return new WaitForSeconds(_indicatorDuration);
        Destroy(newIndicator.gameObject);
    }

    protected abstract void UseSkillOnNpc(G02_NPC npc);
}