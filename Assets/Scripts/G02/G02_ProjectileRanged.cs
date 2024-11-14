using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_ProjectileRanged : G02_Projectile
{
    private void OnTriggerEnter2D(Collider2D other) {
        G02_IDamageable damageable = other.gameObject.GetComponent<G02_IDamageable>();
        if (damageable != null && damageable.TakeDamage(DamageType, ProjectileDamage)) {
            Destroy(this.gameObject);
        } else if (other.gameObject.layer == LayerMask.NameToLayer("Environment")) {
            Destroy(this.gameObject);
        }
    }
}
