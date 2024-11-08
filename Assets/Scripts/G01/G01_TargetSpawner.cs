using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_TargetSpawner : MonoBehaviour
{
    [SerializeField] GameObject _targetPrefab;
    [SerializeField] Vector3[] _targetPositions;

    private void Start() {
        SpawnInitialTargets();    
    }

    private void OnEnable() {
        G01_Target.OnTargetMatched += G01_Target_OnTargetMatched;
        G01_Target.OnTimerEnded += G01_Target_OnTimerEnded;
    }

    private void OnDisable() {
        G01_Target.OnTargetMatched -= G01_Target_OnTargetMatched;
        G01_Target.OnTimerEnded -= G01_Target_OnTimerEnded;        
    }

    private void SpawnInitialTargets() {
        foreach (var targetPos in _targetPositions) {
            Instantiate(_targetPrefab, targetPos, Quaternion.identity);            
        }
    }

    private void G01_Target_OnTargetMatched(Vector3 pos) {
        StartCoroutine(SpawnTargetWithDelayRoutine(pos));
    }

    private void G01_Target_OnTimerEnded(Vector3 pos) {
        StartCoroutine(SpawnTargetWithDelayRoutine(pos));
    }

    private IEnumerator SpawnTargetWithDelayRoutine(Vector3 pos) {
        yield return new WaitForSeconds(G01_GameManager.Instance.GetCurrentTargetSpawnDelay);
        Instantiate(_targetPrefab, pos, Quaternion.identity);       
    }
}
