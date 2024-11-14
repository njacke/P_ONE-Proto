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

    public void LoadG01OnClick() {
        SceneManager.LoadScene("G01");
    }

    public void LoadG02OnClick() {
        SceneManager.LoadScene("G02");
    }
}
