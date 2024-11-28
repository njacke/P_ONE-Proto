using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_Token : MonoBehaviour
{
    public TokenType GetTokenType { get { return _tokenType; } }
    public G05_Field CurrentField { get; set; }

    [SerializeField] private TokenType _tokenType;
    [SerializeField] private SpriteRenderer _outlineSpriteRenderer;

    public enum TokenType {
        None,
        Player,
        Enemy
    }

    public void ToggleSelected(bool selected) {
        if (selected) {
            _outlineSpriteRenderer.color = Color.green;
        } else {
            _outlineSpriteRenderer.color = Color.black;
        }
    }
}
