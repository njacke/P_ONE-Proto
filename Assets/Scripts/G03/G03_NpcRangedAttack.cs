using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_NpcRangedAttack : G03_NpcAttack
{
    [SerializeField] private GameObject _projectilePrefab;

    public override void UseAttack(Vector3 targetPos) {
        Vector3 dir = targetPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Debug.Log("Spawning projectile");

        var newProjectile = Instantiate(_projectilePrefab, this.transform.position, rotation).GetComponent<G03_Projectile>();
        newProjectile.DamageType = AttackType;
        newProjectile.ProjectileDamage = _attackDamage;
        newProjectile.TargetDist = Vector3.Distance(this.transform.position, targetPos);
    }
}
