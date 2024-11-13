using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_SkillsManager : MonoBehaviour
{
    public static Action<int, int> OnReviveUpdate;
    [SerializeField] private G02_Revive _skillRevive;
    [SerializeField] private int _maxReviveReq = 5;
    [SerializeField] private G02_Skill _skill0;
    [SerializeField] private G02_Skill _skill1;
    [SerializeField] private G02_Skill _skill2;
    private int _currentReviveReq = 1;
    private int _currentReviveReqCount = 1;
    private bool _firstReviveUsed = false;

    public G02_Skill GetSkill0 { get { return _skill0; } }
    public G02_Skill GetSkill1 { get { return _skill1; } }
    public G02_Skill GetSkill2 { get { return _skill2; } }

    public float CurrentSkill0CD { get; private set; } = 0f;
    public float CurrentSkill1CD { get; private set; } = 0f;
    public float CurrentSkill2CD { get; private set; } = 0f;

    private void OnEnable() {
        G02_NPC.OnDamageTaken += G02_NPC_OnDamageTaken;
    }

    private void OnDisable() {
        G02_NPC.OnDamageTaken -= G02_NPC_OnDamageTaken;        
    }

    private void Update() {
        CurrentSkill0CD -= Time.deltaTime;
        CurrentSkill1CD -= Time.deltaTime;
        CurrentSkill2CD -= Time.deltaTime;
    }

    private void G02_NPC_OnDamageTaken(G02_NPC.NpcStatus status, int damageAmount) {
        if (status == G02_NPC.NpcStatus.Hostile) {
            _currentReviveReqCount += damageAmount;
            OnReviveUpdate?.Invoke(_currentReviveReqCount, _currentReviveReq);
        }
    }

    public void UseRevive(Vector3 targetPos) {
        if (_currentReviveReqCount < _currentReviveReq) {
            return;
        }

        if(_skillRevive.UseRevive(targetPos, _firstReviveUsed)) {
            _currentReviveReqCount = 0;

            if (!_firstReviveUsed) {
                _firstReviveUsed = true;
            } else if (_currentReviveReq < _maxReviveReq) {
                _currentReviveReq++;
            }
            OnReviveUpdate?.Invoke(_currentReviveReqCount, _currentReviveReq);
        }
    }

    public void UseSkill (int skill, Vector3 targetPos) {
        switch (skill) {
            case 0:
                if (CurrentSkill0CD <= 0f) {
                    _skill0.UseSkill(targetPos);
                    CurrentSkill0CD = _skill0.GetSkillCD;
                }
                break;
            case 1:
                if (CurrentSkill1CD <= 0f) {
                    _skill1.UseSkill(targetPos);
                    CurrentSkill1CD = _skill1.GetSkillCD;
                }
                break;
            case 2:
                if (CurrentSkill2CD <= 0f) {
                    _skill2.UseSkill(targetPos);
                    CurrentSkill2CD = _skill2.GetSkillCD;
                }
                break;
            default:
                break;
        }
    }
}
