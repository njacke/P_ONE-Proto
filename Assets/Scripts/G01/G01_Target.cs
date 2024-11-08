using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class G01_Target : G01_Entity
{
    public static Action<Vector3> OnTargetMatched;
    public static Action OnTargetNotMatched;
    public static Action<Vector3> OnTimerEnded;

    [SerializeField] TextMeshPro _timerText; 

    private float _timer = 0f;

    protected override void Awake() {
        base.Awake();
        Init();
    }

    private void Update() {
        _timer -= Time.deltaTime;
        _timerText.text = Mathf.Ceil(_timer).ToString("F0");

        if (_timer < 0f) {
            Destroy(this.gameObject);
            OnTimerEnded?.Invoke(this.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Trigger detected on Target.");
        var projectile = other.gameObject.GetComponent<G01_Projectile>();
        if (projectile != null) {
            if (projectile.GetShapeType == _shapeType && projectile.GetColorType == _colorType) {
                    Destroy(this.gameObject);
                    OnTargetMatched?.Invoke(this.transform.position);
            } else {
                OnTargetNotMatched?.Invoke();
            }     
        }
    }

    private void Init() {
        int _shapeRNG = UnityEngine.Random.Range(1, Enum.GetValues(typeof(ShapeType)).Length);
        int _colorRNG = UnityEngine.Random.Range(1, Enum.GetValues(typeof(ColorType)).Length);
        UpdateEntity((ShapeType)_shapeRNG, (ColorType)_colorRNG);

        _timer = UnityEngine.Random.Range(G01_GameManager.Instance.GetCurrentMinTargetTimer, G01_GameManager.Instance.GetCurrentMaxTargetTimer + 1); // +1 because exclusive
        _timerText.text = Mathf.CeilToInt(_timer).ToString("F0");
    }
}

