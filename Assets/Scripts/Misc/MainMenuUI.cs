using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private void Start() {
        Cursor.visible = true;        
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void QuitOnClick() {
        Application.Quit();
    }

    public void LoadG01V1OnClick() {
        MenuData.SelectedGame = MenuData.Games.G01_1;
        SceneManager.LoadScene("PlayMenu");
    }

    public void LoadG01V2OnClick() {
        MenuData.SelectedGame = MenuData.Games.G01_2;
        SceneManager.LoadScene("PlayMenu");
    }

    public void LoadG01V3OnClick() {
        MenuData.SelectedGame = MenuData.Games.G01_3;
        SceneManager.LoadScene("PlayMenu");
    }

    public void LoadG02OnClick() {
        MenuData.SelectedGame = MenuData.Games.G02;
        SceneManager.LoadScene("PlayMenu");
    }

    public void LoadG03OnClick() {
        MenuData.SelectedGame = MenuData.Games.G03;
        SceneManager.LoadScene("PlayMenu");
    }

    public void LoadG04OnClick() {
        MenuData.SelectedGame = MenuData.Games.G04;
        SceneManager.LoadScene("PlayMenu");
    }

    public void LoadG05OnClick() {
        MenuData.SelectedGame = MenuData.Games.G05;
        SceneManager.LoadScene("PlayMenu");
    }

    public static class MenuData {
        public static Games SelectedGame = Games.None;
        public enum Games {
            None,
            G01_1,
            G01_2,
            G01_3,
            G02,
            G03,
            G04,
            G05
        }

        public static Dictionary<Games, string> GamesScenesDict = new() {
                    { Games.None, "MainMenu" },
                    { Games.G01_1, "G01_1" },
                    { Games.G01_2, "G01_2" },
                    { Games.G01_3, "G01_3" },
                    { Games.G02, "G02" },
                    { Games.G03, "G03" },
                    { Games.G04, "G04" },
                    { Games.G05, "G05" },
            };

        public static Dictionary<Games, string> GamesNamesDict = new() {
                    { Games.None, "Main Menu" },
                    { Games.G01_1, "G01 - Thought Shaper (v1)" },
                    { Games.G01_2, "G01 - Thought Shaper (v2)" },
                    { Games.G01_3, "G01 - Thought Shaper (v2)" },
                    { Games.G02, "G02 - Nekropik" },
                    { Games.G03, "G03 - Out of Space" },
                    { Games.G04, "G04 - Inventorio" },
                    { Games.G05, "G05 - Ludocris" },
            };
    }
}
