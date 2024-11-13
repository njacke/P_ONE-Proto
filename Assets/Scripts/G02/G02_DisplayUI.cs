using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class G02_DisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _displayXpText;
    [SerializeField] private TextMeshProUGUI _displayHpText;
    [SerializeField] private TextMeshProUGUI _skill0CdText;
    [SerializeField] private TextMeshProUGUI _skill1CdText;
    [SerializeField] private TextMeshProUGUI _skill2CdText;
    [SerializeField] private TextMeshProUGUI _skillReviveCdText;
    [SerializeField] private G02_SkillsManager _skillsManager;
    private bool _skill0ready = false;
    private bool _skill1ready = false;
    private bool _skill2ready = false;

    private void OnEnable() {
        G02_PlayerController.OnHpChange += G02_PlayerController_OnHpChange; 
        G02_SkillsManager.OnReviveUpdate += G02_SkillsManager_OnReviveUpdate;  
        G02_GameManager.OnXpChange += G02_GameManager_OnXpChange;     
    }

    private void OnDisable() {
        G02_PlayerController.OnHpChange -= G02_PlayerController_OnHpChange;
        G02_SkillsManager.OnReviveUpdate -= G02_SkillsManager_OnReviveUpdate;       
        G02_GameManager.OnXpChange -= G02_GameManager_OnXpChange;     
    }


    private void G02_PlayerController_OnHpChange(int newHP) {
        _displayHpText.text = "HP: " + newHP.ToString();
    }

    private void G02_SkillsManager_OnReviveUpdate(int count, int req) {
        if (count < req) {
            _skillReviveCdText.text = count.ToString() + "/" + req.ToString();
        } else {
            _skillReviveCdText.text = "READY";
        }
    }

    private void G02_GameManager_OnXpChange(int lvl, int xp, int xpReq) {
        _displayXpText.text = "LVL " + lvl.ToString() + " (" + xp.ToString() + "/" + xpReq.ToString() + ")";
    }

    private void Update() {
        if (_skillsManager.CurrentSkill0CD > 0f) {
            _skill0CdText.text = _skillsManager.CurrentSkill0CD.ToString("F1");         
            _skill0ready = false;               
        } else if (!_skill0ready) {
            _skill0CdText.text = "READY";
            _skill0ready = true;
        }
        if (_skillsManager.CurrentSkill1CD > 0f) {
            _skill1CdText.text = _skillsManager.CurrentSkill1CD.ToString("F1");      
            _skill1ready = false;                  
        } else if (!_skill1ready) {
            _skill1CdText.text = "READY";
            _skill1ready = true;
        }
        if (_skillsManager.CurrentSkill2CD > 0f) {
            _skill2CdText.text = _skillsManager.CurrentSkill2CD.ToString("F1"); 
            _skill2ready = false;                       
        } else if (!_skill2ready) {
            _skill2CdText.text = "READY";
            _skill2ready = true;
        }                
    }
}
