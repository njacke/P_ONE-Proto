using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_Objective : MonoBehaviour, G03_IDamageable
{
    public static Action<G03_Objective> OnDamageTaken;
    public static Action<G03_Objective> OnDeath;

    [SerializeField] private ObjectiveType _objectiveType;
    [SerializeField] private bool _isBase = false;
    [SerializeField] private int _startHP = 10;
    private int _currentHP = 0;
    private SpriteRenderer _spriteRenderer;

    public ObjectiveType GetObjectiveType { get { return _objectiveType; } }
    public bool GetIsBase { get { return _isBase; } }
    public int ObjectiveCount { get; set; }
    public int GetStartHP { get { return _startHP; } }
    public int GetCurrentHP { get { return _currentHP; } }

    public enum ObjectiveType {
        None,
        Hostile,
        Friendly,
        Destroyed
    }

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentHP = _startHP;
    }

    public bool TakeDamage(G03_NPC.NpcStatus damageType, int damageAmount) {
        bool damageTaken = false;

        if (_objectiveType == ObjectiveType.Hostile && damageType == G03_NPC.NpcStatus.Friendly) {
            _currentHP -= damageAmount;
            damageTaken = true;
        } else if (_objectiveType == ObjectiveType.Friendly && damageType == G03_NPC.NpcStatus.Hostile) {
            _currentHP -= damageAmount;
            damageTaken = true;
        }

        if (_currentHP <= 0) {
            Debug.Log("Objective destroyed; destroying object");
            _objectiveType = ObjectiveType.Destroyed;
            _spriteRenderer.color = Color.gray;
            OnDeath?.Invoke(this);
        }

        OnDamageTaken?.Invoke(this);

        //Debug.Log("damage taken on objective returning: " + damageTaken);

        return damageTaken;
    }
}
