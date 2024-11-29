using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_GameManager : Singleton<G05_GameManager>
{
    public static Action<G05_GameManager> OnTurnStateChanged;

    public HashSet<G05_Token> AllPlayers { get; private set; }
    public HashSet<G05_Enemy> AllEnemies { get; private set; }
    public G05_Dice GetDice { get { return _dice; } }
    public G05_Track GetTrack { get { return _track; } }
    public TurnState GetTurnState { get { return _currentTurnState; } }
    [SerializeField] private G05_Dice _dice;
    [SerializeField] private G05_Track _track;
    private TurnState _currentTurnState;
    public enum TurnState {
        None,
        Roll,
        Move,
        Enemy
    }

    protected override void Awake() {
        base.Awake();
        _currentTurnState = TurnState.Roll;
        AllPlayers = new HashSet<G05_Token>();
        AllEnemies = new HashSet<G05_Enemy>();
    }

    private void OnEnable() {
        G05_Dice.OnValueUpdated += G05_Dice_OnValueUpdated;        
        G05_BoardManager.OnPlayerMoved += G05_BoardManager_OnPlayerMoved;
        G05_BoardManager.OnEnemyTurnResolved += G05_BoardManager_OnEnemyTurnResolved;
        G05_TokenSpawner.OnEnemySpawned += G05_TokenSpawner_OnEnemySpawned;
        G05_TokenSpawner.OnPlayerSpawned += G05_TokenSpawner_OnPlayerSpawned;
        G05_Token.OnTokenKill += G05_Token_OnTokenKill;
    }

    private void OnDisable() {
        G05_Dice.OnValueUpdated -= G05_Dice_OnValueUpdated;        
        G05_BoardManager.OnPlayerMoved -= G05_BoardManager_OnPlayerMoved;        
        G05_BoardManager.OnEnemyTurnResolved -= G05_BoardManager_OnEnemyTurnResolved;
        G05_TokenSpawner.OnEnemySpawned -= G05_TokenSpawner_OnEnemySpawned;
        G05_TokenSpawner.OnPlayerSpawned -= G05_TokenSpawner_OnPlayerSpawned;
        G05_Token.OnTokenKill -= G05_Token_OnTokenKill;
    }

    private void G05_Token_OnTokenKill(G05_Token tokenKilled) {
        if (tokenKilled.GetTokenType == G05_Token.TokenType.Player) {
            AllPlayers.Remove(tokenKilled);
            Debug.Log(tokenKilled.gameObject.name + " was killed.");
            Destroy(tokenKilled.gameObject);
        } else if (tokenKilled.GetTokenType == G05_Token.TokenType.Enemy) {
            var enemy = tokenKilled.GetComponent<G05_Enemy>();
            if (enemy != null) {
                AllEnemies.Remove(tokenKilled.GetComponent<G05_Enemy>());
                Debug.Log(tokenKilled.gameObject.name + " was killed.");
                Destroy(enemy.gameObject);
            } else {
                Debug.Log("Enemy component not found. " + tokenKilled.gameObject.name + " was not killed.");
            }
        }
    }

    private void G05_TokenSpawner_OnPlayerSpawned(G05_Token player) {
        AllPlayers.Add(player);
    }

    private void G05_TokenSpawner_OnEnemySpawned(G05_Enemy enemy) {
        AllEnemies.Add(enemy);
    }

    private void G05_Dice_OnValueUpdated(G05_Dice sender) {
        _currentTurnState = TurnState.Move;
        OnTurnStateChanged?.Invoke(this);
    }

    private void G05_BoardManager_OnPlayerMoved(G05_BoardManager sender) {
        _currentTurnState = TurnState.Enemy;      
        OnTurnStateChanged?.Invoke(this);
    }

    private void G05_BoardManager_OnEnemyTurnResolved(G05_BoardManager sender) {
        _currentTurnState = TurnState.Roll;
        OnTurnStateChanged?.Invoke(this);        
    }
}
