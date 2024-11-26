using System;
using UnityEditor;
using UnityEngine;

public class G04_Block : MonoBehaviour
{
    public G04_CombinedBlock CombinedBlock { get; set; }    
    [SerializeField] private GameObject _outline;
    [SerializeField] float _outlineThickness = .05f;
    private SpriteRenderer _mainSpriteRenderer;
    private SpriteRenderer _outlineSpriteRenderer;
    private Color _startColor;


    void Awake() {      
        _mainSpriteRenderer = GetComponent<SpriteRenderer>();  
        _startColor = _mainSpriteRenderer.color;  

        _outlineSpriteRenderer = _outline.GetComponent<SpriteRenderer>();
        _outline.transform.localScale = Vector3.one + Vector3.one * _outlineThickness;

        CombinedBlock = GetComponentInParent<G04_CombinedBlock>();     
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
            _outlineSpriteRenderer.color = Color.black;
        } else {
            _outlineSpriteRenderer.color = Color.red;
        }
    }

    public void UpdateColor(Color color) {
        _startColor = color;
        _mainSpriteRenderer.color = _startColor;
    }
}
