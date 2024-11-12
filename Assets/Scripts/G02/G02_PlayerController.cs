using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private G02_Revive _reviveSkill;

    private float _xMin;
    private float _xMax;
    private float _yMin;
    private float _yMax;

    private void Start() {
        _xMin = G02_GameManager.Instance.MinXBoundry;
        _xMax = G02_GameManager.Instance.MaxXBoundry;
        _yMin = G02_GameManager.Instance.MinYBoundry;
        _yMax = G02_GameManager.Instance.MaxYBoundry;
    }

    private void Update() {
        Move();
        SkillInput();
    }

    private void Move() {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * _moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * _moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, _xMin, _xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, _yMin, _yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SkillInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            _reviveSkill.UseRevive(GetMouseWorldPos());
        }
    }

    private Vector3 GetMouseWorldPos() {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.nearClipPlane;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        return mouseWorldPosition;
    }
}
