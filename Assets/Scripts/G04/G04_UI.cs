using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class G04_UI : MonoBehaviour
{
    public static Action OnTurnEnded;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _goalText;
    [SerializeField] TextMeshProUGUI _turnText;
    [SerializeField] TextMeshProUGUI _blocksText;
    [SerializeField] TextMeshProUGUI _combinesText;
    [SerializeField] TextMeshProUGUI _block1Text;
    [SerializeField] TextMeshProUGUI _block2Text;

    private void OnEnable() {
        G04_GameManager.OnStateChanged += G04_GameManager_OnStateChanged;
        G04_BlockManager.OnSelectionChanged += G04_BlockManager_OnSelectionChanged;
    }

    private void OnDisable() {
        G04_GameManager.OnStateChanged -= G04_GameManager_OnStateChanged;        
        G04_BlockManager.OnSelectionChanged -= G04_BlockManager_OnSelectionChanged;
    }

    private void G04_BlockManager_OnSelectionChanged(G04_BlockManager sender) {
        var selectedBlocks = sender.GetSelectedCombinedBlocks;
        if (selectedBlocks.Count == 0) {
            _block1Text.text = "Block #1 Info";
            _block2Text.text = "Block #2 Info";
            return;
        }
        
        if (selectedBlocks.Count > 0) {
            var block = selectedBlocks[0];

            string blockInfo = 
                $"Block #1 Info\n" +
                $"Tier: {block.GetBlockTier}\n" +
                $"Type: {block.GetBlockType}\n" +
                $"Size: {block.GetBlockSize}\n" +
                $"Level: {block.GetBlockLevel}\n" +
                $"Base Value: {block.GetBlockBaseValue}\n" +
                $"Bonus Value: {block.GetBlockBonusValue}\n" +
                $"Multiplier: {block.GetBlockMultiplier}\n" +
                $"Current Value: {Mathf.FloorToInt(block.GetBlockTotalValue)}";
            
            _block1Text.text = blockInfo;
        }

        if (selectedBlocks.Count > 1) {
            var block = selectedBlocks[1];

            string blockInfo = 
                $"Block #2 Info\n" +
                $"Tier: {block.GetBlockTier}\n" +
                $"Type: {block.GetBlockType}\n" +
                $"Size: {block.GetBlockSize}\n" +
                $"Level: {block.GetBlockLevel}\n" +
                $"Base Value: {block.GetBlockBaseValue}\n" +
                $"Bonus Value: {block.GetBlockBonusValue}\n" +
                $"Multiplier: {block.GetBlockMultiplier}\n" +
                $"Current Value: {Mathf.FloorToInt(block.GetBlockTotalValue)}";
            
            _block2Text.text = blockInfo;
        } else {
            _block2Text.text = "Block #2 Info";
        }
    }

    private void G04_GameManager_OnStateChanged(G04_GameManager sender) {
        var scoreText = "Total Score: " + Mathf.FloorToInt(sender.GetTotalScore).ToString();
        _scoreText.text = scoreText;       

        var goalText = "Goal Score: " + sender.GetGoalScore.ToString();
        _goalText.text = goalText;

        var turnText = "Turns Left: " + sender.GetRemainingTurns.ToString();
        _turnText.text = turnText;

        var combinesText = "Combines Left (turn): " + sender.GetRemainingCombines.ToString();
        _combinesText.text = combinesText;

        var blocksText = "Blocks Left (turn): " + G04_GameManager.Instance.GetRemainingBlocks.ToString();
        _blocksText.text = blocksText; 
    }

    public void EndTurnOnClick() {
        EventSystem.current.SetSelectedGameObject(null);
        OnTurnEnded?.Invoke();
    }
}
