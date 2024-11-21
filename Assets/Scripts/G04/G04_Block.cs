using System;
using UnityEditor;
using UnityEngine;

public class G04_Block : MonoBehaviour
{
    public G04_CombinedBlock CombinedBlock { get; set; }    
    private SpriteRenderer _spriteRenderer;


    void Start() {        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        CombinedBlock = GetComponentInParent<G04_CombinedBlock>();        
    }

    public void ToggleSelected(bool selected) {
        if (selected) {
            _spriteRenderer.color = Color.green;
        } else {
            _spriteRenderer.color = Color.white;
        }
    }

    public void ToggleEligible(bool eligible) {
        if (eligible) {
            _spriteRenderer.color = Color.white;
        } else {
            _spriteRenderer.color = Color.red;
        }
    }
}
