using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_Projectile : G01_Entity
{
    [SerializeField] private Transform _directionIndicator;
    public Vector2 SetDirection { set { _direction = value; UpdateDirIndicator(); } } 
    private Vector2 _direction;
    private bool _isMoving = false;
    private float _moveSpeed;
    private float _moveDelay;

    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
        _moveSpeed = G01_GameManager.Instance.GetProjectileSpeed;
        _moveDelay = G01_GameManager.Instance.GetCurrentProjectileDelay;

        if (G01_GameManager.Instance.GetGameVersion == G01_GameManager.GameVersion.RandomLauncher) {
            StartCoroutine(EnableMovementRoutine());
        }
        
        //Debug.Log("Projectile spawned with speed of: " + _moveSpeed + " and delay of: " + _moveDelay);
    }

    private void Update() {
        if (_isMoving) {
            transform.Translate(_moveSpeed * Time.deltaTime * _direction);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Trigger detected on Projectile.");
        var ringZone = other.gameObject.GetComponent<G01_Zone>();
        if (ringZone != null) {
            UpdateEntity(ringZone.GetShapeType, ringZone.GetColorType);
        } else if (other.gameObject.GetComponent<G01_Target>()) {
            Destroy(this.gameObject);
        }
    }

    private void UpdateDirIndicator() {
        // get angle from vector + convert to degrees
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg -45f; // -45f due to sprite default position topright (45 degrees)
        _directionIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public IEnumerator EnableMovementRoutine() {
        yield return new WaitForSeconds(_moveDelay);
        _isMoving = true;
        _directionIndicator.gameObject.SetActive(false);
    }

    public void EnableMovement() {
        _isMoving = true;
        _directionIndicator.gameObject.SetActive(false);
    }
}