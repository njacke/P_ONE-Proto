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
        G02_NPC npc = other.GetComponent<G02_NPC>();

        if (AttackType == G02_NPC.NpcStatus.Hostile) {
            if (other.GetComponent<G02_PlayerController>()) {
                Debug.Log("Player took damage");
            } else if (npc != null && npc.CurrentNpcStatus == G02_NPC.NpcStatus.Friendly) {
                Debug.Log("Friendly NPC took damage");
            }
        } else if (AttackType == G02_NPC.NpcStatus.Friendly && npc != null && npc.CurrentNpcStatus == G02_NPC.NpcStatus.Hostile) {
            Debug.Log("Enemy NPC took damage");
        }
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
