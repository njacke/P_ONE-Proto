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
            _eligibleFields = null;
        }

        if (SelectedToken != null && SelectedToken.GetTokenType == G05_Token.TokenType.Player) {
            if (SelectedToken.CurrentField.GetFieldType == G05_Field.FieldType.Finish) {
                Debug.Log("Selected token has already finished");
                return;
            }

            var fields = _track.TrackFields.Where(x => x.GetFieldType == G05_Field.FieldType.Start
                                                    || x.GetFieldType == G05_Field.FieldType.Main
                                                    || x.GetFieldType == G05_Field.FieldType.Finish)
                                        .ToArray();
            var graph = _track.GetFieldGraph(fields);
            var diceValue = G05_GameManager.Instance.GetDice.TotalValue;
            var startField = SelectedToken.CurrentField;
            if (startField.GetFieldType == G05_Field.FieldType.Start) {
                startField = fields.FirstOrDefault(x => x.FieldIndex == 1);
                diceValue--;
            }

            var eligibleFields = _track.GetFieldsByDistance(graph, startField, diceValue, true, true).ToList();

            // additional finish fields (don't need exact move to finish)
            var finishFields = _track.GetFieldsByDistance(graph, startField, diceValue, false, true)
                                    .Where(x => x.GetFieldType == G05_Field.FieldType.Finish && !eligibleFields.Contains(x))
                                    .ToList();

            eligibleFields.AddRange(finishFields);;

            // if any finish fields are within range add all that are empty
            if (eligibleFields.Count(x => x.GetFieldType == G05_Field.FieldType.Finish) > 0) {
                var allFinishFields = _track.TrackFields.Where(x => x.GetFieldType == G05_Field.FieldType.Finish);
                foreach (var field in allFinishFields) {
                    if (!eligibleFields.Contains(field)) {
                        eligibleFields.Add(field);
                    }
                }
            }

            // remove fields occupied by player tokens
            eligibleFields = eligibleFields.Where(x => x.CurrentToken == null || x.CurrentToken.GetTokenType != G05_Token.TokenType.Player).ToList();

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
            Debug.Log("Roll the dice first.");
            return;
        }

        if (SelectedToken == null || _eligibleFields.Length == 0) {
            Debug.Log("No token selected or no eligible move possible.");
            return;
        }

        if (SelectedToken.GetTokenType != G05_Token.TokenType.Player) {
            Debug.Log("Selected token is not controllable");
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
            Debug.Log("Suggested move is not eligible.");
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

        Debug.Log("Enemy turn ended.");
        OnEnemyTurnResolved?.Invoke(this);
    }

    private Vector3 GetMouseWorldPosition() {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
