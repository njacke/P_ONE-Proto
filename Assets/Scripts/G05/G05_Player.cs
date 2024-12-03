using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G05_Player : G05_Token
{
    public PlayerType GetPlayerType { get { return _playerType; } }
    [SerializeField] PlayerType _playerType;
    public enum PlayerType {
        None,
        Hoarder,
        Underdog,
        Moonwalker,
        Performer
    }
}
