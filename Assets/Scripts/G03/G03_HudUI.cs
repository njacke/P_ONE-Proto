using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class G03_HudUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _coordinateTextX;
    [SerializeField] TextMeshProUGUI _coordinateTextY;
    private G03_PlayerController _player;
    private float _mapSizeX;
    private float _mapSizeY;

    private void Start() {
        _player = G03_PlayerController.Instance;
        _mapSizeX = G03_GameManager.Instance.GetMapSizeX;
        _mapSizeY = G03_GameManager.Instance.GetMapSizeY;
    }

    private void Update() {
        SetCoordinateText();        
    }

    private void SetCoordinateText() {
        Vector3 playerCoordinates = new(_player.GetPlayerPos.x + _mapSizeX, _player.GetPlayerPos.y + _mapSizeY, 0);

        _coordinateTextX.text = "X: " + FormatCoordinateToString(playerCoordinates.x);
        _coordinateTextY.text = "Y: " + FormatCoordinateToString(playerCoordinates.y);
    }

    private string FormatCoordinateToString(float coordinate) {
        int integerPart = Mathf.FloorToInt(coordinate); // Get the integer part
        float decimalPart = Mathf.Abs(coordinate - integerPart); // Ensure the decimal part is positive
        string formattedCoordinate = $"{integerPart:00}.{(decimalPart * 100):00}";

    return formattedCoordinate;
    }

}
