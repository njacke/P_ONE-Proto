using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_Token : MonoBehaviour
{
    public static Action<G05_Token> OnTokenKill;
    public TokenType GetTokenType { get { return _tokenType; } }
    public G05_Field CurrentField { get; set; }
    public string GetTokenName { get { return _tokenName; } }
    public string GetTokenInfo { get { return _tokenInfo; } }

    [SerializeField] private string _tokenName;
    [SerializeField] private string _tokenInfo;
    [SerializeField] private TokenType _tokenType;
    [SerializeField] private SpriteRenderer _outlineSpriteRenderer;
    protected G05_Track _track;

    public enum TokenType {
        None,
        Player,
        Enemy
    }

    private void Start() {
        _track = G05_GameManager.Instance.GetTrack;
    }

    public void MoveToField(G05_Field targetField) {
        var newPos = new Vector3(targetField.transform.position.x, targetField.transform.position.y, 0f);
        var previousField = CurrentField;
        transform.position = newPos;
        CurrentField = targetField;
        if (targetField.CurrentToken != null) {
            if (_tokenType == TokenType.Player && targetField.CurrentToken.GetTokenType == TokenType.Enemy) {
                OnTokenKill?.Invoke(targetField.CurrentToken);
            } else if (_tokenType == TokenType.Enemy && targetField.CurrentToken.GetTokenType == TokenType.Player) {
                OnTokenKill?.Invoke(targetField.CurrentToken);
            }
        }

        var previousObject = previousField.GetComponentInChildren<G05_Object>();
        if (previousObject != null && previousObject is G05_IExitAction exitAction) {
            exitAction.ExitAction(this);
        }

        var newObject = targetField.GetComponentInChildren<G05_Object>();
        if (newObject != null && newObject is G05_IEnterAction enterAction) {
            enterAction.EnterAction(this);
        }

        targetField.CurrentToken = this;
        previousField.CurrentToken = null;
    }

    public void ToggleSelected(bool selected) {
        if (selected) {
            _outlineSpriteRenderer.color = Color.green;
        } else {
            _outlineSpriteRenderer.color = Color.black;
        }
    }
}
