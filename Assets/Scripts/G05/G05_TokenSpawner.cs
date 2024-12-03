using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_TokenSpawner : MonoBehaviour
{
    public static Action<G05_Player> OnPlayerSpawned;
    public static Action<G05_Enemy> OnEnemySpawned;
    [SerializeField] private int _minEnemySpawnCd = 5;
    [SerializeField] private int _maxEnemySpawnCd = 8;
    [SerializeField] private int _baseStartSpawnDelay = 5;
    [SerializeField] private GameObject[] _playerTokenPrefabs;
    [SerializeField] private G05_Field[] _playerTokensSpawnFields;
    [SerializeField] private GameObject[] _enemyTokenPrefabs;
    [SerializeField] private G05_Field[] _enemyTokensSpawnFields;
    private Dictionary<G05_Field, int> _enemySpawnFieldCdDict;

    private void Awake() {
        _enemySpawnFieldCdDict = new();

        for (int i = 0; i < _enemyTokensSpawnFields.Length; i++) {
            _enemySpawnFieldCdDict[_enemyTokensSpawnFields[i]] = i * _baseStartSpawnDelay;
        }
    }

    private void Start() {
        SpawnStartPlayerTokens();
        SpawnStartEnemyTokens();
    }

    private void OnEnable() {
        G05_GameManager.OnTurnStateChanged += G05_GameManager_OnTurnStateChanged;        
    }

    private void OnDisable() {
        G05_GameManager.OnTurnStateChanged -= G05_GameManager_OnTurnStateChanged;        
    }

    private void G05_GameManager_OnTurnStateChanged(G05_GameManager sender) {
        if (G05_GameManager.Instance.GetTurnState == G05_GameManager.TurnState.Roll) {
            foreach (var field in _enemyTokensSpawnFields) {
                SpawnEnemyToken(field);
            }
        }
    }

    private void SpawnStartPlayerTokens() {
        if (_playerTokenPrefabs.Length != _playerTokensSpawnFields.Length) {
            Debug.Log("Player prefabs and spawns pos lengths don't match.");
            return;
        }

        for (int i = 0; i < _playerTokenPrefabs.Length; i++) {
            var field = _playerTokensSpawnFields[i];
            var spawnPos = new Vector3 (field.transform.position.x, field.transform.position.y, 0f);
            var newPlayer = Instantiate(_playerTokenPrefabs[i], spawnPos, Quaternion.identity).GetComponent<G05_Player>();
            
            field.CurrentToken = newPlayer;
            newPlayer.CurrentField = field;
            OnPlayerSpawned?.Invoke(newPlayer);
        }
    }

    private void SpawnStartEnemyTokens() {
        foreach (var field in _enemyTokensSpawnFields) {
            SpawnEnemyToken(field);
        }
    }

    private void SpawnEnemyToken(G05_Field field) {
        if (_enemySpawnFieldCdDict[field] > 0 || field.CurrentToken != null) {
            _enemySpawnFieldCdDict[field]--;
            return;
        }

        int rndIndex = UnityEngine.Random.Range(0, _enemyTokenPrefabs.Length);
        var spawnPos = new Vector3 (field.transform.position.x, field.transform.position.y, 0f);
        var newEnemy = Instantiate(_enemyTokenPrefabs[rndIndex], spawnPos, Quaternion.identity).GetComponent<G05_Enemy>();
        
        field.CurrentToken = newEnemy;
        newEnemy.CurrentField = field;

        int rndCd = UnityEngine.Random.Range(_minEnemySpawnCd, _maxEnemySpawnCd + 1); // max exclusive
        _enemySpawnFieldCdDict[field] = rndCd;

        OnEnemySpawned?.Invoke(newEnemy);
    }
}
