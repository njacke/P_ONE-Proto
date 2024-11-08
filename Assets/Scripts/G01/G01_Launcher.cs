using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_Launcher : MonoBehaviour
{
    [SerializeField] GameObject _entityPrefab;
    [SerializeField] private float _startSpawnCD = 3f;
    [SerializeField] private float _startDirChangeCD = .2f;
    [SerializeField] private int _maxDirRetryCount = 100;

    private float _currentSpawnCD;
    private float _currentDirChangeCD;
    private Vector2 _lastDirection = Vector2.zero;
    private G01_Projectile _currentProjectile;

    private void Awake() {
        _currentSpawnCD = _startSpawnCD;
        _currentDirChangeCD = _startDirChangeCD;
    }

    private void Start() {
        SpawnProjectile();
    }
    
    private void Update() {
        if (_currentProjectile == null) {
            _currentSpawnCD -= Time.deltaTime;
            if (_currentSpawnCD <= 0f) {
                SpawnProjectile();
                //SpawnProjectileOld();
            }
        } else {
            _currentDirChangeCD -= Time.deltaTime;
            if (_currentDirChangeCD <= 0f) {
                _currentProjectile.SetDirection = GetNewDirection();
                _currentDirChangeCD = _startDirChangeCD;
            }
        }
    }

    private void SpawnProjectile() {
        _currentProjectile = Instantiate(_entityPrefab, this.transform.position, Quaternion.identity).GetComponent<G01_Projectile>();
        _currentProjectile.SetDirection = GetNewDirection();        
        _currentSpawnCD = _startSpawnCD;
        _currentDirChangeCD = _startDirChangeCD;
    }

    private void SpawnProjectileOld() {
        var newProjectile = Instantiate(_entityPrefab, this.transform.position, Quaternion.identity).GetComponent<G01_Projectile>();
        newProjectile.SetDirection = GetNewDirection();
        _currentSpawnCD = _startSpawnCD;
    }

    private Vector2 GetNewDirection() {
        var dir = GetDirection();
        int retryCount = 0;
        
        while (Vector2.Equals(dir, _lastDirection) && retryCount < _maxDirRetryCount) {
            dir = GetDirection();
            retryCount++;
        }

        _lastDirection = dir;
        return dir;
    }

    private Vector2 GetDirection() {
        int _dirX = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        int _dirY = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        var dir = new Vector2(_dirX, _dirY);

        return dir;
    }
    public void FireProjectile() {
        if (_currentProjectile != null) {
            _currentProjectile.EnableMovement();
            _currentProjectile = null;
            _currentSpawnCD = _startSpawnCD;
        }
    }
}
