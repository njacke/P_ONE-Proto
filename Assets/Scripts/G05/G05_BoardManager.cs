using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class G05_BoardManager : MonoBehaviour
{
    public static Action<G05_BoardManager> OnSelect;
    public static Action<G05_BoardManager> OnPlayerMoved;
    public static Action<G05_BoardManager> OnEnemyTurnResolved;

    public G05_Token SelectedToken { get; private set; }
    public G05_Field SelectedField { get; private set; }

    private G05_Field[] _eligibleFields;
    private G05_Track _track;

    private void Start() {
        _track = G05_GameManager.Instance.GetTrack;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Select();
        }

        if (Input.GetMouseButtonDown(1)) {
            Move();
        }
    }

    private void OnEnable() {
        G05_BoardManager.OnSelect += G05_BoardManager_OnSelect; 
        G05_BoardManager.OnPlayerMoved += G05_BoardManager_OnPlayerMoved; 
        G05_GameManager.OnTurnStateChanged += G05_GameManager_OnTurnStateChanged;        
        G05_UI.OnUseSelectedItems += G05_UI_OnUseSelectedItems;
    }

    private void OnDisable() {
        G05_BoardManager.OnSelect -= G05_BoardManager_OnSelect;        
        G05_BoardManager.OnPlayerMoved -= G05_BoardManager_OnPlayerMoved; 
        G05_GameManager.OnTurnStateChanged -= G05_GameManager_OnTurnStateChanged;        
        G05_UI.OnUseSelectedItems -= G05_UI_OnUseSelectedItems;
    }


    private void G05_UI_OnUseSelectedItems() {
        UpdateEligibleFields();
    }

    private void G05_GameManager_OnTurnStateChanged(G05_GameManager sender) {
        UpdateEligibleFields();
        if (sender.GetTurnState == G05_GameManager.TurnState.Enemy) {
            ResolveEnemyTurn();
        }
    }

    private void G05_BoardManager_OnPlayerMoved(G05_BoardManager sender) {
        if (_eligibleFields != null) {
            foreach (var field in _eligibleFields) {
                field.ToggleEligible(false);
            }
            _eligibleFields = null;
        }
    }

    private void G05_BoardManager_OnSelect(G05_BoardManager sender) {
        UpdateEligibleFields();
    }

    private void UpdateEligibleFields() {
        if (G05_GameManager.Instance.GetTurnState != G05_GameManager.TurnState.Move) {
            return;
        }

        // cleanup
        if (_eligibleFields != null) {
            foreach (var field in _eligibleFields) {
                field.ToggleEligible(false);
            }
            G05_GameManager.Instance.GetDice.UpdateSpecialValue(0);
            _eligibleFields = null;
        }

        if (SelectedToken != null && SelectedToken.GetTokenType == G05_Token.TokenType.Player) {
            var player = SelectedToken.GetComponent<G05_Player>();
            if (player == null) {
                Debug.Log("SelectedToken is categorised as a player but doesn't have a player component.");
                return;
            }

            if (player.CurrentField.GetFieldType == G05_Field.FieldType.Finish) {
                G05_GameManager.Instance.GetGameLog.UpdateLog("Selected token has already finished");
                return;
            }


            // SPECIAL VALUE (players specials & field speed)
            var specialValue = player.CurrentField.GetFieldSpeed;
            
            // hardcoded UNDERDOG skill for prototype (+3 bonus if base is 1)
            if (G05_GameManager.Instance.GetDice.BaseValue == 1 && player.GetPlayerType == G05_Player.PlayerType.Underdog) {
                specialValue += 3;
            }

            var fields = _track.TrackFields.Where(x => x.GetFieldType == G05_Field.FieldType.Start
                                                    || x.GetFieldType == G05_Field.FieldType.Main
                                                    || x.GetFieldType == G05_Field.FieldType.Finish)
                                        .ToArray();
            var graph = _track.GetFieldGraph(fields);

            // hardcoded PERFORMER skill for prototype (+1 bonus for each unit in range on the track)
            if (player.GetPlayerType == G05_Player.PlayerType.Performer) {
                int bonus = 0;
                int minRange = 3;
                var checkStartField = player.CurrentField;

                // adjust values if player is on start
                if (checkStartField.GetFieldType == G05_Field.FieldType.Start) {
                    checkStartField = fields.FirstOrDefault(x => x.GetFieldIndex == 1);
                    minRange--;
                }

                HashSet<G05_Field> fieldsInRange = _track.GetFieldsByDistance(graph, checkStartField, minRange, false, false, true).ToHashSet();

                // add start fields if start is in range
                if (fieldsInRange.Where(x => x.GetFieldType == G05_Field.FieldType.Start).FirstOrDefault() != null) {
                    var startFields = _track.TrackFields.Where(x => x.GetFieldType == G05_Field.FieldType.Start);
                    foreach (var field in startFields) {
                            fieldsInRange.Add(field);                    
                    }
                }

                foreach (var field in fieldsInRange) {
                    if (field.CurrentToken != null && field.CurrentToken != player) {
                        Debug.Log("Player field is: " + checkStartField.gameObject.name);
                        Debug.Log("Perfomer bonus added for field: " + field.gameObject.name);
                        bonus++;
                    }
                }     

                specialValue += bonus;                          
            }

            G05_GameManager.Instance.GetDice.UpdateSpecialValue(specialValue);
            var diceValue = G05_GameManager.Instance.GetDice.TotalValue;
            
            var startField = player.CurrentField;

            // adjust values if player is on start
            if (startField.GetFieldType == G05_Field.FieldType.Start) {
                startField = fields.FirstOrDefault(x => x.GetFieldIndex == 1);
                diceValue--;
            }

            // hardcoded MOONWALKER skill for prototype (can move backwards)
            bool isMoonwalker = player.GetPlayerType == G05_Player.PlayerType.Moonwalker;

            HashSet<G05_Field> eligibleFields = _track.GetFieldsByDistance(graph, startField, diceValue, true, !isMoonwalker, false).ToHashSet();

            // additional finish fields (don't need exact move to finish)
            HashSet<G05_Field> finishFields = _track.GetFieldsByDistance(graph, startField, diceValue, false, !isMoonwalker, false)
                                    .Where(x => x.GetFieldType == G05_Field.FieldType.Finish)
                                    .ToHashSet();

            eligibleFields.UnionWith(finishFields);

            // if any finish fields are within range add all
            if (eligibleFields.FirstOrDefault(x => x.GetFieldType == G05_Field.FieldType.Finish) != null) {
                HashSet<G05_Field> allFinishFields = _track.TrackFields.Where(x => x.GetFieldType == G05_Field.FieldType.Finish).ToHashSet();
                foreach (var field in allFinishFields) {
                    eligibleFields.Add(field);
                }
            }

            // remove start fields (can't move to start -> moonwalker edge case)
            eligibleFields = eligibleFields.Where(x => x.GetFieldType != G05_Field.FieldType.Start).ToHashSet();

            // remove fields occupied by player tokens (can't move to another player's pos)
            eligibleFields = eligibleFields.Where(x => x.CurrentToken == null || x.CurrentToken.GetTokenType != G05_Token.TokenType.Player).ToHashSet();

            _eligibleFields = eligibleFields.ToArray();

            foreach (var field in _eligibleFields) {
                field.ToggleEligible(true);
            }
        }
    }

    private void Select() {
        RaycastHit2D[] hits = Physics2D.RaycastAll(GetMouseWorldPosition(), Vector2.zero);

        G05_Token tokenHit = null;
        G05_Field fieldHit = null;

        foreach (var hit in hits) {
            if (hit.collider != null) {
                if (!hit.collider.TryGetComponent(out tokenHit)) {
                    hit.collider.TryGetComponent(out fieldHit);
                }
            }
        }

        // only select one per click; token has priority
        if (tokenHit != null) {
            if (SelectedToken == tokenHit) {
                SelectedToken.ToggleSelected(false);
                SelectedToken = null;
            } else {
                if (SelectedToken != null) {
                    SelectedToken.ToggleSelected(false);
                    SelectedToken = null;
                }

                SelectedToken = tokenHit;
                SelectedToken.ToggleSelected(true);
                
                if (SelectedField != null) {
                    SelectedField.ToggleSelected(false);
                    SelectedField = null;
                }
            }

            OnSelect?.Invoke(this);

        } else if (fieldHit != null) {
            if (SelectedField == fieldHit) {
                SelectedField.ToggleSelected(false);
                SelectedField = null;
            } else {
                if (SelectedField != null) {
                    SelectedField.ToggleSelected(false);
                    SelectedField = null;
                }

                SelectedField = fieldHit;
                SelectedField.ToggleSelected(true);
                
                if (SelectedToken != null) {
                    SelectedToken.ToggleSelected(false);
                    SelectedToken = null;
                }
            }

            OnSelect?.Invoke(this);
        }
    }

    private void Move() {
        if (_eligibleFields == null) {
            Debug.Log("No token selected or no eligible move possible.");
            return;
        }

        if (G05_GameManager.Instance.GetTurnState != G05_GameManager.TurnState.Move && SelectedToken != null) {
            G05_GameManager.Instance.GetGameLog.UpdateLog("Roll the dice first.");
            return;
        }

        if (SelectedToken == null || _eligibleFields.Length == 0) {
            G05_GameManager.Instance.GetGameLog.UpdateLog("No token selected or no eligible move possible.");
            return;
        }

        if (SelectedToken.GetTokenType != G05_Token.TokenType.Player) {
            G05_GameManager.Instance.GetGameLog.UpdateLog("Selected token is not controllable");
            return;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(GetMouseWorldPosition(), Vector2.zero);
        G05_Field fieldHit = null;

        foreach (var hit in hits) {
            if (hit.collider != null && hit.collider.TryGetComponent(out G05_Field field)) {
                    fieldHit = field;
                    break;
            }
        }

        if (fieldHit != null && _eligibleFields.Contains(fieldHit)) {
            SelectedToken.MoveToField(fieldHit);
            OnPlayerMoved?.Invoke(this);
        } else {
            G05_GameManager.Instance.GetGameLog.UpdateLog("Suggested move is not eligible.");
        }
    }

    private void ResolveEnemyTurn() {
        var enemies = G05_GameManager.Instance.AllEnemies;

        int lastActionsCount = int.MaxValue;

        while (lastActionsCount > 0) {
            int actionsCount = 0;
            foreach (var enemy in enemies) {
                if(enemy.TakeAction()) {
                    actionsCount++;
                };
            }
            lastActionsCount = actionsCount;
        }

        foreach (var enemy in enemies) {
            enemy.ActionAvailable = true;
            //Debug.Log("Setting enemy aciton available to true");
        }

        G05_GameManager.Instance.GetGameLog.UpdateLog("Enemy turn ended.");
        OnEnemyTurnResolved?.Invoke(this);
    }

    private Vector3 GetMouseWorldPosition() {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
