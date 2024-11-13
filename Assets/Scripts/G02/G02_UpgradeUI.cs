using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G02_UpgradeUI : MonoBehaviour
{
    public static Action OnUpgradeChosen;

    [SerializeField] G02_SkillsManager _skillsManager;

    private void OnEnable() {
        ResetAllButtons();
    }

    public void ResetAllButtons() {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons) {
            button.OnDeselect(null);
        }
    }

    public void Skill0PowerOnClick() {
        _skillsManager.GetSkill0.LevelUpPower();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill0RadiusOnClick() {
        _skillsManager.GetSkill0.LevelUpRadius();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill0DurationOnClick() {
        _skillsManager.GetSkill0.LevelUpDuration();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill0CooldownOnClick() {
        _skillsManager.GetSkill0.LevelUpCD();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill1PowerOnClick() {
        _skillsManager.GetSkill1.LevelUpPower();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill1RadiusOnClick() {
        _skillsManager.GetSkill1.LevelUpRadius();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill1DurationOnClick() {
        _skillsManager.GetSkill1.LevelUpDuration();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill1CooldownOnClick() {
        _skillsManager.GetSkill1.LevelUpCD();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill2PowerOnClick() {
        _skillsManager.GetSkill2.LevelUpPower();
        OnUpgradeChosen?.Invoke();
        }

    public void Skill2RadiusOnClick() {
        _skillsManager.GetSkill2.LevelUpRadius();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill2DurationOnClick() {
        _skillsManager.GetSkill2.LevelUpDuration();
        OnUpgradeChosen?.Invoke();
    }

    public void Skill2CooldownOnClick() {
        _skillsManager.GetSkill2.LevelUpCD();
        OnUpgradeChosen?.Invoke();
    }
}
