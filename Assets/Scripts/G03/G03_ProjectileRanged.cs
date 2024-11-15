using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_ProjectileRanged : G03_Projectile
{
    [SerializeField] float _maxLifetime = 2f;

    protected override void Update() {
        base.Update();
        _maxLifetime -= Time.deltaTime;
        if (_maxLifetime <= 0f) {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        G03_IDamageable damageable = other.gameObject.GetComponent<G03_IDamageable>();
        if (damageable != null && damageable.TakeDamage(DamageType, ProjectileDamage)) {
            Destroy(this.gameObject);
        } else if (other.gameObject.layer == LayerMask.NameToLayer("Environment")) {
            Destroy(this.gameObject);
        }
    }
}
