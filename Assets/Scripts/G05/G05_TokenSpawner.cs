using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_TokenSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerTokenPrefab;
    [SerializeField] private G05_Field[] _playerTokensSpawnFields;

    private void Start() {
        SpawnPlayerTokens();
    }

    private void SpawnPlayerTokens() {
        foreach (var field in _playerTokensSpawnFields) {
            // z: 1f to avoid raycast detection issues 
            var spawnPos = new Vector3 (field.transform.position.x, field.transform.position.y, 1f);
            var newPlayer = Instantiate(_playerTokenPrefab, spawnPos, Quaternion.identity).GetComponent<G05_Token>();
            
            field.CurrentToken = newPlayer;
            newPlayer.CurrentField = field;
        }
    }

}
