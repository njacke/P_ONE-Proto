using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class G02_GameManager : Singleton<G02_GameManager>
{
    public static Action<int, int, int> OnXpChange;

    [SerializeField] private float _padding = 1f;
    [SerializeField] private float _lvlUpXpReq = 5f;
    [SerializeField] private float _lvlUpXpReqAdj = .2f;
    [SerializeField] private G02_UpgradeUI _upgradeUI;
    [SerializeField] private Collider2D[] _borderColliders;

    public float MinXBoundry { get; private set; }
    public float MaxXBoundry { get; private set; }
    public float MinYBoundry { get; private set; }
    public float MaxYBoundry { get; private set; }

    private int _currentLvlXp = 0;
    private int _currentLvl = 1;

    protected override void Awake() {
        base.Awake();
        SetUpMoveBoundries();

        _borderColliders[0].offset = new Vector2(0, MaxYBoundry + 1);
        _borderColliders[1].offset = new Vector2(0, MinYBoundry - 1);
        _borderColliders[2].offset = new Vector2(MaxXBoundry + 1, 0);
        _borderColliders[3].offset = new Vector2(MinXBoundry - 1, 0);
    }

    private void Start() {
        Time.timeScale = 1f;
        OnXpChange?.Invoke(_currentLvl, _currentLvlXp, Mathf.CeilToInt(_lvlUpXpReq));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Time.timeScale = 1f;
            SceneManager.LoadScene("PlayMenu");
        }
        
        if (Input.GetKeyDown(KeyCode.R)) {
            Time.timeScale = 1f;
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }

    private void SetUpMoveBoundries() {
        Camera gameCamera = Camera.main;
        MinXBoundry = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + _padding;
        MaxXBoundry = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - _padding;
        MinYBoundry = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + _padding;
        MaxYBoundry = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - _padding;
    }

    private void OnEnable() {
        G02_PlayerController.OnHpChange += G02_PlayerController_OnHpChange;   
        G02_NPC.OnDeath += G02_NPC_OnDeath;     
        G02_UpgradeUI.OnUpgradeChosen += G02_UpgradeUI_OnUpgradeChosen;
    }

    private void OnDisable() {
        G02_PlayerController.OnHpChange -= G02_PlayerController_OnHpChange;       
        G02_NPC.OnDeath -= G02_NPC_OnDeath;     
        G02_UpgradeUI.OnUpgradeChosen -= G02_UpgradeUI_OnUpgradeChosen;
    }


    private void G02_PlayerController_OnHpChange(int currentHp) {
        if (currentHp <= 0) {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }

    private void G02_NPC_OnDeath(G02_NPC.NpcStatus status) {
        if (status == G02_NPC.NpcStatus.Hostile) {
            _currentLvlXp++;
            if (_currentLvlXp >= _lvlUpXpReq) {
                _currentLvl++;
                _currentLvlXp = 0;
                _lvlUpXpReq *= 1 + _lvlUpXpReqAdj;
                Time.timeScale = 0f;
                _upgradeUI.gameObject.SetActive(true);
            }

            OnXpChange?.Invoke(_currentLvl, _currentLvlXp, Mathf.CeilToInt(_lvlUpXpReq));
        }
    }

    private void G02_UpgradeUI_OnUpgradeChosen() {
        Time.timeScale = 1f;
        _upgradeUI.gameObject.SetActive(false);
    }
}

