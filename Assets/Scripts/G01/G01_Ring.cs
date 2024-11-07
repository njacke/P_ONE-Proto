using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_Ring : MonoBehaviour
{
    [SerializeField] private RingType _ringType = RingType.None;
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private Sprite _inactiveSprite;
    [SerializeField] private float _rotationTime = 1f;
    [SerializeField] private float _rotationAngle = 90f;
    private SpriteRenderer _spriteRenderer;

    private bool _isBusy = false;
    private bool _isActive = false;

    public enum RingType {
        None,
        Color,
        Shape
    }

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ChangeActiveStatus(true); // change to default ring only if no seleciton input
    }

    public void ChangeActiveStatus(bool activeStatus) {
        _isActive = activeStatus;
        if (_isActive) {
            _spriteRenderer.sprite = _activeSprite;
            _spriteRenderer.sortingOrder = 1;
        } else {
            _spriteRenderer.sprite = _inactiveSprite;
            _spriteRenderer.sortingOrder = 0;
        }

        //Debug.Log("Active status changed to: " + activeStatus + " for ring " + _ringType.ToString());
    }

    private IEnumerator RotateRingRoutine(bool isClockwise) { 
        if (!_isActive || _isBusy) {
            yield break;
        }

        _isBusy = true;

        float currentRotationZ = this.transform.rotation.eulerAngles.z;
        float targetRotationZ;

        if (isClockwise) {
            targetRotationZ = currentRotationZ - _rotationAngle;
        } else {
            targetRotationZ = currentRotationZ + _rotationAngle;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetRotationZ);
        float timeElapsed = 0f;

        while (timeElapsed < _rotationTime) {
            timeElapsed += Time.deltaTime;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, timeElapsed / _rotationTime);
            yield return null;
        }

        this.transform.rotation = targetRotation;
        _isBusy = false;

        //Debug.Log("Rotation complete.");
    }

    public void RotateRing(bool isClockwise) {
        StartCoroutine(RotateRingRoutine(isClockwise));
    }
}
