using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    protected Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    protected virtual void Start() {
        Cursor.visible = false;

        //confine cursor if not in Unity
        if (Application.isPlaying) {
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    protected virtual void Update() {
        Vector2 cursorPos = Input.mousePosition;
        _image.rectTransform.position = cursorPos;

        if (!Application.isPlaying) { return; }

        Cursor.visible = false;
    }
}
