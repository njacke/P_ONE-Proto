using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private GameObject _G01_1_info;
    [SerializeField] private GameObject _G01_2_info;
    [SerializeField] private GameObject _G01_3_info;
    [SerializeField] private GameObject _G02_info;
    [SerializeField] private GameObject _G03_info;
    [SerializeField] private GameObject _G04_info;
    [SerializeField] private GameObject _G05_info;

    private Dictionary<MainMenuUI.MenuData.Games, GameObject> _gamesInfoDict;

    private void Awake() {
        _titleText.text = MainMenuUI.MenuData.GamesNamesDict[MainMenuUI.MenuData.SelectedGame];  
        _gamesInfoDict = new Dictionary<MainMenuUI.MenuData.Games, GameObject>() {
            { MainMenuUI.MenuData.Games.G01_1, _G01_1_info },
            { MainMenuUI.MenuData.Games.G01_2, _G01_2_info },
            { MainMenuUI.MenuData.Games.G01_3, _G01_3_info },
            { MainMenuUI.MenuData.Games.G02, _G02_info },
            { MainMenuUI.MenuData.Games.G03, _G03_info },
            { MainMenuUI.MenuData.Games.G04, _G04_info },
            { MainMenuUI.MenuData.Games.G05, _G05_info },
        };

        var newGameInfo = Instantiate(_gamesInfoDict[MainMenuUI.MenuData.SelectedGame], this.transform.position, Quaternion.identity);
        newGameInfo.transform.SetParent(this.transform);
    }

    private void Start() {
        Cursor.visible = true;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenu");
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene(MainMenuUI.MenuData.GamesScenesDict[MainMenuUI.MenuData.SelectedGame]);
        }
    }

    public void BackOnClick() {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayOnClick() {
        SceneManager.LoadScene(MainMenuUI.MenuData.GamesScenesDict[MainMenuUI.MenuData.SelectedGame]);
    }
}
