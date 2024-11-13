using UnityEngine;

interface G02_IDamageable {
    public bool TakeDamage(G02_NPC.NpcStatus damageType, int damageAmount);
}