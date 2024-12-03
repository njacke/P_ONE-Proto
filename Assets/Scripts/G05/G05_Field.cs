using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_Field : MonoBehaviour
{
    public FieldType GetFieldType { get { return _fieldType; } }
    public G05_Token CurrentToken { get; set; }
    public G05_Field[] AdjacentFields { get; set; }
    public int GetFieldIndex { get {return _fieldIndex; } }
    public string GetFieldName { get { return _fieldName; } }
    public string GetFieldInfo { get { return _fieldInfo; } }
    public int GetFieldSpeed { get { return _fieldSpeed; } }
    public int GetShortcutIndex { get { return _shortcutIndex; } }
    public bool IsShortcutEnabled { get; private set; } = false;

    [SerializeField] private string _fieldName;
    [SerializeField] private string _fieldInfo;
    [SerializeField] private int _fieldIndex;
    [SerializeField] private int _fieldSpeed;
    [SerializeField] private FieldType _fieldType;
    [SerializeField] private int _shortcutIndex;
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

    public void ToggleShortcut(bool isEnabled) {
        if (_fieldType != FieldType.Shortcut) {
            Debug.Log("Tried to enable shortcut on a field that is not a shortcut.");
            return;
        }

        if (isEnabled) {
            IsShortcutEnabled = true;
        } else {
            IsShortcutEnabled = false;
        }
    }
}
