using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class G03_HudUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _coordinateTextX;
    [SerializeField] TextMeshProUGUI _coordinateTextY;
    [SerializeField] TextMeshProUGUI _topText;
    [SerializeField] TextMeshProUGUI _botText;
    [SerializeField] private Image _charge1;
    [SerializeField] private Image _charge2;
    [SerializeField] private Image _charge3;
    [SerializeField] private Image _charge4;
    [SerializeField] private Sprite _chargeEmptySprite;
    [SerializeField] private Sprite _chargeTopSprite;
    [SerializeField] private Sprite _chargeBotSprite;
    [SerializeField] private Transform _friendlyObjectiveDisplay; 
    [SerializeField] private Transform _hostileObjectiveDisplay; 
    [SerializeField] private GameObject _objectiveDisplayPrefab;
    private string _currentTopName = "Radius";
    private string _currentBotName = "?";
    private List<Image> _chargesList = new();
    private G03_PlayerController _player;
    private float _mapSizeX;
    private float _mapSizeY;
    private G03_Objective[] _gameObjectives;
    private Dictionary<G03_Objective, TextMeshProUGUI> _objectivesTextDict = new();

    private void Awake() {
        ResetBombText();

        _chargesList.Add(_charge1);
        _chargesList.Add(_charge2);
        _chargesList.Add(_charge3);
        _chargesList.Add(_charge4);

        foreach (var charge in _chargesList) {
            charge.sprite = _chargeEmptySprite;
        }   
    }

    private void Start() {
        _player = G03_PlayerController.Instance;
        _mapSizeX = G03_GameManager.Instance.GetMapSizeX;
        _mapSizeY = G03_GameManager.Instance.GetMapSizeY;
        _gameObjectives = G03_GameManager.Instance.GetGameObjectives;

        SetUpGameObjectivesUI();
    }

    private void Update() {
        SetCoordinateText();        
    }


    private void OnEnable() {
        G03_BombLauncher.OnBombCreated += G03_BombLauncher_OnBombCreated;
        G03_BombLauncher.OnBombLaunched += G03_BombLauncher_OnBombLaunched;
        G03_BombLauncher.OnChargeChange += G03_BombLauncher_OnChargeChange;
        G03_Objective.OnDamageTaken += G03_Objective_OnDamageTaken;        
    }

    private void OnDisable() {
        G03_BombLauncher.OnBombCreated -= G03_BombLauncher_OnBombCreated;
        G03_BombLauncher.OnBombLaunched -= G03_BombLauncher_OnBombLaunched;
        G03_BombLauncher.OnChargeChange -= G03_BombLauncher_OnChargeChange;
        G03_Objective.OnDamageTaken -= G03_Objective_OnDamageTaken;        
    }


    private void G03_BombLauncher_OnBombCreated(string effectName) {
        _currentBotName = effectName;
        SetBombText(0, 0);
    }

    private void G03_BombLauncher_OnBombLaunched() {
        ResetBombText();
    }

    private void G03_BombLauncher_OnChargeChange(int topCharges, int botCharges, int maxCharges) {
        if (maxCharges > _chargesList.Count) {
            Debug.Log("Max charges: " + maxCharges);
            Debug.Log("Charges count: " + _chargesList.Count);
            Debug.LogError("Not enough charge images");
            return;
        }

        int maxEmptyIndex = _chargesList.Count - topCharges - botCharges - 1;
        int maxTopIndex = maxEmptyIndex + topCharges;

        // set charges
        for (int i = 0; i < _chargesList.Count; i++) {
            if (i <= maxEmptyIndex) {
                _chargesList[i].sprite = _chargeEmptySprite;
            } else if (i <= maxTopIndex) {
                _chargesList[i].sprite = _chargeTopSprite;
            } else {
                _chargesList[i].sprite = _chargeBotSprite;
            }
        }

        SetBombText(topCharges, botCharges);
    }

    private void G03_Objective_OnDamageTaken(G03_Objective sender) {
        var newRemainingHp = (float)sender.GetCurrentHP / sender.GetStartHP * 100f;
        if (newRemainingHp < 0) {
            newRemainingHp = 0;
        }

        Debug.Log("New remaining HP is " + newRemainingHp);

        UpdateObjectiveUI(sender, newRemainingHp);
    }

    private void SetUpGameObjectivesUI() {
        foreach (var objective in _gameObjectives) {
            var newObjectiveDisplay = Instantiate(_objectiveDisplayPrefab, this.transform.position, Quaternion.identity);
            var displaytext = newObjectiveDisplay.GetComponent<TextMeshProUGUI>();
            _objectivesTextDict.Add(objective, displaytext);

            if (objective.GetObjectiveType == G03_Objective.ObjectiveType.Friendly) {
                newObjectiveDisplay.transform.SetParent(_friendlyObjectiveDisplay);
            } else if (objective.GetObjectiveType == G03_Objective.ObjectiveType.Hostile) {
                newObjectiveDisplay.transform.SetParent(_hostileObjectiveDisplay);
            }

            UpdateObjectiveUI(objective, 100);
            newObjectiveDisplay.transform.localScale = Vector3.one;
        }
    }

    private void UpdateObjectiveUI(G03_Objective objective, float remainingHpPercent) {
        string newText;

        if (objective.GetIsBase) {
            newText = "Base " + objective.ObjectiveCount.ToString() + ": " + Mathf.CeilToInt(remainingHpPercent).ToString() + "%";            
        } else {
            newText = "Tower " + objective.ObjectiveCount.ToString() + ": " + Mathf.CeilToInt(remainingHpPercent).ToString() + "%";
        }

        _objectivesTextDict[objective].text = newText;
    }

    private void SetBombText(int topCharges, int botCharges) {
        var newTopText = _currentTopName + "\n(+" + topCharges.ToString() + ")";
        var newBotText = _currentBotName + "\n(+" + botCharges.ToString() + ")"; 

        _topText.text = newTopText;
        _botText.text = newBotText;       
    }

    private void ResetBombText() {
        _topText.text = "?";
        _botText.text = "?";
    }


    private void SetCoordinateText() {
        Vector3 playerCoordinates = new(_player.GetPlayerPos.x + _mapSizeX, _player.GetPlayerPos.y + _mapSizeY, 0);

        _coordinateTextX.text = "X: " + FormatCoordinateToString(playerCoordinates.x);
        _coordinateTextY.text = "Y: " + FormatCoordinateToString(playerCoordinates.y);
    }

    private string FormatCoordinateToString(float coordinate) {
        int integerPart = Mathf.FloorToInt(coordinate);
        float decimalPart = Mathf.Abs(coordinate - integerPart);
        string formattedCoordinate = $"{integerPart:00}.{(decimalPart * 100):00}";

    return formattedCoordinate;
    }

}
