using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class G01_Launcher : MonoBehaviour
{
    [SerializeField] GameObject _entityPrefab;
    [SerializeField] private float _startCD = 3f;
    [SerializeField] private int _maxDirRetryCount = 100;

    private float _currentCD;
    private Vector2 _lastDirection = Vector2.zero;

    private void Awake() {
        _currentCD = _startCD;
    }
    
    private void Update() {
        _currentCD -= Time.deltaTime;
        if (_currentCD <= 0f) {
            SpawnProjectile();
        }
    }

    private void SpawnProjectile() {
        var dir = GetNewDirection();
        int retryCount = 0;
        
        while (Vector2.Equals(dir, _lastDirection) && retryCount < _maxDirRetryCount) {
            dir = GetNewDirection();
            retryCount++;
        }

        var newProjectile = Instantiate(_entityPrefab, this.transform.position, Quaternion.identity).GetComponent<G01_Projectile>();
        newProjectile.SetDirection = dir;
        _lastDirection = dir;
        _currentCD = _startCD;
    }

    private Vector2 GetNewDirection() {
        int _dirX = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        int _dirY = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        var dir = new Vector2(_dirX, _dirY);

        return dir;
    }
}
