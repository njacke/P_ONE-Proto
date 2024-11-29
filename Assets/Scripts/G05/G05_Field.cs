using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_Field : MonoBehaviour
{
    public FieldType GetFieldType { get { return _fieldType; } }
    public G05_Token CurrentToken { get; set; }
    public G05_Field[] AdjacentFields { get; set; }
    public int FieldIndex { get {return _fieldIndex; } }

    [SerializeField] private int _fieldIndex;
    [SerializeField] private FieldType _fieldType;
    [SerializeField] private SpriteRenderer _outlineSpriteRenderer;
    private SpriteRenderer _spriteRenderer;
    private Color _startColor;

    public enum FieldType {
        None,
        Start,
        Finish,
        Main,
        Shortcut,
        Enemy,
    }

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startColor = _spriteRenderer.color;
    }


    public void ToggleSelected(bool selected) {
        if (selected) {
            _outlineSpriteRenderer.color = Color.green;
        } else {
            _outlineSpriteRenderer.color = Color.black;
        }
    }

    public void ToggleEligible(bool eligible) {
        if (eligible) {
            _spriteRenderer.color = Color.yellow;
        } else {
            _spriteRenderer.color = _startColor;
        }        
    }
}
