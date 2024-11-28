using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class G05_BoardManager : MonoBehaviour
{
    public static Action<G05_BoardManager> OnSelect;
    public static Action<G05_BoardManager> OnPlayerMoved;

    public G05_Token SelectedToken { get; private set; }
    public G05_Field SelectedField { get; private set; }

    private G05_Field[] _eligibleFields;

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
    }

    private void OnDisable() {
        G05_BoardManager.OnSelect -= G05_BoardManager_OnSelect;        
        G05_BoardManager.OnPlayerMoved -= G05_BoardManager_OnPlayerMoved; 
        G05_GameManager.OnTurnStateChanged -= G05_GameManager_OnTurnStateChanged;        
    }

    private void G05_GameManager_OnTurnStateChanged(G05_GameManager sender) {
        Debug.Log("Updating eligible fields from OnTurnStateChanged");
        UpdateEligibleFields();
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
        Debug.Log("Updating eligible fields from OnSelect");
        UpdateEligibleFields();
    }

    private void UpdateEligibleFields() {
        if (G05_GameManager.Instance.GetTurnState != G05_GameManager.TurnState.Move) {
            return;
        }

        if (SelectedToken != null && SelectedToken.GetTokenType == G05_Token.TokenType.Player) {
            _eligibleFields = G05_GameManager.Instance.GetTrack.GetEligibleFields(SelectedToken);
            foreach (var field in _eligibleFields) {
                field.ToggleEligible(true);
            }
        } else if (_eligibleFields != null) {
            foreach (var field in _eligibleFields) {
                field.ToggleEligible(false);
            }
            _eligibleFields = null;
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
            if (hit.collider != null) {
                    hit.collider.TryGetComponent(out fieldHit);
            }
        }

        if (fieldHit != null && _eligibleFields.Contains(fieldHit)) {
            // z: 1f to avoid raycast detection issues
            var newPos = new Vector3(fieldHit.transform.position.x, fieldHit.transform.position.y, 1f);
            SelectedToken.transform.position = newPos;
            fieldHit.CurrentToken = SelectedToken;
            SelectedToken.CurrentField = fieldHit;
            OnPlayerMoved?.Invoke(this);
        } else {
            Debug.Log("Suggested move is not eligible.");
        }
    }

    private Vector3 GetMouseWorldPosition() {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
