using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLog : Singleton<GameLog>
{
    [SerializeField] int _logSize = 3;
    [SerializeField] GameObject _logTextPrefab;
    List<TextMeshProUGUI> _logTextsList = new();

    protected override void Awake() {
        base.Awake();

        for (int i = 0; i < _logSize; i++) {
            var newLogText = Instantiate(_logTextPrefab, this.transform.position, Quaternion.identity).GetComponent<TextMeshProUGUI>();
            newLogText.gameObject.transform.SetParent(this.transform);
            _logTextsList.Add(newLogText);
            newLogText.transform.localScale = Vector3.one;
        }

        foreach (var logText in _logTextsList) {
            logText.text = "";
        }
    }

    public void UpdateLog(string message) {
        for (int i = _logTextsList.Count - 1; i > 0; i--) {
            _logTextsList[i].text = _logTextsList[i - 1].text;                       
        }
        _logTextsList[0].text = message;
    }
}
