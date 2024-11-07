using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_TargetSpawner : MonoBehaviour
{
    [SerializeField] GameObject _targetPrefab;
    [SerializeField] float _spawnDelay = 1f;
    [SerializeField] Vector3[] _targetPositions;

    private void Awake() {
        Init();    
    }

    private void OnEnable() {
        G01_Target.OnDeath += G01_Target_OnDeath;
    }

    private void OnDisable() {
        G01_Target.OnDeath -= G01_Target_OnDeath;        
    }

    private void Init() {
        foreach (var targetPos in _targetPositions) {
            Instantiate(_targetPrefab, targetPos, Quaternion.identity);            
        }
    }

    private void G01_Target_OnDeath(Vector3 pos) {
        StartCoroutine(SpawnTargetWithDelayRoutine(pos));
    }

    private IEnumerator SpawnTargetWithDelayRoutine(Vector3 pos) {
        yield return new WaitForSeconds(_spawnDelay);
        Instantiate(_targetPrefab, pos, Quaternion.identity);       
    }
}
