using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_CircleController : MonoBehaviour
{
    [SerializeField] private G01_Ring _ringColor;
    [SerializeField] private G01_Ring _ringShape;
    [SerializeField] private G01_Launcher _launcher;
    private G01_Ring.RingType _selectedRing = G01_Ring.RingType.Shape;

    private void Update()
    {
        //SelectionInput();
        NoSelectionInput();
    }

    private void NoSelectionInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            _launcher.FireProjectile();
        }
        if (Input.GetKey(KeyCode.Q)) {
            _ringColor.RotateRing(false);
        }
        if (Input.GetKey(KeyCode.W)) {
            _ringColor.RotateRing(true);
        }
        if (Input.GetKey(KeyCode.A)) {
            _ringShape.RotateRing(false);
        }
        if (Input.GetKey(KeyCode.S)) {
            _ringShape.RotateRing(true);
        }
    }

    private void SelectionInput() {
        if (Input.GetKey(KeyCode.W))
        {
            if (_selectedRing == G01_Ring.RingType.Shape)
            {
                _selectedRing = G01_Ring.RingType.Color;
                _ringShape.ChangeActiveStatus(false);
                _ringColor.ChangeActiveStatus(true);
            }
        }

        else if (Input.GetKey(KeyCode.S))
        {
            if (_selectedRing == G01_Ring.RingType.Color)
            {
                _selectedRing = G01_Ring.RingType.Shape;
                _ringColor.ChangeActiveStatus(false);
                _ringShape.ChangeActiveStatus(true);
            }
        }

        else if (Input.GetKey(KeyCode.D))
        {
            if (_selectedRing == G01_Ring.RingType.Color)
            {
                _ringColor.RotateRing(true);
            }
            else if (_selectedRing == G01_Ring.RingType.Shape)
            {
                _ringShape.RotateRing(true);
            }
        }

        else if (Input.GetKey(KeyCode.A))
        {
            if (_selectedRing == G01_Ring.RingType.Color)
            {
                _ringColor.RotateRing(false);
            }
            else if (_selectedRing == G01_Ring.RingType.Shape)
            {
                _ringShape.RotateRing(false);
            }
        }
    }
}
