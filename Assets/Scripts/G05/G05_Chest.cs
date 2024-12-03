using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class G05_Chest : G05_Object, G05_IEnterAction
{
    [SerializeField] private int _minItems = 2;
    [SerializeField] private int _maxItems = 4;
    [SerializeField] private Sprite _emptySprite;
    private bool _isOpened = false;

    public void EnterAction(G05_Token tokenEntered) {
        Debug.Log("Chest enter action called");
        if (_isOpened || tokenEntered.GetTokenType == G05_Token.TokenType.Player) {
            _isOpened = true;
            GetComponent<SpriteRenderer>().sprite = _emptySprite;
            var rndAmount = UnityEngine.Random.Range(_minItems, _maxItems + 1); // max exclusive
            for (int i = 0; i < rndAmount; i++) {
                G05_GameManager.Instance.CreateNewItem();
            }
        }
    }
}
