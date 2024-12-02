using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class G05_Item_UI : MonoBehaviour, IPointerClickHandler
{
    public static Action<G05_Item_UI> OnSelection;
    public int SlotIndex { get; set; } 
    public bool IsEmpty { get; set; }
    public bool IsSelected { get; set; }
    public G05_ItemEffect Effect { get; private set; }
    [SerializeField] private Image _mainImage;
    [SerializeField] private Image _bgImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private TextMeshProUGUI _typeText;

    private void OnEnable() {
        G05_GameManager.OnTurnStateChanged += G05_GameManager_OnTurnStateChanged;
    }

    private void OnDisable() {
        G05_GameManager.OnTurnStateChanged -= G05_GameManager_OnTurnStateChanged;
        
    }

    private void G05_GameManager_OnTurnStateChanged(G05_GameManager manager) {
        ToggleSelected(false);
    }

    public void SetEmpty() {
        Effect = null;
        IsEmpty = true;
        ToggleSelected(false);
        _mainImage.color = Color.gray;
        _nameText.text = "Item Slot " + SlotIndex.ToString();
        _valueText.text = "(empty)";
        _typeText.text = "";
    }

    public void SetItem(G05_ItemEffect effect) {
        Effect = effect;
        IsEmpty = false;
        ToggleSelected(false);        
        _mainImage.color = Color.white;
        _nameText.text = effect.EffectName;
        _valueText.text = effect.EffectValue.ToString();

        string typeText = effect.EffectCategory switch {
            G05_ItemEffect.EffectCat.PreRoll => "[Pre-roll]",
            G05_ItemEffect.EffectCat.PostRoll => "[Post-roll]",
            _ => "",
        };

        _typeText.text = typeText;
    }

    private void ToggleSelected(bool isSelected) {
        if (isSelected) {
            _bgImage.color = Color.green;
            IsSelected = true;
        } else {
            _bgImage.color = Color.black;
            IsSelected = false;
        }
    }
 
    public void OnPointerClick(PointerEventData eventData) {
        if (IsEmpty) {
            Debug.Log("No item to select.");
            return;
        }

        if (Effect.EffectCategory == G05_ItemEffect.EffectCat.PreRoll && G05_GameManager.Instance.GetTurnState != G05_GameManager.TurnState.Roll) {
            Debug.Log("Can't select post-roll.");
            return;
        }

        if (Effect.EffectCategory == G05_ItemEffect.EffectCat.PostRoll && G05_GameManager.Instance.GetTurnState != G05_GameManager.TurnState.Move) {
            Debug.Log("Can't select pre-roll.");
            return;
        }

        if (IsSelected) {
            ToggleSelected(false);
        } else {
            ToggleSelected(true);
        }
    }
}
