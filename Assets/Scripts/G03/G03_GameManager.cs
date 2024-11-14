using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class G03_GameManager : Singleton<G03_GameManager>
{
    [SerializeField] private float _padding = 0f;
    [SerializeField] private float _mapSizeX = 10;
    [SerializeField] private float _mapSizeY = 10;

    public float MinXBoundry { get; private set; }
    public float MaxXBoundry { get; private set; }
    public float MinYBoundry { get; private set; }
    public float MaxYBoundry { get; private set; }


    protected override void Awake() {
        base.Awake();
        SetUpMoveBoundries();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void SetUpMoveBoundries() {
        MinXBoundry = -_mapSizeX + _padding;
        MaxXBoundry = +_mapSizeX - _padding;
        MinYBoundry = -_mapSizeY + _padding;
        MaxYBoundry = +_mapSizeY - _padding;
    }

}

