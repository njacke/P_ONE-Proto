using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class G02_NPC : MonoBehaviour, G02_IDamageable
{
    public static Action<GameObject> OnStatusChange;
    public static Action<NpcStatus, int> OnDamageTaken;
    public static Action<NpcStatus> OnDeath;

    [SerializeField] private NpcStatus _currentNpcStatus = NpcStatus.Hostile;
    [SerializeField] private int _startHP = 1;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _startMoveDirCD = 3f;
    [SerializeField] private float _chaseRange = 3f;
    [SerializeField] private float _targetScanRadius = 10f;
    [SerializeField] private G02_NpcAttack _npcAttack;
    [SerializeField] private TextMeshPro _textHP;
    public NpcStatus CurrentNpcStatus { get { return _currentNpcStatus; } set { UpdateNpcStatus(value); } }
    public int GetCurrentHP { get { return _currentHP; } }
    public float GetMoveSpeed { get { return _moveSpeed; } }
    public int GetStartAttackCD { get { return _currentHP; } }
    private bool _isFeared = false;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _myRb;
    private Collider2D _myCollider;
    private NpcState _currentNpcState = NpcState.None;
    private int _currentHP = 0;
    private Transform _currentTarget = null;
    private Vector2 _currentMoveDir = Vector2.zero;
    private float _currentMoveDirCD = 0f;
    private float _attackRange = 0f;
    private float _startAttackCD = 0f;
    private float _currentAttackCD = .5f; // start attack delay after spawning
    private bool _hasOrder = false;
    private Vector3 _currentOrderPos = Vector2.zero;

    private float _xMin;
    private float _xMax;
    private float _yMin;
    private float _yMax;

    public enum NpcStatus {
        None,
        Hostile,
        Corpse,
        Friendly
    }

    public enum NpcState {
        None,
        Roaming,
        Chasing,
        Attacking
    }

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _myCollider = GetComponent<Collider2D>();
        _myRb = GetComponent<Rigidbody2D>();
        UpdateHP(_startHP);
        _attackRange = _npcAttack.GetAttackRange;
        _startAttackCD = _npcAttack.GetAttackCD;
    }

    private void Start() {
        _xMin = G02_GameManager.Instance.MinXBoundry;
        _xMax = G02_GameManager.Instance.MaxXBoundry;
        _yMin = G02_GameManager.Instance.MinYBoundry;
        _yMax = G02_GameManager.Instance.MaxYBoundry;

        UpdateNpcStatus(_currentNpcStatus);
    }

    private void OnEnable() {
        G02_NPC.OnStatusChange += G02_NPC_OnStatusChange; 
        G02_PlayerController.OnOrderStarted += G02_PlayerController_OnOrderStarted;       
        G02_PlayerController.OnOrderEnded += G02_PlayerController_OnOrderEnded;       
    }

    private void OnDisable() {
        G02_NPC.OnStatusChange -= G02_NPC_OnStatusChange;        
        G02_PlayerController.OnOrderStarted -= G02_PlayerController_OnOrderStarted;       
        G02_PlayerController.OnOrderEnded -= G02_PlayerController_OnOrderEnded;       
    }


    private void G02_NPC_OnStatusChange(GameObject sender) {
        if (_currentTarget != null && sender.gameObject == _currentTarget.gameObject) {
            _currentTarget = null;
        }
    }

    private void G02_PlayerController_OnOrderStarted(Vector3 orderPos) {
        if (_currentNpcState == NpcState.Roaming && CurrentNpcStatus == NpcStatus.Friendly) {
            _currentOrderPos = orderPos;
            _hasOrder = true;
        }
    }

    private void G02_PlayerController_OnOrderEnded() {
        _hasOrder = false;
    }

    private void Update() {
        _currentMoveDirCD -= Time.deltaTime;
        _currentAttackCD -= Time.deltaTime;

        if (!_isFeared) {
            NpcStateTransition();
            if (_currentMoveDirCD <= 0f) {
                _currentMoveDir = GetRandomMoveDir();
                _currentMoveDirCD = _startMoveDirCD;
            }
        }
    }

    private void FixedUpdate() {
        if (CurrentNpcStatus == NpcStatus.Hostile || CurrentNpcStatus == NpcStatus.Friendly) {
            if (_currentNpcState == NpcState.Roaming) {
                Roam();
            } else if (_currentNpcState == NpcState.Chasing) {
                Chase();
            } else if (_currentNpcState == NpcState.Attacking) {
                Attack();
            }
        }
    }


    private void UpdateNpcStatus(NpcStatus newNpcStatus) {
        _currentNpcStatus = newNpcStatus;
        _npcAttack.AttackType = newNpcStatus;


        if (newNpcStatus == NpcStatus.Friendly) {
            UpdateHP(_startHP); // reset HP when revived
            _hasOrder = false;
        }
        
        _spriteRenderer.color = newNpcStatus switch {
            NpcStatus.Hostile => Color.red,
            NpcStatus.Corpse => Color.yellow,
            NpcStatus.Friendly => Color.green,
            _ => _spriteRenderer.color
        };
 
        _currentTarget = GetClosestTargetInRange();

        OnStatusChange?.Invoke(this.gameObject);
    }

    private Transform GetClosestTargetInRange() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _targetScanRadius);
        var sortedHits = hits.OrderBy(hit => Vector2.Distance(transform.position, hit.transform.position));

        foreach (var hit in sortedHits) {
            G02_NPC npc = hit.GetComponent<G02_NPC>();
            G02_PlayerController player = hit.GetComponent<G02_PlayerController>();

            if (_currentNpcStatus == NpcStatus.Hostile && 
                ((npc != null && npc.CurrentNpcStatus == NpcStatus.Friendly) || player != null)) {
                return hit.transform;
            }

            if (_currentNpcStatus == NpcStatus.Friendly && npc != null && npc.CurrentNpcStatus == NpcStatus.Hostile) {
                return hit.transform;
            }
        }

        return null;
    }

    private void NpcStateTransition() {
        if (_currentTarget == null) {
            if (_currentNpcState != NpcState.Roaming && (CurrentNpcStatus == NpcStatus.Hostile || CurrentNpcStatus == NpcStatus.Friendly)) {
                _currentNpcState = NpcState.Roaming;
            }
            return;
        }

        float targetDistance = Vector3.Distance(_currentTarget.position, this.transform.position);

        if (_currentNpcStatus != NpcStatus.None) {
            if (_currentNpcStatus == NpcStatus.Corpse && _currentNpcState != NpcState.None) {
                //Debug.Log("Switching to none state");
                _currentNpcState = NpcState.None;
                _hasOrder = false;
            } else if (_currentNpcState != NpcState.Roaming && targetDistance > _chaseRange) {
                //Debug.Log("Switching to roaming state");
                _currentNpcState = NpcState.Roaming;
                _currentTarget = GetClosestTargetInRange();
            } else if (_currentNpcState != NpcState.Chasing && targetDistance > _attackRange && targetDistance <= _chaseRange) {
                //Debug.Log("Switching to chasing state");
                _currentNpcState = NpcState.Chasing;
                _hasOrder = false;
                _currentTarget = GetClosestTargetInRange();
            } else if (_currentNpcState != NpcState.Attacking && targetDistance <= _attackRange) {
               //Debug.Log("Switching to attacking state");
                _currentNpcState = NpcState.Attacking;
                _hasOrder = false;
                _currentTarget = GetClosestTargetInRange();
            }
        }
    }

    private void Roam() {
        _currentTarget = GetClosestTargetInRange();

        if (_hasOrder && CurrentNpcStatus == NpcStatus.Friendly) {
            MoveToOrder();
        } else {
            var deltaX = _currentMoveDir.x * Time.deltaTime * _moveSpeed;
            var deltaY = _currentMoveDir.y * Time.deltaTime * _moveSpeed;

            var newXPos = Mathf.Clamp(transform.position.x + deltaX, _xMin, _xMax);
            var newYPos = Mathf.Clamp(transform.position.y + deltaY, _yMin, _yMax);
            transform.position = new Vector2(newXPos, newYPos);
        }
    }

    private Vector2 GetRandomMoveDir() {
        return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private void Chase() {
        if (_currentTarget == null) {
            return;
        }

        Vector2 dir = (_currentTarget.position - this.transform.position).normalized;
        float deltaX = dir.x * Time.deltaTime * _moveSpeed;
        float deltaY = dir.y * Time.deltaTime * _moveSpeed;

        this.transform.position = new Vector2(this.transform.position.x + deltaX, this.transform.position.y + deltaY);
    }

    protected virtual void Attack() {
        if (_currentTarget == null) {
            return;
        }

        if (_currentAttackCD <= 0f) {
            _npcAttack.UseAttack(_currentTarget.position);
            //Debug.Log("Using attack");
            _currentAttackCD = _startAttackCD;
        }
    }

    public bool TakeDamage(NpcStatus damageType, int damageAmount) {
        if (CurrentNpcStatus == NpcStatus.Hostile && damageType == NpcStatus.Friendly) {
            UpdateHP(_currentHP - damageAmount);
            OnDamageTaken?.Invoke(CurrentNpcStatus, damageAmount);

            if (_currentHP <= 0) {
            Debug.Log("Hostile NPC died; updating to corpse");
            OnDeath?.Invoke(CurrentNpcStatus);
            UpdateNpcStatus(NpcStatus.Corpse);
            }
            return true;
        } else if (CurrentNpcStatus == NpcStatus.Friendly && damageType == NpcStatus.Hostile) {
            UpdateHP(_currentHP - damageAmount);
            OnDamageTaken?.Invoke(CurrentNpcStatus, damageAmount);
            
            if (_currentHP <= 0) {
            Debug.Log("Friendly NPC died; destroying object");
            OnDeath?.Invoke(CurrentNpcStatus);
            Destroy(this.gameObject);
            }
            return true;
        }
        return false;
    }

    public void UpdateHP(int newHP) {
        _currentHP = newHP;
        _textHP.text = newHP.ToString();
    }

    public void UpdateMoveSpeed(float newSpeed) {
        _moveSpeed = newSpeed;
    }

    public void UpdateAttackCD(float newAttackCD, bool resetCD) {
        _startAttackCD = newAttackCD;
        if (resetCD) {
            _currentAttackCD = 0f;
        }
    }

    public void SetFear(bool isFeared, float moveSpeed) {
        if (isFeared) {
            _isFeared = isFeared;
            _currentNpcState = NpcState.Roaming;
            _currentMoveDir = GetRandomMoveDir();
        } else {
            _isFeared = false;
        }

        _moveSpeed = moveSpeed;
    }

    private void MoveToOrder() {
        if (this.transform.position != _currentOrderPos) {
            Debug.Log("move to order entered");
            this.transform.position = Vector3.MoveTowards(this.transform.position, _currentOrderPos, _moveSpeed * Time.deltaTime);
        } else {
            _hasOrder = false;
        }      
    }
}
