using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_Bomb : MonoBehaviour
{
    private G03_BombEffect _currentBombEffect;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _myCollider;
    private int _topCharges = 0;
    private int _botCharges = 0;
    private float _bombLifetime;
    private bool _hasActivated = false;
    private Dictionary<int, float> _chargesScaleDisct = new() {
        { 0, 1f },
        { 1, 1.4f },
        { 2, 1.8f },
        { 3, 2.2f },
        { 4, 2.6f },
    };

    private List<G03_NPC> _affectedNpcs = new();

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _myCollider = GetComponent<Collider2D>();
    }


    private void Update() {
        if (_hasActivated) {
            _bombLifetime -= Time.deltaTime;
            if (_bombLifetime <= 0f) {
                Destroy(this.gameObject);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other) {
        var npc = other.GetComponent<G03_NPC>();
        if (npc != null && _currentBombEffect.ApplyEffect(npc)) {
            _affectedNpcs.Add(npc);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!_currentBombEffect.GetIsOneTime) {
            var npc = other.GetComponent<G03_NPC>();
            if (npc != null && _currentBombEffect.RemoveEffect(npc)) {
                _affectedNpcs.Remove(npc);
            }
        }
    }

    private void OnDestroy() {
        if (!_currentBombEffect.GetIsOneTime) {
            foreach (var npc in _affectedNpcs) {
                _currentBombEffect.RemoveEffect(npc);
            }
        }
    }

    public void SetEffect(G03_BombEffect bombEffect) {
        _currentBombEffect = bombEffect;        
        _bombLifetime = _currentBombEffect.GetEffectLifetime;
        _spriteRenderer.color = _currentBombEffect.GetEffectColor;
    }

    private void SetScale(int topCharges) {
        this.transform.localScale = new Vector3(this.transform.localScale.x * _chargesScaleDisct[topCharges],
                                                this.transform.localScale.y * _chargesScaleDisct[topCharges],
                                                this.transform.localScale.z);
    }

    private void SetPower(int botCharges) {
        _currentBombEffect.SetEffectPower(botCharges);
    }


    public void ChargeBomb(int topCharges, int botCharges) {
        _topCharges = topCharges;
        _botCharges = botCharges;      
        SetScale(topCharges);
        SetPower(botCharges);
    }

    public void ActivateBomb() {
        this.transform.position = G03_PlayerController.Instance.transform.position;
        _spriteRenderer.enabled = true;
        _myCollider.enabled = true;        
        _hasActivated = true;
        Debug.Log("Bomb activated with " + _topCharges.ToString() + " top charges and " + _botCharges.ToString() + " bot charges.");
    }
}
