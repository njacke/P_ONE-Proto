using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_CircleController : MonoBehaviour
{
    [SerializeField] private G01_Ring _ringColor;
    [SerializeField] private G01_Ring _ringShape;
    [SerializeField] private G01_Launcher _launcher;
    private G01_Ring.RingType _selectedRing = G01_Ring.RingType.Shape;

    private void Update() {
        //SelectionInput();
        if (G01_GameManager.Instance.GetGameVersion == G01_GameManager.GameVersion.DirectionalLauncher) {
            LaunchDirectionInput();
        }
        NoSelectionInput();
    }

    private void LaunchDirectionInput() {
        if (Input.GetKey(KeyCode.Alpha1)) {
            _launcher.SetProjectileDir(0);
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            _launcher.SetProjectileDir(1);
        }
        if (Input.GetKey(KeyCode.Alpha3)) {
            _launcher.SetProjectileDir(2);
        }
        if (Input.GetKey(KeyCode.Alpha4)) {
            _launcher.SetProjectileDir(3);
        }
    }

    private void NoSelectionInput() {
        if (G01_GameManager.Instance.GetGameVersion != G01_GameManager.GameVersion.RandomLauncher && Input.GetKeyDown(KeyCode.Space)) {
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
