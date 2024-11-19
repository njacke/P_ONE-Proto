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
        SceneManager.LoadScene("G01_1");
    }

    public void LoadG01V2OnClick() {
        SceneManager.LoadScene("G01_2");
    }

    public void LoadG01V3OnClick() {
        SceneManager.LoadScene("G01_3");
    }

    public void LoadG02OnClick() {
        SceneManager.LoadScene("G02");

    }

    public void LoadG03OnClick() {
        SceneManager.LoadScene("G03");
    }
}
