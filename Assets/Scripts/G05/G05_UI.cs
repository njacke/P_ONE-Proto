using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class G05_UI : MonoBehaviour
{
    public static Action OnDiceRoll;
    public static Action OnUseSelectedItems;
    public static Action OnItemInitDone;
    public int EmptyItemSlotsCount { get; private set; }
    public G05_Item_UI[] ItemSlots { get; private set; }
    [SerializeField] private TextMeshProUGUI _baseDiceValue;
    [SerializeField] private TextMeshProUGUI _bonusDiceValue;
    [SerializeField] private TextMeshProUGUI _totalDiceValue;
    [SerializeField] private GameObject _itemDisplay;
    [SerializeField] private GameObject _itemPrefab;

    private void Awake() {
        _baseDiceValue.text = "?";
        _bonusDiceValue.text = "?";
        _totalDiceValue.text = "?";
    }

    private void Start() {
        InitItemDisplay();
    }

    private void OnEnable() {
        G05_Dice.OnValueUpdated += G05_Dice_OnValueUpdated;        
        G05_BoardManager.OnPlayerMoved += G05_BoardManager_OnPlayerMoved;       
        G05_GameManager.OnNewItemCreated += G05_GameManager_OnNewItemCreated; 
    }

    private void OnDisable() {
        G05_Dice.OnValueUpdated -= G05_Dice_OnValueUpdated;
        G05_BoardManager.OnPlayerMoved -= G05_BoardManager_OnPlayerMoved;
        G05_GameManager.OnNewItemCreated -= G05_GameManager_OnNewItemCreated; 
    }

    private void G05_GameManager_OnNewItemCreated(G05_ItemEffect effect) {
        Debug.Log("New item created called in UI with effect " + effect.ToString());
        G05_Item_UI selectedSlot = null;
        foreach (var slot in ItemSlots) {
            if (slot.IsEmpty) {
                selectedSlot = slot;
                Debug.Log("Selected slot + "+ slot.SlotIndex.ToString());
                break;
            }
        }

        if (selectedSlot != null) {
            selectedSlot.SetItem(effect);
        }
    }

    private void G05_BoardManager_OnPlayerMoved(G05_BoardManager sender) {
        _baseDiceValue.text = "?";
        _bonusDiceValue.text = "?";
        _totalDiceValue.text = "?";
    }

    private void G05_Dice_OnValueUpdated(G05_Dice sender) {
        _baseDiceValue.text = sender.BaseValue.ToString();
        _bonusDiceValue.text = (sender.BonusValue + sender.SpecialValue).ToString(); // combined in display
        _totalDiceValue.text = sender.TotalValue.ToString();
    }

    public void RollDiceOnClick() {
        EventSystem.current.SetSelectedGameObject(null);

        if (G05_GameManager.Instance.GetTurnState != G05_GameManager.TurnState.Roll) {
            Debug.Log("Move player token to end turn.");
        } else {
            OnDiceRoll?.Invoke();
        }
    }

    public void UseSelectedItemsOnClick() {
        foreach (var slot in ItemSlots) {
            if (!slot.IsEmpty && slot.IsSelected) {
                slot.Effect.ResolveEffect();
                slot.SetEmpty();
            }
        }

        EventSystem.current.SetSelectedGameObject(null);
        OnUseSelectedItems?.Invoke();        
    }

    private void InitItemDisplay() {
        var maxItems = G05_GameManager.Instance.GetMaxItems;
        ItemSlots = new G05_Item_UI[maxItems];

        for (int i = 0; i < maxItems; i++) {
            var newItem = Instantiate(_itemPrefab, transform.position, Quaternion.identity).GetComponent<G05_Item_UI>();
            newItem.gameObject.transform.SetParent(_itemDisplay.transform);
            newItem.SlotIndex = i;
            newItem.SetEmpty();
            ItemSlots[i] = newItem;
        }

        OnItemInitDone?.Invoke();
    }
}
