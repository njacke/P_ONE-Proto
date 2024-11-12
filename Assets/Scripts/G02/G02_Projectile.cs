using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_Projectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    public int ProjectileDamage { get; set; } = 0;
    public G02_NPC.NpcStatus ProjectileType { get; set; }


    private void Update() {
        MoveProjectile();
    }

    private void OnTriggerEnter2D(Collider2D other) {
       G02_NPC npc = other.GetComponent<G02_NPC>();

        if (ProjectileType == G02_NPC.NpcStatus.Hostile) {
            if (other.GetComponent<G02_PlayerController>()) {
                Debug.Log("Player took damage");
                Destroy(this.gameObject);
            } else if (npc != null && npc.CurrentNpcStatus == G02_NPC.NpcStatus.Friendly) {
                Debug.Log("Friendly NPC took damage");
                Destroy(this.gameObject);
            }
        } else if (ProjectileType == G02_NPC.NpcStatus.Friendly && npc != null && npc.CurrentNpcStatus == G02_NPC.NpcStatus.Hostile) {
            Debug.Log("Enemy NPC took damage");
            Destroy(this.gameObject);
        }
    }

    private void MoveProjectile() {
        transform.Translate(_moveSpeed * Time.deltaTime * Vector3.right);
    }
}
