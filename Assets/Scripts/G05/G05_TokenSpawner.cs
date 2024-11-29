using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_TokenSpawner : MonoBehaviour
{
    public static Action<G05_Token> OnPlayerSpawned;
    public static Action<G05_Enemy> OnEnemySpawned;
    [SerializeField] private GameObject _playerTokenPrefab;
    [SerializeField] private G05_Field[] _playerTokensSpawnFields;
    [SerializeField] private GameObject _enemyTokenPrefab;
    [SerializeField] private G05_Field[] _enemyTokensSpawnFields;

    private void Start() {
        SpawnPlayerTokens();
        SpawnEnemyTokens();
    }

    private void SpawnPlayerTokens() {
        foreach (var field in _playerTokensSpawnFields) {
            var spawnPos = new Vector3 (field.transform.position.x, field.transform.position.y, 0f);
            var newPlayer = Instantiate(_playerTokenPrefab, spawnPos, Quaternion.identity).GetComponent<G05_Token>();
            
            field.CurrentToken = newPlayer;
            newPlayer.CurrentField = field;
            OnPlayerSpawned?.Invoke(newPlayer);
        }
    }

    private void SpawnEnemyTokens() {
        foreach (var field in _enemyTokensSpawnFields) {
            var spawnPos = new Vector3 (field.transform.position.x, field.transform.position.y, 0f);
            var newEnemy = Instantiate(_enemyTokenPrefab, spawnPos, Quaternion.identity).GetComponent<G05_Enemy>();
            
            field.CurrentToken = newEnemy;
            newEnemy.CurrentField = field;
            OnEnemySpawned?.Invoke(newEnemy);
        }
    }
}
