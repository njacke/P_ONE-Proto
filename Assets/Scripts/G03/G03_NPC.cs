using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class G03_NPC : MonoBehaviour, G03_IDamageable
{
    public static Action<G03_NPC> OnStatusChange;
    public static Action<NpcStatus, int> OnDamageTaken;
    public static Action<NpcStatus> OnDeath;

    [SerializeField] private NpcStatus _currentNpcStatus = NpcStatus.Hostile;
    [SerializeField] private NpcState _currentNpcState = NpcState.None;
    [SerializeField] private int _startHP = 1;
    [SerializeField] private float _startMoveSpeed = 1f;
    [SerializeField] private float _startMoveDirCD = 3f;
    [SerializeField] private float _chaseRange = 3f;
    [SerializeField] private float _targetScanRadius = 10f;
    [SerializeField] private G03_NpcAttack _npcAttack;
    [SerializeField] private TextMeshPro _textHP;
    [SerializeField] private int _corpseLayerOrder = -2;
    public NpcStatus CurrentNpcStatus { get { return _currentNpcStatus; } set { UpdateNpcStatus(value); } }
    public int GetCurrentHP { get { return _currentHP; } }
    public float GetStartMoveSpeed { get { return _startMoveSpeed; } }
    public int GetStartAttackCD { get { return _currentHP; } }
    public G03_Objective CurrentGoalObjective { get; set; }
    public G03_Objective BaseObjective { get; set; }
    public G03_NpcAttack GetNpcAttack { get { return _npcAttack ; } }

    private float _currentMoveSpeed;
    private bool _isFeared = false;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _myCollider;
    private int _currentHP = 0;
    private Collider2D _currentTargetCollider = null;
    private Vector2 _currentMoveDir = Vector2.zero;
    private float _currentMoveDirCD = 0f;
    private float _attackRange = 0f;
    private float _startAttackCD = 0f;
    private float _currentAttackCD = 0f;
    
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
        _currentMoveSpeed = _startMoveSpeed;
        UpdateHP(_startHP);
        _attackRange = _npcAttack.GetAttackRange;
        _startAttackCD = _npcAttack.GetAttackCD;
    }

    private void Start() {
        _xMin = G03_GameManager.Instance.MinXBoundry;
        _xMax = G03_GameManager.Instance.MaxXBoundry;
        _yMin = G03_GameManager.Instance.MinYBoundry;
        _yMax = G03_GameManager.Instance.MaxYBoundry;

        UpdateNpcStatus(_currentNpcStatus);
    }

    private void OnEnable() {
        G03_NPC.OnStatusChange += G03_NPC_OnStatusChange;    
        G03_Objective.OnDeath += G03_Objective_OnDeath;
    }

    private void OnDisable() {
        G03_NPC.OnStatusChange -= G03_NPC_OnStatusChange;            
        G03_Objective.OnDeath -= G03_Objective_OnDeath;
    }

    private void G03_NPC_OnStatusChange(G03_NPC sender) {
        if (_currentTargetCollider != null && sender.gameObject == _currentTargetCollider.gameObject) {
            _currentTargetCollider = null;
        }
    }

    private void G03_Objective_OnDeath(G03_Objective sender) {
        if (_currentTargetCollider != null && sender.gameObject == _currentTargetCollider.gameObject) {
            _currentTargetCollider = null;
        }

        if (sender == CurrentGoalObjective) {
            CurrentGoalObjective = BaseObjective;
        }
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

        if (newNpcStatus == NpcStatus.Corpse) {
            _myCollider.enabled = false; // disable collider; G03 only
            _spriteRenderer.sortingOrder = _corpseLayerOrder;
            //Destroy(this.gameObject);
        }
        
        _spriteRenderer.color = newNpcStatus switch {
            NpcStatus.Hostile => Color.red,
            NpcStatus.Corpse => Color.yellow,
            NpcStatus.Friendly => Color.green,
            _ => _spriteRenderer.color
        };
 
        _currentTargetCollider = GetClosestTargetInRange();

        OnStatusChange?.Invoke(this);
    }

    private Collider2D GetClosestTargetInRange() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, _targetScanRadius);
        var sortedHits = hits.OrderBy(hit => Vector2.Distance(this.transform.position, hit.transform.position));

        foreach (var hit in sortedHits) {
            G03_NPC npc = hit.GetComponent<G03_NPC>();

            if (npc != null && _currentNpcStatus == NpcStatus.Hostile && npc.CurrentNpcStatus == NpcStatus.Friendly) {
                return hit.GetComponent<Collider2D>();
            } else if (npc != null && _currentNpcStatus == NpcStatus.Friendly && npc.CurrentNpcStatus == NpcStatus.Hostile) {
                return hit.GetComponent<Collider2D>();
            } else {
                G03_Objective objective = hit.GetComponent<G03_Objective>();

                if (objective != null && _currentNpcStatus == NpcStatus.Hostile && objective.GetObjectiveType == G03_Objective.ObjectiveType.Friendly) {
                    return hit.GetComponent<Collider2D>();
                } else if (objective != null && _currentNpcStatus == NpcStatus.Friendly && objective.GetObjectiveType == G03_Objective.ObjectiveType.Hostile) {
                    return hit.GetComponent<Collider2D>();
                }
            }
        }

        return null;
    }

    private void NpcStateTransition() {
        if (_currentTargetCollider == null) {
            if (_currentNpcState != NpcState.Roaming && (CurrentNpcStatus == NpcStatus.Hostile || CurrentNpcStatus == NpcStatus.Friendly)) {
                _currentNpcState = NpcState.Roaming;
            }
            return;
        }

        Vector3 targetClosestPoint = _currentTargetCollider.ClosestPoint(this.transform.position);
        float targetDistance = Vector3.Distance(targetClosestPoint, this.transform.position);

        if (_currentNpcStatus != NpcStatus.None) {
            if (_currentNpcStatus == NpcStatus.Corpse && _currentNpcState != NpcState.None) {
                //Debug.Log("Switching to none state");
                _currentNpcState = NpcState.None;
            } else if (_currentNpcState != NpcState.Roaming && targetDistance > _chaseRange) {
                //Debug.Log("Switching to roaming state");
                _currentNpcState = NpcState.Roaming;
                _currentTargetCollider = GetClosestTargetInRange();
            } else if (_currentNpcState != NpcState.Chasing && targetDistance > _attackRange && targetDistance <= _chaseRange) {
                //Debug.Log("Switching to chasing state");
                _currentNpcState = NpcState.Chasing;
                _currentTargetCollider = GetClosestTargetInRange();
            } else if (_currentNpcState != NpcState.Attacking && targetDistance <= _attackRange) {
               //Debug.Log("Switching to attacking state");
                _currentNpcState = NpcState.Attacking;
                _currentTargetCollider = GetClosestTargetInRange();
            }
        }
    }

    private void Roam() {
        _currentTargetCollider = GetClosestTargetInRange();
        MoveToCurrentObjective();
    }

    private Vector2 GetRandomMoveDir() {
        return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private void Chase() {
        if (_currentTargetCollider == null) {
            return;
        }

        Vector2 dir = (_currentTargetCollider.transform.position - this.transform.position).normalized;
        float deltaX = dir.x * Time.deltaTime * _currentMoveSpeed;
        float deltaY = dir.y * Time.deltaTime * _currentMoveSpeed;

        this.transform.position = new Vector2(this.transform.position.x + deltaX, this.transform.position.y + deltaY);
    }

    protected virtual void Attack() {
        if (_currentTargetCollider == null) {
            return;
        }

        if (_currentAttackCD <= 0f) {
            _npcAttack.UseAttack(_currentTargetCollider.transform.position);
            //Debug.Log("Using attack");
            _currentAttackCD = _startAttackCD;
        }
    }

    public bool TakeDamage(NpcStatus damageType, int damageAmount) {
        //Debug.Log("Take damage entered.");
        //Debug.Log(CurrentNpcStatus.ToString() + " " + damageType.ToString());

        bool damageTaken = false;

        if (CurrentNpcStatus == NpcStatus.Hostile && damageType == NpcStatus.Friendly) {
            UpdateHP(_currentHP - damageAmount);
            OnDamageTaken?.Invoke(CurrentNpcStatus, damageAmount);
            damageTaken = true;

        } else if (CurrentNpcStatus == NpcStatus.Friendly && damageType == NpcStatus.Hostile) {
            UpdateHP(_currentHP - damageAmount);
            OnDamageTaken?.Invoke(CurrentNpcStatus, damageAmount);
            damageTaken = true;
        }

        if (_currentHP <= 0) {
        //Debug.Log("NPC died; updating to corpse");
        OnDeath?.Invoke(CurrentNpcStatus);
        UpdateNpcStatus(NpcStatus.Corpse);
        //Destroy(this.gameObject);
        }

        return damageTaken;
    }

    public void UpdateHP(int newHP) {
       // Debug.Log("Update HP called");
        _currentHP = newHP;
        _textHP.text = newHP.ToString();
    }

    public void UpdateMoveSpeed(float newSpeed) {
        _currentMoveSpeed = newSpeed;
    }

    public void UpdateMoveSpeed(float newSpeed, float duration) {
        StartCoroutine(UpdateSpeedRoutine(newSpeed, duration));
    }

    private IEnumerator UpdateSpeedRoutine(float newSpeed, float duration) {
        _currentMoveSpeed = newSpeed;
        yield return new WaitForSeconds(duration);
        _currentMoveSpeed = _startMoveSpeed;
    }

    public void UpdateAttackCd(bool resetCD, float newAttackCd) {
        _startAttackCD = newAttackCd;
        if (resetCD) {
            _currentAttackCD = 0f;
        }
    }

    public void UpdateAttackCd(bool resetCD, float newAttackCd, float duration) {
        StartCoroutine(UpdateAttackCdRoutine(resetCD, newAttackCd, duration));
    }

    private IEnumerator UpdateAttackCdRoutine(bool resetCD, float newAttackCd, float duration) {
        _startAttackCD = newAttackCd;
        if (resetCD) {
            _currentAttackCD = 0f;
        }
        yield return new WaitForSeconds(duration);
        _startAttackCD = _npcAttack.GetAttackCD;
    }

    public void SetFear(bool isFeared, float moveSpeed) {
        if (isFeared) {
            _isFeared = isFeared;
            _currentNpcState = NpcState.Roaming;
            _currentMoveDir = GetRandomMoveDir();
        } else {
            _isFeared = false;
        }

        _currentMoveSpeed = moveSpeed;
    }

    private void MoveToCurrentObjective() {
        if (CurrentGoalObjective != null && this.transform.position != CurrentGoalObjective.transform.position) {
            this.transform.position = Vector3.MoveTowards(this.transform.position, CurrentGoalObjective.transform.position, _currentMoveSpeed * Time.deltaTime);
        }
    }
}
