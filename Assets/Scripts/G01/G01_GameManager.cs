using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_GameManager : Singleton<G01_GameManager>
{
    public static Action OnGameStateChanged;
    [SerializeField] private int _startHP = 3;
    [SerializeField] private float _startProjectileSpeed = 1f;
    [SerializeField] private float _startProjectileDelay = 1f;
    [SerializeField] private float _startTargetSpawnDelay = 1f;
    [SerializeField] private float _startMinTargetTimer = 20f;
    [SerializeField] private float _startMaxTargetTimer = 30f;

    [SerializeField] private float _adjustmentFactor = .05f;

    private float _currentProjectileSpeed = 0f;
    private float _currentProjectileDelay = 0f;
    private float _currentTargetSpawnDelay = 0f;
    private float _currentMinTargetTimer = 0f;
    private float _currentMaxTargetTimer = 0f;
    private int _currentHP = 0;
    private int _currentScore = 0;

    public int GetCurrentHP { get { return _currentHP; } }
    public int GetCurrentScore { get { return _currentScore; } }
    public float GetProjectileSpeed { get { return _currentProjectileSpeed; } }
    public float GetCurrentProjectileDelay { get { return _currentProjectileDelay; } }
    public float GetCurrentTargetSpawnDelay { get { return _currentTargetSpawnDelay; } }
    public int GetCurrentMinTargetTimer { get { return Mathf.CeilToInt(_currentMinTargetTimer); } }
    public int GetCurrentMaxTargetTimer { get { return Mathf.CeilToInt(_currentMaxTargetTimer); } }


    private void Start() {
        ResetGame();
    }

    private void OnEnable() {
        G01_Target.OnTargetMatched += G01_Target_OnTargetMatched;
        G01_Target.OnTargetNotMatched += G01_Target_OnTargetNotMatched;
        G01_Target.OnTimerEnded += G01_Target_OnTimerEnded;
    }


    private void OnDisable() {
        G01_Target.OnTargetMatched -= G01_Target_OnTargetMatched;
        G01_Target.OnTargetNotMatched -= G01_Target_OnTargetNotMatched;        
        G01_Target.OnTimerEnded -= G01_Target_OnTimerEnded;
    }


    private void G01_Target_OnTargetMatched(Vector3 pos) {
        _currentScore++;
        _currentProjectileSpeed *= 1 + _adjustmentFactor;
        _currentProjectileDelay *= 1 - _adjustmentFactor;
        _currentTargetSpawnDelay *= 1 - _adjustmentFactor;
        OnGameStateChanged?.Invoke();
    }

    private void G01_Target_OnTargetNotMatched() {
        //TakeDamage();
    }

    private void G01_Target_OnTimerEnded(Vector3 vector) {
        TakeDamage();
    }

    private void TakeDamage() {
        _currentHP--;
        if (_currentHP <= 0) {
            ResetGame();
        } else {
            OnGameStateChanged?.Invoke();
        }
    }

    private void ResetGame() {
        _currentHP = _startHP;
        _currentScore = 0;        
        _currentProjectileSpeed = _startProjectileSpeed;
        _currentProjectileDelay = _startProjectileDelay;
        _currentTargetSpawnDelay = _startTargetSpawnDelay;
        _currentMinTargetTimer = _startMinTargetTimer;
        _currentMaxTargetTimer = _startMaxTargetTimer;

        OnGameStateChanged?.Invoke();
    }
}