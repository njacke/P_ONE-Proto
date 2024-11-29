using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class G05_Enemy : G05_Token
{
    public bool ActionAvailable { get; set; } = true;
    [SerializeField] private int _moveDistanceSpawn = 1;
    [SerializeField] private int _moveDistanceMin = 1;
    [SerializeField] private int _moveDistanceMax = 1;
    private bool _isActive = false;
    public bool TakeAction() {
        if (!ActionAvailable) {
            Debug.Log("Enemy action not available");
            return false;
        }

        // in spawn hallway
        if (!_isActive) {
            var fields = _track.TrackFields.Where(x => x.GetFieldType == G05_Field.FieldType.Enemy
                                                    || x.GetFieldType == G05_Field.FieldType.Main)
                                            .ToArray();
            var graph = _track.GetFieldGraph(fields);
            var targetField = _track.GetFieldsByDistance(graph, CurrentField, _moveDistanceSpawn, true, true).FirstOrDefault();
            if (targetField.CurrentToken != null && targetField.CurrentToken.GetTokenType == TokenType.Enemy) {
                Debug.Log("Invalid move; field is occupied by an enemy");
                return false;
            }

            if (targetField != null) {
                MoveToField(targetField);
                ActionAvailable = false;
                if (targetField.GetFieldType != G05_Field.FieldType.Enemy) {
                    _isActive = true;
                }
                return true;
            } else {
                return false;
            }
        }

        // on main board
        else {
            Debug.Log("Active monster moving");
            var fields = _track.TrackFields.Where(_x => _x.GetFieldType == G05_Field.FieldType.Main).ToArray();
            var graph = _track.GetFieldGraph(fields);
            var moveDistance = UnityEngine.Random.Range(_moveDistanceMin, _moveDistanceMax + 1); // +1 for max exclusive
            
            var players = G05_GameManager.Instance.AllPlayers;
            G05_Field[] closestPath = null;
            int closestPathDist = int.MaxValue;

            foreach (var player in players) {
                if (player.CurrentField.GetFieldType == G05_Field.FieldType.Main) {
                    var path = _track.GetShortestPath(graph, CurrentField, player.CurrentField);
                    if (path.Length < closestPathDist) {
                        closestPath = path;
                    }                    
                }
            }

            if (closestPath == null || closestPath.Length < moveDistance) {
                Debug.Log("No viable move to a player found. Moving to first target by exact distance.");
                var targetField = _track.GetFieldsByDistance(graph, CurrentField, moveDistance, true, false).FirstOrDefault();
                
                if (targetField.CurrentToken != null && targetField.CurrentToken.GetTokenType == TokenType.Enemy) {
                    Debug.Log("Invalid move; field is occupied by an enemy");
                    return false;
                }

                if (targetField != null) {
                    MoveToField(targetField);
                    ActionAvailable = false;
                    return true;
                }

                Debug.Log("No target field found.");
                return false;

            } else {
                var targetField = closestPath[moveDistance]; // index 0 is starting field

                if (targetField.CurrentToken != null && targetField.CurrentToken.GetTokenType == TokenType.Enemy) {
                    Debug.Log("Invalid move; field is occupied by an enemy");
                    return false;
                }
                
                MoveToField(targetField);
                ActionAvailable = false;
                return true;
            }
        }
    }

}
