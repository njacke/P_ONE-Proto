using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class G04_UI : MonoBehaviour
{
    public static Action OnTurnEnded;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _goalText;
    [SerializeField] TextMeshProUGUI _turnText;
    [SerializeField] TextMeshProUGUI _combinesText;

    private void OnEnable() {
        G04_GameManager.OnStateChanged += G04_GameManager_OnStateChanged;
    }

    private void OnDisable() {
        G04_GameManager.OnStateChanged -= G04_GameManager_OnStateChanged;        
    }

    private void G04_GameManager_OnStateChanged(G04_GameManager sender) {
        var scoreText = "Total Score: " + sender.GetTotalScore.ToString();
        _scoreText.text = scoreText;       

        var goalText = "Goal Score: " + sender.GetGoalScore.ToString();
        _goalText.text = goalText;

        var turnText = "Turns Left: " + sender.GetRemainingTurns.ToString();
        _turnText.text = turnText;

        var combinesText = "Combines Left: " + sender.GetRemainingCombines.ToString();
        _combinesText.text = combinesText;
    }

    public void EndTurnOnClick() {
        OnTurnEnded?.Invoke();
    }
}
