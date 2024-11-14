using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_PlayerController : Singleton<G02_PlayerController>, G02_IDamageable
{
    public static Action<int> OnHpChange;
    public static Action<Vector3> OnOrderStarted;
    public static Action OnOrderEnded;
    [SerializeField] private int _startHP = 3;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _orderDur = 2f;
    [SerializeField] private SpriteRenderer _moveIndicator;
    [SerializeField] private SpriteRenderer _orderIndicator;
    [SerializeField] private float _maxSkillRange = 5f;
    [SerializeField] private G02_SkillsManager _skillManager;

    private int _currentHP;
    private Vector3 _currentMovePos = Vector3.zero;
    private bool _isOrderActive = false;
    private float _orderDurRemaining = 0f;

    private float _xMin;
    private float _xMax;
    private float _yMin;
    private float _yMax;

    protected override void Awake() {
        base.Awake();
        _currentHP = _startHP;
        _currentMovePos = this.transform.position;      
        _moveIndicator.enabled = false;  
        _orderIndicator.enabled = false;
    }

    private void Start() {
        _xMin = G02_GameManager.Instance.MinXBoundry;
        _xMax = G02_GameManager.Instance.MaxXBoundry;
        _yMin = G02_GameManager.Instance.MinYBoundry;
        _yMax = G02_GameManager.Instance.MaxYBoundry;
    }

    private void Update() {
        _orderDurRemaining -= Time.deltaTime;
        if (_isOrderActive && _orderDurRemaining < 0f) {
            OnOrderEnded?.Invoke();
            _isOrderActive = false;
            _orderIndicator.enabled = false;
        }

        //MoveOld();
        SkillInput();
        MoveInput();
        Move();
    }

    private void MoveInput() {
        if (Input.GetMouseButtonDown(0)) {
            var mouseWorldPos = GetMouseWorldPos();
            float newXPos = Mathf.Clamp(mouseWorldPos.x, _xMin, _xMax);
            float newYPos = Mathf.Clamp(mouseWorldPos.y, _yMin, _yMax);
            
            _currentMovePos = new Vector3 (newXPos, newYPos, 0);
            _moveIndicator.gameObject.transform.position = _currentMovePos;
            _moveIndicator.enabled = true;
        }

        if (Input.GetMouseButtonDown(1)) {
            var mouseWorldPos = GetMouseWorldPos();
            float newXPos = Mathf.Clamp(mouseWorldPos.x, _xMin, _xMax);
            float newYPos = Mathf.Clamp(mouseWorldPos.y, _yMin, _yMax);
            
            var orderPos = new Vector3 (newXPos, newYPos, 0);
            _orderIndicator.gameObject.transform.position = orderPos;
            _orderIndicator.enabled = true;
            _orderDurRemaining = _orderDur;
            _isOrderActive = true;

            OnOrderStarted?.Invoke(orderPos);
        }
    }

    private void Move() {
            if (this.transform.position != _currentMovePos) {
                this.transform.position = Vector3.MoveTowards(this.transform.position, _currentMovePos, _moveSpeed * Time.deltaTime);
            } else {
                _moveIndicator.enabled = false;
            }            
    }

    private void MoveOld() {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * _moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * _moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, _xMin, _xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, _yMin, _yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SkillInput() {
        if (CheckSkillRange()) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                _skillManager.UseRevive(GetMouseWorldPos());
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                _skillManager.UseSkill(0, GetMouseWorldPos());
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                _skillManager.UseSkill(1, GetMouseWorldPos());
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                _skillManager.UseSkill(2, GetMouseWorldPos());
            }
        }
    }

    private Vector3 GetMouseWorldPos() {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.nearClipPlane;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        return mouseWorldPosition;
    }

    public bool CheckSkillRange() {
        //Debug.Log("check skill range called; distance: " + Vector2.Distance(this.transform.position, GetMouseWorldPos()));
        if (Vector2.Distance(this.transform.position, GetMouseWorldPos()) <= _maxSkillRange) {
            return true;
        }

        return false;
    }

    public bool TakeDamage(G02_NPC.NpcStatus damageType, int damageAmount) {
        if (damageType == G02_NPC.NpcStatus.Hostile) {
            _currentHP -= damageAmount;
            OnHpChange?.Invoke(_currentHP);
            //Debug.Log("Current player HP: " + _currentHP);
            if (_currentHP <= 0) {
                Debug.Log("Player died.");
            }
            return true;
        }
        return false;
    }
}
