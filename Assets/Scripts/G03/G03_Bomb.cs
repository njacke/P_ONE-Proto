using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_Bomb : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Collider2D _myCollider;
    private int _topCharges = 0;
    private int _botCharges = 0;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _myCollider = GetComponent<Collider2D>();
    }

    public void SetScale (float scaleFactor) {
        this.transform.localScale = new Vector3(this.transform.localScale.x * scaleFactor,
                                                this.transform.localScale.y * scaleFactor,
                                                this.transform.localScale.z);
    }

    public void ChargeBomb(int topCharges, int botCharges) {
        _topCharges = topCharges;
        _botCharges = botCharges;       
    }

    public void ActivateBomb() {
        this.transform.position = G03_PlayerController.Instance.transform.position;
        _spriteRenderer.enabled = true;
        _myCollider.enabled = true;        
        Debug.Log("Bomb activated with " + _topCharges.ToString() + " top charges and " + _botCharges.ToString() + " bot charges.");
    }
}
