using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class G02_NpcMeleeAttack : G02_NpcAttack
{
    [SerializeField] private float _attackDuration = .5f;
    private PolygonCollider2D _attackCollider;
    private SpriteRenderer _attackVisual;

    private void Awake() {
        _attackCollider = GetComponent<PolygonCollider2D>();
        _attackVisual = GetComponentInChildren<SpriteRenderer>();

        _attackCollider.enabled = false;
        _attackVisual.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        G02_IDamageable damageable = other.gameObject.GetComponent<G02_IDamageable>();
        damageable?.TakeDamage(AttackType, _attackDamage);
    }


    public override void UseAttack(Vector3 targetPos) {
        Debug.Log("Melee attack used");
        StartCoroutine(UseAttackRoutine(targetPos));
    }

    private IEnumerator UseAttackRoutine(Vector3 targetPos) {
        Vector3 dir = targetPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
        _attackCollider.enabled = true;
        _attackVisual.enabled = true;

        yield return new WaitForSeconds(_attackDuration);

        _attackCollider.enabled = false;
        _attackVisual.enabled = false;
    }
}
