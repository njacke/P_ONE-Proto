using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_Target : G01_Entity
{
    public static Action<Vector3> OnDeath;

    [SerializeField] private GameObject _targetPrefab;
    [SerializeField] private float _spawnDelay = 1f;

    protected override void Awake() {
        base.Awake();
        Init();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger detected on Target.");
        var projectile = other.gameObject.GetComponent<G01_Projectile>();
        if (projectile != null && projectile.GetShapeType == _shapeType && projectile.GetColorType == _colorType) {
                Destroy(this.gameObject);
                OnDeath?.Invoke(this.transform.position);
        }     
    }

    private void Init() {
        int _shapeRNG = UnityEngine.Random.Range(1, Enum.GetValues(typeof(ShapeType)).Length);
        int _colorRNG = UnityEngine.Random.Range(1, Enum.GetValues(typeof(ColorType)).Length);

        UpdateEntity((ShapeType)_shapeRNG, (ColorType)_colorRNG);
    }
}

