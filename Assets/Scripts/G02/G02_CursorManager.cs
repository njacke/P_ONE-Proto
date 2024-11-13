using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_CursorManager : CursorManager
{
    [SerializeField] private Sprite _inRangeSprite;
    [SerializeField] private Sprite _outOfRangeSprite;
    private bool _isInRange = false; // additional check so I only change sprite when range indcator changes; TODO: event based

    protected override void Update() {
        base.Update();
        if (_isInRange != G02_PlayerController.Instance.CheckSkillRange()) {
            Debug.Log("Entered cursor update.");
            if (_isInRange) {
                _image.sprite = _outOfRangeSprite;
                _isInRange = false;
            } else {
                _image.sprite = _inRangeSprite;
                _isInRange = true;
            }
        }
    }
}
