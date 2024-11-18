using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class G03_GameManager : Singleton<G03_GameManager>
{
    [SerializeField] private float _padding = 0f;
    [SerializeField] private float _mapSizeX = 10;
    [SerializeField] private float _mapSizeY = 10;
    [SerializeField] private G03_Objective[] _gameObjectives;

    public G03_Objective[] GetGameObjectives { get { return _gameObjectives; } }
    public float GetMapSizeX { get { return _mapSizeX; } }
    public float GetMapSizeY { get { return _mapSizeY; } }
    public float MinXBoundry { get; private set; }
    public float MaxXBoundry { get; private set; }
    public float MinYBoundry { get; private set; }
    public float MaxYBoundry { get; private set; }


    protected override void Awake() {
        base.Awake();
        SetUpMoveBoundries();
        SetObjectiveCounts();
    }

    private void OnEnable() {
        G03_Objective.OnDeath += G03_Objective_OnDeath;
    }

    private void OnDisable() {
        G03_Objective.OnDeath -= G03_Objective_OnDeath;
    }

    private void G03_Objective_OnDeath(G03_Objective sender) {
        int friendlyBasesCount = 0;
        int hostileBasesCount = 0;

        foreach (var objective in _gameObjectives) {
            if (objective != null && objective.GetIsBase == true) {
                if (objective.GetObjectiveType == G03_Objective.ObjectiveType.Friendly) {
                    friendlyBasesCount++;
                } else if (objective.GetObjectiveType == G03_Objective.ObjectiveType.Hostile) {
                    hostileBasesCount++;
                }
            }
        }

        Debug.Log("Friendly bases count: " + friendlyBasesCount);
        Debug.Log("Hostile bases count: " + hostileBasesCount);

        if (friendlyBasesCount <= 0) {
            Debug.Log("Game over; hostile wins. Reloading.");
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        } else if (hostileBasesCount <= 0) {
            Debug.Log("Game over; friendly wins. Reloading.");
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
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

    private void SetObjectiveCounts() {
        int friendlyNormalObjCount = 0;
        int friendlyBaseObjCount = 0;
        int hostileNormalObjCount = 0;
        int hostileBaseObjCount = 0;

        foreach (var objective in _gameObjectives) {
            if (objective.GetObjectiveType == G03_Objective.ObjectiveType.Friendly) {
                if (objective.GetIsBase == true) {
                    friendlyBaseObjCount++;
                    objective.ObjectiveCount = friendlyBaseObjCount;
                } else {
                    friendlyNormalObjCount++;
                    objective.ObjectiveCount = friendlyNormalObjCount;
                }
            } else if (objective.GetObjectiveType == G03_Objective.ObjectiveType.Hostile) {
                if (objective.GetIsBase == true) {
                    hostileBaseObjCount++;
                    objective.ObjectiveCount = hostileBaseObjCount;
                } else {
                    hostileNormalObjCount++;
                    objective.ObjectiveCount = hostileNormalObjCount;
                }
            }
        }
    }

}

