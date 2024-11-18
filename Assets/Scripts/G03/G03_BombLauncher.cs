using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_BombLauncher : MonoBehaviour
{
    public static Action<string> OnBombCreated;
    public static Action OnBombLaunched;
    public static Action<int, int, int> OnChargeChange;

    [SerializeField] private int _maxCharges = 4;
    [SerializeField] private float _startChargeCd = 2f;
    [SerializeField] private float _createBombDelay = 1f;
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private GameObject[] _bombEffects;

    private G03_Bomb _currentBomb = null;
    private Coroutine _createNewBombRoutine = null;
    private int _totalCharges = 0;
    private int _topCharges = 0;
    private int _botCharges = 0;
    private float _currentChargeCd = 0f;

    private void Awake() {
        _currentChargeCd = _startChargeCd;
    }

    private void Start() {
        CreateNewBomb();
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
                OnChargeChange?.Invoke(_topCharges, _botCharges, _maxCharges);
                //Debug.Log("Charge added; total charges: " + _totalCharges.ToString());
            }
        }
    }

    private void CreateNewBomb() {
        _currentBomb = Instantiate(_bombPrefab, transform.position, Quaternion.identity).GetComponent<G03_Bomb>();
        var rndEffectIndex = UnityEngine.Random.Range(0, _bombEffects.Length);
        var newBombEffect = Instantiate(_bombEffects[rndEffectIndex], this.transform.position, Quaternion.identity).GetComponent<G03_BombEffect>();
        newBombEffect.transform.SetParent(_currentBomb.transform);
        _currentBomb.SetEffect(newBombEffect);
        OnBombCreated?.Invoke(newBombEffect.GetEffectName);
    }

    private IEnumerator CreateNewBombRoutine() {
        //Debug.Log("Create new bomb routine started.");
        yield return new WaitForSeconds(_createBombDelay);
        CreateNewBomb();
        _createNewBombRoutine = null;
    }

    public void RedistributeCharge(bool addTop) {
        if (addTop && _topCharges < _totalCharges) {
            _botCharges--;
            _topCharges ++;
        } else if (_botCharges < _totalCharges) {
            _topCharges--;
            _botCharges++;
        }
        OnChargeChange?.Invoke(_topCharges, _botCharges, _maxCharges);
    }

    public void LaunchBomb() {
        if (_currentBomb != null) {
            _currentBomb.ChargeBomb(_topCharges, _botCharges);
            _totalCharges = 0;
            _topCharges = 0;
            _botCharges = 0;
            _currentChargeCd = _startChargeCd;
            OnChargeChange?.Invoke(_topCharges, _botCharges, _maxCharges);

            _currentBomb.ActivateBomb();
            _currentBomb = null;
            OnBombLaunched?.Invoke();
        }
    }
}
