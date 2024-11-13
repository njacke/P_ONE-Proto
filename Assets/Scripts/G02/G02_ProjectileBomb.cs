using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_ProjectileBomb : G02_Projectile
{
    [SerializeField] private float _explosionDuration = .5f;
    [SerializeField] private SpriteRenderer _attackVisual;
    private CircleCollider2D _attackCollider;
    private Vector3 _startPos;
    private Coroutine _explosionRoutine = null;

    private void Awake() {
        _attackCollider = GetComponent<CircleCollider2D>();

        _attackCollider.enabled = false;
        _attackVisual.enabled = false;
    }

    private void Start() {
        _startPos = this.transform.position;
    }

    protected override void Update() {
        base.Update();
        if (_explosionRoutine == null && Vector3.Distance(_startPos, this.transform.position) > TargetDist) {
            _explosionRoutine = StartCoroutine(ExplosionRoutine());
        }        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        G02_IDamageable damageable = other.gameObject.GetComponent<G02_IDamageable>();
        damageable?.TakeDamage(DamageType, ProjectileDamage);
    }

    private IEnumerator ExplosionRoutine() {
        _moveSpeed = 0f;
        _attackCollider.enabled = true;
        _attackVisual.enabled = true;
        yield return new WaitForSeconds(_explosionDuration);
        Destroy(this.gameObject);
    }
}
