using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class G05_GameManager : Singleton<G05_GameManager>
{
    public static Action<G05_GameManager> OnTurnStateChanged;
    public static Action<G05_GameManager> OnPlayerTokenDeath;
    public static Action<G05_ItemEffect> OnNewItemCreated;

    public HashSet<G05_Player> AllPlayers { get; private set; }
    public HashSet<G05_Enemy> AllEnemies { get; private set; }
    public G05_Dice GetDice { get { return _dice; } }
    public G05_Track GetTrack { get { return _track; } }
    public TurnState GetTurnState { get { return _currentTurnState; } }
    public int GetMaxItems { get { return _maxItems; } }
    public int GetCurrentTurnCount { get { return _currentTurnCount; } }
    public int GetPlayersAliveCount { get { return CalcPlayersAlive(); } }
    public GameLog GetGameLog { get { return _gameLog; } }

    [SerializeField] private G05_Dice _dice;
    [SerializeField] private G05_Track _track;
    [SerializeField] private int _maxItems = 5;
    [SerializeField] private GameLog _gameLog;

    private Type[] _allEffectTypes;
    private TurnState _currentTurnState;
    private int _currentTurnCount = 1;

    public enum TurnState {
        None,
        Roll,
        Move,
        Enemy
    }

    protected override void Awake() {
        base.Awake();
        _currentTurnState = TurnState.Roll;
        AllPlayers = new HashSet<G05_Player>();
        AllEnemies = new HashSet<G05_Enemy>();

        // add all effect types
        _allEffectTypes = new Type[] {
            typeof(G05_IE_AddedValue),
            typeof(G05_IE_RollValue),
            typeof(G05_IE_RollBonus)
        };
    }

    private void Start() {
        Time.timeScale = 1f;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("PlayMenu");
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }

    private void OnEnable() {
        G05_Dice.OnDiceRoll += G05_Dice_OnDiceRoll;        
        G05_BoardManager.OnPlayerMoved += G05_BoardManager_OnPlayerMoved;
        G05_BoardManager.OnEnemyTurnResolved += G05_BoardManager_OnEnemyTurnResolved;
        G05_TokenSpawner.OnEnemySpawned += G05_TokenSpawner_OnEnemySpawned;
        G05_TokenSpawner.OnPlayerSpawned += G05_TokenSpawner_OnPlayerSpawned;
        G05_Token.OnTokenKill += G05_Token_OnTokenKill;
        G05_UI.OnItemInitDone += G05_UI_OnItemInitDone;

    }

    private void OnDisable() {
        G05_Dice.OnDiceRoll -= G05_Dice_OnDiceRoll;        
        G05_BoardManager.OnPlayerMoved -= G05_BoardManager_OnPlayerMoved;        
        G05_BoardManager.OnEnemyTurnResolved -= G05_BoardManager_OnEnemyTurnResolved;
        G05_TokenSpawner.OnEnemySpawned -= G05_TokenSpawner_OnEnemySpawned;
        G05_TokenSpawner.OnPlayerSpawned -= G05_TokenSpawner_OnPlayerSpawned;
        G05_Token.OnTokenKill -= G05_Token_OnTokenKill;
        G05_UI.OnItemInitDone -= G05_UI_OnItemInitDone;
    }

    private void G05_UI_OnItemInitDone() {
        // hardcoded hoarder skill for prototype (start with +3 items)
        foreach (var player in AllPlayers) {
            if (player.GetPlayerType == G05_Player.PlayerType.Hoarder) {
                for (int i = 0; i < 3; i++) {
                    CreateNewItem();
                }
            }
        }
    }

    private void G05_Token_OnTokenKill(G05_Token tokenKilled) {
        if (tokenKilled.GetTokenType == G05_Token.TokenType.Player) {
            var player = tokenKilled.GetComponent<G05_Player>();
            if (player != null) {
                AllPlayers.Remove(player);
                Debug.Log(tokenKilled.gameObject.name + " was killed.");
                Destroy(tokenKilled.gameObject);
                OnPlayerTokenDeath?.Invoke(this);
            } else {
                Debug.Log("Player component not found. " + tokenKilled.gameObject.name + " was not killed.");
            }

        } else if (tokenKilled.GetTokenType == G05_Token.TokenType.Enemy) {
            var enemy = tokenKilled.GetComponent<G05_Enemy>();
            if (enemy != null) {
                AllEnemies.Remove(tokenKilled.GetComponent<G05_Enemy>());
                Debug.Log(tokenKilled.gameObject.name + " was killed.");
                Destroy(enemy.gameObject);
                CreateNewItem(); // loot drop
            } else {
                Debug.Log("Enemy component not found. " + tokenKilled.gameObject.name + " was not killed.");
            }
        }
    }

    private void G05_TokenSpawner_OnPlayerSpawned(G05_Player player) {
        AllPlayers.Add(player);
    }

    private void G05_TokenSpawner_OnEnemySpawned(G05_Enemy enemy) {
        AllEnemies.Add(enemy);
    }

    private void G05_Dice_OnDiceRoll(G05_Dice sender) {
        _currentTurnState = TurnState.Move;
        OnTurnStateChanged?.Invoke(this);
    }

    private void G05_BoardManager_OnPlayerMoved(G05_BoardManager sender) {
        _currentTurnState = TurnState.Enemy;      
        OnTurnStateChanged?.Invoke(this);
    }

    private void G05_BoardManager_OnEnemyTurnResolved(G05_BoardManager sender) {
        bool isFinished = true;
        int playersFinished = 0;

        foreach (var player in AllPlayers) {
            if (player != null) {
                if (player.CurrentField.GetFieldType == G05_Field.FieldType.Finish) {
                playersFinished++;
                } else {
                    isFinished = false;
                }
            }
        }

        if (isFinished) {
            if (playersFinished > 0 ) {
                _gameLog.UpdateLog("Player won with " + playersFinished + " tokens in " + _currentTurnCount + " turns.");
            } else {
                _gameLog.UpdateLog("Player lost.");
            }
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        } else {
            _currentTurnCount++;
            _currentTurnState = TurnState.Roll;
            OnTurnStateChanged?.Invoke(this);
        }
    }

    private G05_ItemEffect GetNewItemEffect() {
        int rndIndex = UnityEngine.Random.Range(0, _allEffectTypes.Length);
        Type selectedType = _allEffectTypes[rndIndex];

        G05_ItemEffect newItem = (G05_ItemEffect)Activator.CreateInstance(selectedType);

        return newItem;
    }

    public void CreateNewItem() {
            var newEffect = GetNewItemEffect();
            OnNewItemCreated?.Invoke(newEffect);
    }

    private int CalcPlayersAlive() {
        int count = 0;
        foreach (var player in AllPlayers) {
            if (player != null) {
                count++;
            }
        }       
        
        return count;
    }
}
