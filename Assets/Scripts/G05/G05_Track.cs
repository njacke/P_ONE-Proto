using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class G05_Track : MonoBehaviour
{
    public G05_Field[] TrackFields { get; private set; }
    private Dictionary<Vector3, G05_Field> _posFieldDict;

    private void Start() {
        TrackFields = FindObjectsOfType<G05_Field>();
        _posFieldDict = new Dictionary<Vector3, G05_Field>();
        foreach (var field in TrackFields) {
            _posFieldDict.Add(field.transform.position, field);
        }

        foreach (var field in TrackFields) {
            field.AdjacentFields = GetAdjacentFields(field);
        }
    }

    public G05_Field[] GetAdjacentFields(G05_Field field) {
        if (field == null) {
            Debug.LogWarning("Field is null. Cannot find adjacent fields.");
            return Array.Empty<G05_Field>();
        }

        Vector3[] offsets = {
            new(1, 0, 0),  // top
            new(-1, 0, 0), // bottom
            new(0, 1, 0),  // right
            new(0, -1, 0)  // left
        };

        List<G05_Field> adjacentFields = new();

        foreach (var offset in offsets) {
            if (_posFieldDict.TryGetValue(field.transform.position + offset, out G05_Field adjField) && adjField != null) {
                adjacentFields.Add(adjField);
            }
        }

        return adjacentFields.ToArray();
    }

    public G05_Field[] GetEligibleFields(G05_Token playerToken) {
        var diceRoll = G05_GameManager.Instance.GetDice.CurrentValue;
        if (diceRoll < 1) {
            Debug.Log("Dice roll not eligible: " + diceRoll);
            return Array.Empty<G05_Field>();
        }

        List<G05_Field> currentFields = new() {
            playerToken.CurrentField
        };

        for (int i = 0; i < diceRoll; i++) {
            List<G05_Field> newFields = new();
            foreach (var field in currentFields) {
                var adjFields = GetAdjacentFields(field);              
                foreach (var adjField in adjFields) {
                    if (adjField.FieldIndex > field.FieldIndex) {
                        newFields.Add(adjField);                       
                    }
                }
            }

            currentFields = newFields;
            Debug.Log("Eligible fields count: " + currentFields.Count);
        }

        return currentFields.ToArray();
    }
}
