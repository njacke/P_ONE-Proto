using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private GameObject _spawnIndicator;
    [SerializeField] private float _startSpawnCd = 5f;
    [SerializeField] private float _spawnCdAdjustment = .05f;
    [SerializeField] private float _spawnDelay = 1f;
    private float _currentSpawnCD = 0f;
    private float _xMin;
    private float _xMax;
    private float _yMin;
    private float _yMax;

    private void Start() {
        _xMin = G02_GameManager.Instance.MinXBoundry;
        _xMax = G02_GameManager.Instance.MaxXBoundry;
        _yMin = G02_GameManager.Instance.MinYBoundry;
        _yMax = G02_GameManager.Instance.MaxYBoundry;        
    }

    private void Update() {
        _currentSpawnCD -= Time.deltaTime;
        if (_currentSpawnCD <= 0f) {
            StartCoroutine(SpawnEnemy());
            _currentSpawnCD = _startSpawnCd;
        }
    }

    private IEnumerator SpawnEnemy(){
        var spawnPosX = UnityEngine.Random.Range(_xMin, _xMax);
        var spawnPosY = UnityEngine.Random.Range(_yMin, _yMax);
        var spawnPos = new Vector3 (spawnPosX, spawnPosY, 0);

        var newIndicator = Instantiate(_spawnIndicator, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(_spawnDelay);

        var rndEnemyIndex = UnityEngine.Random.Range(0, _enemyPrefabs.Length);
        Instantiate(_enemyPrefabs[rndEnemyIndex], spawnPos, Quaternion.identity);
        Destroy(newIndicator.gameObject);
        _startSpawnCd *= 1 - _spawnCdAdjustment;
    }
}
