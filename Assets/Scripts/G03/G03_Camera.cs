using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G03_Camera : MonoBehaviour
{
    [SerializeField] Transform _player;

    private void Update() {
        this.transform.position = new Vector3(_player.position.x, _player.position.y, this.transform.position.z);
    }
}
