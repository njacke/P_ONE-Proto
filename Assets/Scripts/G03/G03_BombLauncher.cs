using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_BombLauncher : MonoBehaviour
{
    [SerializeField] private int _maxCharges = 6;
    [SerializeField] private float _startChargeCd = 2f;
    [SerializeField] private float _createBombDelay = 1f;
    [SerializeField] GameObject _bombPrefab;

    private G03_Bomb _currentBomb = null;
    private Coroutine _createNewBombRoutine = null;
    private int _totalCharges = 0;
    private int _topCharges = 0;
    private int _botCharges = 0;
    private float _currentChargeCd = 0f;

    private void Awake() {
        _currentBomb = CreateNewBomb();
    }

    private void Update() {
        UpdateCharges();

        if (_currentBomb == null) {
            _createNewBombRoutine ??= StartCoroutine(CreateNewBombRoutine());
        }
    }

    private void UpdateCharges() {
        if (_currentBomb != null)
        {
            _currentChargeCd -= Time.deltaTime;
            if (_totalCharges < _maxCharges && _currentChargeCd <= 0f) {
                if (_topCharges < _botCharges) {
                    _topCharges++;
                } else {
                    _botCharges++;
                }
                _totalCharges++;
                _currentChargeCd = _startChargeCd;
                Debug.Log("Charge added; total charges: " + _totalCharges.ToString());
            }
        }
    }

    private G03_Bomb CreateNewBomb() {
        var newBomb = Instantiate(_bombPrefab, transform.position, Quaternion.identity).GetComponent<G03_Bomb>();
        return newBomb;
    }

    private IEnumerator CreateNewBombRoutine() {
        Debug.Log("Create new bomb routine started.");
        yield return new WaitForSeconds(_createBombDelay);
        _currentBomb = CreateNewBomb();
        _createNewBombRoutine = null;
    }

    public void LaunchBomb() {
        if (_currentBomb != null) {
            _currentBomb.ChargeBomb(_topCharges, _botCharges);
            _totalCharges = 0;
            _topCharges = 0;
            _botCharges = 0;
            _currentChargeCd = _startChargeCd;

            _currentBomb.ActivateBomb();

            _currentBomb = null;
        }
    }
}
