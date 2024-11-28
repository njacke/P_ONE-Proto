using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_GameManager : Singleton<G05_GameManager>
{
    public static Action<G05_GameManager> OnTurnStateChanged;

    public G05_Dice GetDice { get { return _dice; } }
    public G05_Track GetTrack { get { return _track; } }
    public TurnState GetTurnState { get { return _currentTurnState; } }
    [SerializeField] private G05_Dice _dice;
    [SerializeField] private G05_Track _track;
    private TurnState _currentTurnState;
    public enum TurnState {
        None,
        Roll,
        Move
    }

    protected override void Awake() {
        base.Awake();
        _currentTurnState = TurnState.Roll;
    }

    private void OnEnable() {
        G05_Dice.OnValueUpdated += G05_Dice_OnValueUpdated;        
        G05_BoardManager.OnPlayerMoved += G05_BoardManager_OnPlayerMoved;
    }

    private void OnDisable() {
        G05_Dice.OnValueUpdated -= G05_Dice_OnValueUpdated;        
        G05_BoardManager.OnPlayerMoved -= G05_BoardManager_OnPlayerMoved;        
    }

    private void G05_Dice_OnValueUpdated(G05_Dice dice) {
        _currentTurnState = TurnState.Move;
        OnTurnStateChanged?.Invoke(this);
    }

    private void G05_BoardManager_OnPlayerMoved(G05_BoardManager manager) {
        _currentTurnState = TurnState.Roll;      
        OnTurnStateChanged?.Invoke(this);
    }
}
