using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_PlayerController : Singleton<G03_PlayerController>
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private bool _isGridMovement = false;
    private G03_BombLauncher _bombLauncher;
    private Vector3 _newPos = Vector3.zero;
    private bool _isMoving = false;
    private float _xMin;
    private float _xMax;
    private float _yMin;
    private float _yMax;

    public Vector3 GetPlayerPos { get { return this.transform.position; } }

    protected override void Awake() {
        base.Awake();
        _bombLauncher = GetComponent<G03_BombLauncher>();
        _newPos = this.transform.position;
    }

    private void Start() {
        _xMin = G03_GameManager.Instance.MinXBoundry;
        _xMax = G03_GameManager.Instance.MaxXBoundry;
        _yMin = G03_GameManager.Instance.MinYBoundry;
        _yMax = G03_GameManager.Instance.MaxYBoundry;
    }

    private void Update() {
        if (!_isGridMovement) {
            Move();
        } else {
            if (!_isMoving) {
                MoveGridInput();
            }
            MoveGrid();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            _bombLauncher.LaunchBomb();
        }
    }

    private void Move() {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * _moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * _moveSpeed;

        var newPosX = Mathf.Clamp(this.transform.position.x + deltaX, _xMin, _xMax);
        var newPosY = Mathf.Clamp(this.transform.position.y + deltaY, _yMin, _yMax);

        this.transform.position = new Vector3(newPosX, newPosY, this.transform.position.z);
    }

    private void MoveGridInput() {
        if (Input.GetKeyDown(KeyCode.W)) {
            _newPos = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
        } else if (Input.GetKeyDown(KeyCode.A)) {
            _newPos = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);
        } else if (Input.GetKeyDown(KeyCode.S)) {
            _newPos = new Vector3(this.transform.position.x, this.transform.position.y - 1, this.transform.position.z);
        } else if (Input.GetKeyDown(KeyCode.D)) {
            _newPos = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
        }
    }

    private void MoveGrid() {
        if (this.transform.position != _newPos) {
            _isMoving = true;
            this.transform.position = Vector3.MoveTowards(this.transform.position, _newPos, _moveSpeed * Time.deltaTime);
        } else {
            _isMoving = false;
        }
    }
}
