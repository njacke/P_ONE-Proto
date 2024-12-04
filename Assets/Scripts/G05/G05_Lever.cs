using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class G05_Lever : G05_Object, G05_IEnterAction
{
    [SerializeField] private int _gateIndex;
    [SerializeField] private SpriteRenderer _gateRenderer;
    [SerializeField] private Sprite _usedSprite;
    private G05_Field[] _gateFields;
    private bool _wasUsed = false;

    private void Start() {
        _gateFields = G05_GameManager.Instance.GetTrack.TrackFields.Where(x => x.IsGated && x.GetGateIndex == _gateIndex)
                                                                    .ToArray();
        Debug.Log(_gateFields.Length);
    }

    public void EnterAction(G05_Token tokenEntered) {
        if (!_wasUsed && tokenEntered.GetTokenType == G05_Token.TokenType.Player) {
            _wasUsed = true;
            GetComponent<SpriteRenderer>().sprite = _usedSprite;
            _gateRenderer.enabled = false;
            foreach (var field in _gateFields) {
                field.IsGated = false;
            }
        }
    }
}   
