using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.Timeline;

public class G02_NPC : MonoBehaviour
{
    [SerializeField] private NpcStatus _currentNpcStatus = NpcStatus.Hostile;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _startMoveDirCD = 3f;
    [SerializeField] private float _chaseRange = 3f;
    [SerializeField] private float _targetScanRadius = 10f;
    [SerializeField] private G02_NpcAttack _npcAttack;
    public NpcStatus CurrentNpcStatus { get { return _currentNpcStatus; } set { UpdateNpcStatus(value); } }
    private SpriteRenderer _spriteRenderer;
    private NpcState _currentNpcState = NpcState.None;
    private Transform _player;
    private Transform _currentTarget = null;
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
        _attackRange = _npcAttack.GetAttackRange;
        _startAttackCD = _npcAttack.GetAttackCD;
    }

    private void Start() {
        _player = FindObjectOfType<G02_PlayerController>().transform;

        _xMin = G02_GameManager.Instance.MinXBoundry;
        _xMax = G02_GameManager.Instance.MaxXBoundry;
        _yMin = G02_GameManager.Instance.MinYBoundry;
        _yMax = G02_GameManager.Instance.MaxYBoundry;

        UpdateNpcStatus(_currentNpcStatus);
    }

    private void Update() {
        NpcStateTransition();

        _currentMoveDirCD -= Time.deltaTime;
        _currentAttackCD -= Time.deltaTime;

        if (_currentMoveDirCD <= 0f) {
            _currentMoveDir = GetRandomMoveDir();
            _currentMoveDirCD = _startMoveDirCD;
        }
    }

    private void FixedUpdate() {
        if (_currentNpcState == NpcState.Roaming) {
            Roam();
        } else if (_currentNpcState == NpcState.Chasing) {
            Chase();
        } else if (_currentNpcState == NpcState.Attacking) {
            Attack();
        }
    }


    private void UpdateNpcStatus(NpcStatus newNpcStatus) {
        _currentNpcStatus = newNpcStatus;
        _npcAttack.AttackType = newNpcStatus;
        
        _spriteRenderer.color = newNpcStatus switch {
            NpcStatus.Hostile => Color.red,
            NpcStatus.Corpse => Color.yellow,
            NpcStatus.Friendly => Color.green,
            _ => _spriteRenderer.color
        };

        
        _currentTarget = GetClosestTargetInRange();
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
            return;
        }

        float targetDistance = Vector3.Distance(_currentTarget.position, this.transform.position);

        if (_currentNpcStatus != NpcStatus.None) {
            if (_currentNpcStatus == NpcStatus.Corpse && _currentNpcState != NpcState.None) {
                Debug.Log("Switching to none state");
                _currentNpcState = NpcState.None;
            } else if (_currentNpcState != NpcState.Roaming && targetDistance > _chaseRange) {
                Debug.Log("Switching to roaming state");
                _currentNpcState = NpcState.Roaming;
                _currentTarget = GetClosestTargetInRange();
            } else if (_currentNpcState != NpcState.Chasing && targetDistance > _attackRange && targetDistance <= _chaseRange) {
                Debug.Log("Switching to chasing state");
                _currentNpcState = NpcState.Chasing;
                _currentTarget = GetClosestTargetInRange();
            } else if (_currentNpcState != NpcState.Attacking && targetDistance <= _attackRange) {
                Debug.Log("Switching to attacking state");
                _currentNpcState = NpcState.Attacking;
                _currentTarget = GetClosestTargetInRange();
            } 
        }
    }

    private void Roam() {
        _currentTarget = GetClosestTargetInRange();
        var deltaX = _currentMoveDir.x * Time.deltaTime * _moveSpeed;
        var deltaY = _currentMoveDir.y * Time.deltaTime * _moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, _xMin, _xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, _yMin, _yMax);
        transform.position = new Vector2(newXPos, newYPos);
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
            Debug.Log("Using attack");
            _currentAttackCD = _startAttackCD;
        }
    }
}
