using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_Launcher : MonoBehaviour
{
    [SerializeField] GameObject _entityPrefab;
    [SerializeField] private float _startDirChangeCD = .2f;
    [SerializeField] private int _maxDirRetryCount = 100;

    private float _currentSpawnCD;
    private float _currentDirChangeCD;
    private Vector2 _lastDirection = Vector2.zero;
    private G01_Projectile _currentProjectile;

    private void Awake() {
        _currentDirChangeCD = _startDirChangeCD;
    }

    private void Start() {
        _currentSpawnCD = G01_GameManager.Instance.GetCurrentProjectileSpawnCd;

        if (G01_GameManager.Instance.GetGameVersion == G01_GameManager.GameVersion.RandomLauncher) {
            SpawnProjectileRnd();
        } else if (G01_GameManager.Instance.GetGameVersion == G01_GameManager.GameVersion.LockInLauncher) {
            SpawnProjectileLock();
        } else if (G01_GameManager.Instance.GetGameVersion == G01_GameManager.GameVersion.DirectionalLauncher) {
            SpawnProjectileDir();
        }
    }
    
    private void Update() {
        if (G01_GameManager.Instance.GetGameVersion == G01_GameManager.GameVersion.RandomLauncher) {
            _currentSpawnCD -= Time.deltaTime;
            if (_currentSpawnCD <= 0f) {
                SpawnProjectileRnd();
            }            
        } else if (G01_GameManager.Instance.GetGameVersion == G01_GameManager.GameVersion.LockInLauncher) {
            if (_currentProjectile == null) {
                _currentSpawnCD -= Time.deltaTime;
                if (_currentSpawnCD <= 0f) {
                    SpawnProjectileLock();
                }
            } else {
                _currentDirChangeCD -= Time.deltaTime;
                if (_currentDirChangeCD <= 0f) {
                    _currentProjectile.SetDirection = GetNewDirection();
                    _currentDirChangeCD = _startDirChangeCD;
                }
            }
        } else if (G01_GameManager.Instance.GetGameVersion == G01_GameManager.GameVersion.DirectionalLauncher) {
            if (_currentProjectile == null) {
                _currentSpawnCD -= Time.deltaTime;
                if (_currentSpawnCD <= 0f) {
                    SpawnProjectileDir();
                }
            }
        }

        //Debug.Log("Current Spawn Cooldown: " + _currentSpawnCD);
    }

    private void SpawnProjectileRnd() {
        _currentProjectile = Instantiate(_entityPrefab, this.transform.position, Quaternion.identity).GetComponent<G01_Projectile>();
        _currentProjectile.SetDirection = GetNewDirection();
        _currentSpawnCD = G01_GameManager.Instance.GetCurrentProjectileSpawnCd;
    }

    private void SpawnProjectileLock() {
        _currentProjectile = Instantiate(_entityPrefab, this.transform.position, Quaternion.identity).GetComponent<G01_Projectile>();
        _currentProjectile.SetDirection = GetNewDirection();        
        _currentSpawnCD = G01_GameManager.Instance.GetCurrentProjectileSpawnCd;
        _currentDirChangeCD = _startDirChangeCD;
    }


    private void SpawnProjectileDir() {
        _currentProjectile = Instantiate(_entityPrefab, this.transform.position, Quaternion.identity).GetComponent<G01_Projectile>();
        _currentProjectile.SetDirection = GetNewDirection();
        _currentSpawnCD = G01_GameManager.Instance.GetCurrentProjectileSpawnCd;        
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
            _currentSpawnCD = G01_GameManager.Instance.GetCurrentProjectileSpawnCd;
        }
    }

    public void SetProjectileDir(int dirIndex) {
        if (_currentProjectile == null) {
            return;
        }

        var dir = Vector2.zero;
        switch (dirIndex) {
            case 0:
                dir = new Vector2(-1, 1);
                break;
            case 1:
                dir = new Vector2(1, 1);
                break;
            case 2:
                dir = new Vector2(1, -1);
                break;
            case 3:
                dir = new Vector2(-1, -1);
                break;
            default:
                break;
        }

        _currentProjectile.SetDirection = dir;
        _lastDirection = dir;
    }
}
