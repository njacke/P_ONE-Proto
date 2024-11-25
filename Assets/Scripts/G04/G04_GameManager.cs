using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G04_GameManager : Singleton<G04_GameManager>
{
    public static Action<G04_GameManager> OnStateChanged;
    public float GetTotalScore { get {return _totalScore; } }
    public float GetRemainingTurns { get {return _remainingTurns; } }
    public float GetGoalScore { get {return _goalScore; } }
    public int GetRemainingCombines { get {return _remainingCombines; } }
    [SerializeField] private int _goalScore = 10000;
    [SerializeField] private int _remainingTurns = 10;
    [SerializeField] private int _maxCombines = 1;
    [SerializeField] private G04_Grid _storageGrid;
    private int _remainingCombines = 0;
    private float _totalScore = 0f;

    private void Start() {
        _remainingCombines = _maxCombines;
        OnStateChanged?.Invoke(this);
    }

    private void OnEnable() {
        G04_CombinedBlock.OnCurrentValueChanged += G04_CombinedBlock_OnCurrentValueChanged;
        G04_UI.OnTurnEnded += G04_UI_OnTurnEnded;
        G04_BlockManager.OnBlockCombined += G04_BlockManager_OnBlockCombined;
    }

    private void OnDisable() {
        G04_CombinedBlock.OnCurrentValueChanged -= G04_CombinedBlock_OnCurrentValueChanged;
        G04_UI.OnTurnEnded -= G04_UI_OnTurnEnded;
        G04_BlockManager.OnBlockCombined -= G04_BlockManager_OnBlockCombined;
    }

    private void G04_BlockManager_OnBlockCombined(G04_BlockManager manager) {
        _remainingCombines--;
        OnStateChanged?.Invoke(this);
    }

    private void G04_UI_OnTurnEnded() {
        _remainingTurns--;
        _remainingCombines = _maxCombines;
        OnStateChanged?.Invoke(this);

        if (_remainingTurns < 0) {
            Debug.Log("You lose.");
        }
    }

    private void G04_CombinedBlock_OnCurrentValueChanged(G04_CombinedBlock sender) {
        Debug.Log("On current value changed entered in Game Manager" + sender);
        _totalScore = 0f;
        var allCombinedBlocks = _storageGrid.GetAllCombinedBlocksOnGrid();        
        foreach (var combinedBlock in allCombinedBlocks) {
            _totalScore += combinedBlock.GetBlockCurrentValue;
        }

        Debug.Log("New score is: " + GetTotalScore);
        OnStateChanged?.Invoke(this);

        if (_totalScore > _goalScore) {
            Debug.Log("You win.");
        }
    }
}
