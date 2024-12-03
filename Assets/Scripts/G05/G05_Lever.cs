using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class G05_Lever : G05_Object, G05_IEnterAction, G05_IExitAction
{
    [SerializeField] private int _shortcutIndex;
    [SerializeField] private SpriteRenderer _gateRenderer;
    private G05_Field[] _shortcutFields;

    private void Start() {
        _shortcutFields = G05_GameManager.Instance.GetTrack.TrackFields.Where(x => x.GetFieldType == G05_Field.FieldType.Shortcut
                                                                                && x.GetShortcutIndex == _shortcutIndex).ToArray();

        Debug.Log(_shortcutFields.Length);
    }

    public void EnterAction(G05_Token tokenEntered) {
        if (tokenEntered.GetTokenType == G05_Token.TokenType.Player) {
            _gateRenderer.enabled = false;
            foreach (var field in _shortcutFields) {
                field.ToggleShortcut(true);
            }
        } else if (tokenEntered.GetTokenType == G05_Token.TokenType.Enemy) {
            _gateRenderer.enabled = true;
            foreach (var field in _shortcutFields) {
                field.ToggleShortcut(false);
            }
        }
    }

    public void ExitAction(G05_Token tokenExited) {
        _gateRenderer.enabled = true;
        foreach (var field in _shortcutFields) {
            field.ToggleShortcut(false);
        }
    }
}
