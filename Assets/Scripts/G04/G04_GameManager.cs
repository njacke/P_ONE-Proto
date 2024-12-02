using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class G04_GameManager : Singleton<G04_GameManager>
{
    public static Action<G04_GameManager> OnStateChanged;
    public float GetTotalScore { get {return _totalScore; } }
    public float GetRemainingTurns { get {return _remainingTurns; } }
    public float GetGoalScore { get {return _goalScore; } }
    public int GetRemainingCombines { get {return _remainingCombines; } }
    public int GetRemainingBlocks { get {return _remainingBlocks; } }
    public G04_Grid GetGrid { get {return _grid; } }

    [SerializeField] private int _goalScore = 10000;
    [SerializeField] private int _remainingTurns = 10;
    [SerializeField] private int _maxCombines = 1;
    [SerializeField] private int _maxBlocksStart = 3;
    [SerializeField] private int _maxBlocksTurn = 1;
    [SerializeField] private float _reloadDelay = 1f;

    [SerializeField] private G04_Grid _grid;
    private int _remainingCombines = 0;
    private int _remainingBlocks = 0;
    private float _totalScore = 0f;

    private void Start() {
        _remainingCombines = _maxCombines;
        _remainingBlocks = _maxBlocksStart;
        OnStateChanged?.Invoke(this);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("PlayMenu");
        }

        if (Input.GetKeyDown(KeyCode.F5)) {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }

    private void OnEnable() {
        G04_CombinedBlock.OnValuesChanged += G04_CombinedBlock_OnValuesChanged;
        G04_UI.OnTurnEnded += G04_UI_OnTurnEnded;
        G04_BlockManager.OnBlockCombined += G04_BlockManager_OnBlockCombined;
        G04_BlockManager.OnStartBlockPlaced += G04_BlockManager_OnStartBlockPlaced;
    }

    private void OnDisable() {
        G04_CombinedBlock.OnValuesChanged -= G04_CombinedBlock_OnValuesChanged;
        G04_UI.OnTurnEnded -= G04_UI_OnTurnEnded;
        G04_BlockManager.OnBlockCombined -= G04_BlockManager_OnBlockCombined;
        G04_BlockManager.OnStartBlockPlaced -= G04_BlockManager_OnStartBlockPlaced;
    }

    private void G04_BlockManager_OnStartBlockPlaced(G04_BlockManager manager) {
        _remainingBlocks--;
        OnStateChanged?.Invoke(this);
    }

    private void G04_BlockManager_OnBlockCombined(G04_BlockManager manager) {
        _remainingCombines--;
        OnStateChanged?.Invoke(this);
    }

    private void G04_UI_OnTurnEnded() {
        _remainingTurns--;
        _remainingCombines = _maxCombines;
        _remainingBlocks = _maxBlocksTurn;

        if (_totalScore > _goalScore) {
            GameLog.Instance.UpdateLog("YOU WIN!");
            StartCoroutine(ReloadActiveSceneRoutine());
        } else if (_remainingTurns <= 0) {
            GameLog.Instance.UpdateLog("YOU LOSE");
            StartCoroutine(ReloadActiveSceneRoutine());
        }

        OnStateChanged?.Invoke(this);

    }

    private void G04_CombinedBlock_OnValuesChanged(G04_CombinedBlock sender) {
        //Debug.Log("On current value changed entered in Game Manager" + sender);
        _totalScore = 0f;
        var allCombinedBlocks = _grid.GetAllCombinedBlocksOnGrid();        
        foreach (var combinedBlock in allCombinedBlocks) {
            _totalScore += combinedBlock.GetBlockTotalValue;
        }

       // Debug.Log("New score is: " + GetTotalScore);
        OnStateChanged?.Invoke(this);
    }

    private IEnumerator ReloadActiveSceneRoutine() {
        yield return new WaitForSeconds(_reloadDelay);
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
