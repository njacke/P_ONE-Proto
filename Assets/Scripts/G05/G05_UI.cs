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
    [SerializeField] private TextMeshProUGUI _selectionName;
    [SerializeField] private TextMeshProUGUI _selectionDescription;
    [SerializeField] private TextMeshProUGUI _turnCount;
    [SerializeField] private TextMeshProUGUI _playersCount;


    private void Awake() {
        _baseDiceValue.text = "?";
        _bonusDiceValue.text = "?";
        _totalDiceValue.text = "?";
    }

    private void Start() {
        InitItemDisplay();
        _turnCount.text = G05_GameManager.Instance.GetCurrentTurnCount.ToString();
        _playersCount.text = G05_GameManager.Instance.GetPlayersAliveCount.ToString();
    }

    private void OnEnable() {
        G05_Dice.OnValueUpdated += G05_Dice_OnValueUpdated;        
        G05_BoardManager.OnPlayerMoved += G05_BoardManager_OnPlayerMoved;       
        G05_GameManager.OnNewItemCreated += G05_GameManager_OnNewItemCreated; 
        G05_BoardManager.OnSelect += G05_BoardManager_OnSelect;
        G05_GameManager.OnTurnStateChanged += G05_GameManager_OnTurnStateChanged;
        G05_GameManager.OnPlayerTokenDeath += G05_GameManager_OnPlayerTokenDeath;
    }

    private void OnDisable() {
        G05_Dice.OnValueUpdated -= G05_Dice_OnValueUpdated;
        G05_BoardManager.OnPlayerMoved -= G05_BoardManager_OnPlayerMoved;
        G05_GameManager.OnNewItemCreated -= G05_GameManager_OnNewItemCreated; 
        G05_BoardManager.OnSelect -= G05_BoardManager_OnSelect;
        G05_GameManager.OnTurnStateChanged -= G05_GameManager_OnTurnStateChanged;
        G05_GameManager.OnPlayerTokenDeath -= G05_GameManager_OnPlayerTokenDeath;
    }

    private void G05_GameManager_OnPlayerTokenDeath(G05_GameManager sender) {
        _playersCount.text = G05_GameManager.Instance.GetPlayersAliveCount.ToString();
    }

    private void G05_GameManager_OnTurnStateChanged(G05_GameManager sender) {
        if (sender.GetTurnState == G05_GameManager.TurnState.Roll) {
            _turnCount.text = G05_GameManager.Instance.GetCurrentTurnCount.ToString();
        }
    }

    private void G05_BoardManager_OnSelect(G05_BoardManager sender) {
        if (sender.SelectedToken != null) {
            _selectionName.text = sender.SelectedToken.GetTokenName;
            _selectionDescription.text = sender.SelectedToken.GetTokenInfo;
        } else if (sender.SelectedField != null) {
            _selectionName.text = sender.SelectedField.GetFieldName;
            _selectionDescription.text = sender.SelectedField.GetFieldInfo;
        } else {
            _selectionName.text = "n/a";
            _selectionDescription.text = "";
        }
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
            G05_GameManager.Instance.GetGameLog.UpdateLog("Move player token to end turn.");
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
            newItem.transform.localScale = Vector3.one;
        }

        OnItemInitDone?.Invoke();
    }
}
