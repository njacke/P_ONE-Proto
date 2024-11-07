using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_Projectile : G01_Entity
{
    [SerializeField] private float _moveSpeed = 1f;
    public Vector2 SetDirection { set { _direction = value; } } 
    private Vector2 _direction;

    protected override void Awake() {
        base.Awake();
    }

    private void Update() {
        transform.Translate(_moveSpeed * Time.deltaTime * _direction);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger detected on Projectile.");
        var ringZone = other.gameObject.GetComponent<G01_Zone>();
        if (ringZone != null) {
            UpdateEntity(ringZone.GetShapeType, ringZone.GetColorType);
        } else if (other.gameObject.GetComponent<G01_Target>()) {
            Destroy(this.gameObject);
        }
    }
}