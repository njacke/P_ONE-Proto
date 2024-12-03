using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class G05_Track : MonoBehaviour
{
    public G05_Field[] TrackFields { get; private set; }
    public Dictionary<Vector3, G05_Field> _posFieldDict;

    private void Awake() {
        TrackFields = FindObjectsOfType<G05_Field>();

        _posFieldDict = new Dictionary<Vector3, G05_Field>();

        foreach (var field in TrackFields) {
            _posFieldDict.Add(field.transform.position, field);            
        }

        foreach (var field in TrackFields) {
            field.AdjacentFields = GetAdjacentFields(field);
        }

    }

    public Dictionary<G05_Field, G05_Field[]> GetFieldGraph(G05_Field[] fields) {
        Dictionary<G05_Field, G05_Field[]> graph = new();

        foreach (var field in fields) {
            if (field == null) {
                Debug.LogError("Null field in fields array!");
                continue;
            }

            graph[field] = field.AdjacentFields ?? Array.Empty<G05_Field>();
        }

        return graph;
    }

    public G05_Field[] GetAdjacentFields(G05_Field field) {
        if (field == null) {
            Debug.LogWarning("Field is null. Cannot find adjacent fields.");
            return Array.Empty<G05_Field>();
        }

        Vector3[] offsets = {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
        };

        List<G05_Field> adjacentFields = new();

        foreach (var offset in offsets) {
            Vector3 adjPos = field.transform.position + offset;
            _posFieldDict.TryGetValue(adjPos, out var adjField);
            if (adjField != null) {
                adjacentFields.Add(adjField);
            }
        }

        return adjacentFields.ToArray();
    }

    public G05_Field[] GetShortestPath(Dictionary<G05_Field, G05_Field[]> graph, G05_Field startField, G05_Field endField) {
        Queue<G05_Field> queue = new();
        Dictionary<G05_Field, G05_Field> predecessors = new();
        HashSet<G05_Field> visited = new();

        queue.Enqueue(startField);
        visited.Add(startField);

        while (queue.Count > 0) {
            var current = queue.Dequeue();

            // reconstruct the path
            if (current == endField) {
                List<G05_Field> path = new();
                while (current != startField) {
                    path.Add(current);
                    current = predecessors[current];
                }
                path.Add(startField);
                path.Reverse();
                return path.ToArray();
            }

            // search adjacent fields
            foreach (var adjField in graph[current]) {
                if (!visited.Contains(adjField) && graph.ContainsKey(adjField)) {
                    queue.Enqueue(adjField);
                    visited.Add(adjField);
                    predecessors[adjField] = current;
                }
            }
        }
        
        Debug.Log("No path found between start and end field.");
        return null;
    }

    public G05_Field[] GetFieldsByDistance(Dictionary<G05_Field, G05_Field[]> graph, G05_Field startField, int distance, bool isExact, bool isForward, bool ignoreShortcuts) {
        Queue<(G05_Field, int, int)> queue = new();  // store field, distance, lastFieldIndex
        HashSet<G05_Field> visited = new();
        List<G05_Field> result = new();

        queue.Enqueue((startField, 0, startField.GetFieldIndex));
        visited.Add(startField);

        while (queue.Count > 0) {
            var (current, dist, lastFieldIndex) = queue.Dequeue();

            if (isExact && dist == distance || !isExact && dist <= distance) {
                result.Add(current);
            }

            if (dist < distance) {
                foreach (var adjField in graph[current]) {                    
                    // do shortcut check if needed
                    bool shortcutCheck = true;
                    if (!ignoreShortcuts) {
                        if (current.GetFieldType == G05_Field.FieldType.Main && adjField.GetFieldType == G05_Field.FieldType.Shortcut &&
                            !adjField.IsShortcutEnabled && adjField.GetFieldIndex >= lastFieldIndex) {
                            shortcutCheck = false;
                        } else if (current.GetFieldType == G05_Field.FieldType.Shortcut && adjField.GetFieldType == G05_Field.FieldType.Main &&
                            !adjField.IsShortcutEnabled && adjField.GetFieldIndex <= lastFieldIndex) {
                            shortcutCheck = false;
                        }
                    }

                    // if moving forward check that index is >= than previous
                    if (!visited.Contains(adjField) && graph.ContainsKey(adjField) &&
                            (!isForward || adjField.GetFieldIndex >= lastFieldIndex) && shortcutCheck) {

                        queue.Enqueue((adjField, dist + 1, adjField.GetFieldIndex));
                        visited.Add(adjField);                        
                    }
                }
            }
        }

        return result.ToArray();
    }
}