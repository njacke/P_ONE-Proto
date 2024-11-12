using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class G02_NpcAttack : MonoBehaviour
{
    [SerializeField] protected float _attackRange = .5f;
    [SerializeField] protected float _attackCD = 3f;
    public G02_NPC.NpcStatus AttackType { get; set; }
    public float GetAttackRange { get { return _attackRange; } }
    public float GetAttackCD { get { return _attackCD; } }
    public abstract void UseAttack(Vector3 targetPos);
}
