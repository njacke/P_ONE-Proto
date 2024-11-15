using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_Objective : MonoBehaviour, G03_IDamageable
{
    public static Action<G03_Objective> OnDeath;

    [SerializeField] private ObjectiveType _objectiveType;
    [SerializeField] private bool _isBase = false;
    [SerializeField] private int _currentHP = 10;

    public ObjectiveType GetObjectiveType { get { return _objectiveType; } }
    public bool GetIsBase { get { return _isBase; } }

    public enum ObjectiveType {
        None,
        Hostile,
        Friendly
    }

    public bool TakeDamage(G03_NPC.NpcStatus damageType, int damageAmount) {
        Debug.LogWarning("Take damage entered");
        bool damageTaken = false;

        if (_objectiveType == ObjectiveType.Hostile && damageType == G03_NPC.NpcStatus.Friendly) {
            _currentHP--;
            damageTaken = true;
        } else if (_objectiveType == ObjectiveType.Friendly && damageType == G03_NPC.NpcStatus.Hostile) {
            _currentHP--;
            damageTaken = true;
        }

        if (_currentHP <= 0) {
            Debug.Log("Objective destroyed; destroying object");
            OnDeath?.Invoke(this);
            Destroy(this.gameObject);
        }

        Debug.Log("damage taken on objective returning: " + damageTaken);

        return damageTaken;
    }
}
