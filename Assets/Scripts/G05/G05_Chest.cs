using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class G05_Chest : G05_Object, G05_IEnterAction
{
    [SerializeField] private int _minItems = 2;
    [SerializeField] private int _maxItems = 4;
    [SerializeField] private Sprite _usedSprite;
    private bool _wasUsed = false;

    public void EnterAction(G05_Token tokenEntered) {
        Debug.Log("Chest enter action called");
        if (!_wasUsed && tokenEntered.GetTokenType == G05_Token.TokenType.Player) {
            _wasUsed = true;
            GetComponent<SpriteRenderer>().sprite = _usedSprite;
            
            var rndAmount = UnityEngine.Random.Range(_minItems, _maxItems + 1); // max exclusive

            // hardcoded HOARDER ability (+1 loot in each chest)
            var player = tokenEntered.GetComponent<G05_Player>();
            if (player != null && player.GetPlayerType == G05_Player.PlayerType.Hoarder) {
                rndAmount++;
            }

            for (int i = 0; i < rndAmount; i++) {
                G05_GameManager.Instance.CreateNewItem();
            }
        }
    }
}
