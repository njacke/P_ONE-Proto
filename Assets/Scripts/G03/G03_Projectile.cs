using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_Projectile : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 10f;
    public G03_NPC.NpcStatus DamageType { get; set; }
    public int ProjectileDamage { get; set; } = 0;
    public float TargetDist { get; set; }

    protected virtual void Update() {
        MoveProjectile();
    }

    private void MoveProjectile() {
        transform.Translate(_moveSpeed * Time.deltaTime * Vector3.right);
    }
}
