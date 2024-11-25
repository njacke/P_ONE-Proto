using System;
using UnityEditor;
using UnityEngine;

public class G04_Block : MonoBehaviour
{
    public G04_CombinedBlock CombinedBlock { get; set; }    
    private SpriteRenderer _spriteRenderer;
    private Color _currentColor;


    void Awake() {        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        CombinedBlock = GetComponentInParent<G04_CombinedBlock>();     
        _currentColor = _spriteRenderer.color;   
    }

    public void ToggleSelected(bool selected) {
        if (selected) {
            _spriteRenderer.color = Color.green;
        } else {
            _spriteRenderer.color = _currentColor;
        }
    }

    public void ToggleEligible(bool eligible) {
        if (eligible) {
            _spriteRenderer.color = _currentColor;
        } else {
            _spriteRenderer.color = Color.red;
        }
    }

    public void UpdateColor(Color color) {
        _currentColor = color;
        _spriteRenderer.color = _currentColor;
    }
}
