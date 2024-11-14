using UnityEngine;

interface G03_IDamageable {
    public bool TakeDamage(G03_NPC.NpcStatus damageType, int damageAmount);
}