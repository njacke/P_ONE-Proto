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

    public G05_Field[] GetShortestPath(Dictionary<G05_Field, G05_Field[]> graph, G05_Field startField, G05_Field endField, bool ignoreGates) {
        Queue<G05_Field> queue = new();
        Dictionary<G05_Field, G05_Field> predecessors = new();
        HashSet<G05_Field> visited = new();

        queue.Enqueue(startField);
        visited.Add(startField);

        while (queue.Count > 0) {
            var current = queue.Dequeue();

            // reconstruct path when target is reached
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

            // check adjacent fields
            foreach (var adjField in graph[current]) {
                // gate check
                if (!ignoreGates && adjField.IsGated && current.IsGated) {
                    continue;
                }

                // other constraints check
                if (!visited.Contains(adjField) && graph.ContainsKey(adjField)) {
                    queue.Enqueue(adjField);
                    visited.Add(adjField);
                    predecessors[adjField] = current;
                }
            }
        }

        // return an empty array if no path found
        return Array.Empty<G05_Field>();
    }

    public G05_Field[] GetFieldsByDistance(Dictionary<G05_Field, G05_Field[]> graph, G05_Field startField, int distance, bool isExact, bool isForwardOnly, bool ignoreGates) {
        Queue<(G05_Field current, G05_Field previous, int dist)> queue = new(); // store (current field, previous field, distance)
        HashSet<(G05_Field, int)> visited = new(); // track visited (field, distance) combinations
        List<G05_Field> result = new();

        queue.Enqueue((startField, null, 0)); // start with no previous field
        visited.Add((startField, 0));

        while (queue.Count > 0) {
            var (current, previous, dist) = queue.Dequeue();

            // add to result if the distance matches criteria
            if ((isExact && dist == distance) || (!isExact && dist <= distance)) {
                result.Add(current);
            }

            if (dist < distance) {
                foreach (var adjField in graph[current]) {
                    // skip if moving back to the previous field
                    if (adjField == previous) {
                        continue;
                    }

                    bool isValidMove = true;

                    // direction check
                    if (isForwardOnly) {
                        // only move to higher index
                        isValidMove = adjField.GetFieldIndex > current.GetFieldIndex;
                    } else {
                        bool isMovingForward = adjField.GetFieldIndex > current.GetFieldIndex;
                        bool isMovingBackward = adjField.GetFieldIndex < current.GetFieldIndex;

                        //consistent direction check
                        if (previous != null) {
                            bool wasMovingForward = previous.GetFieldIndex < current.GetFieldIndex;
                            bool wasMovingBackward = previous.GetFieldIndex > current.GetFieldIndex;

                            if (isMovingForward && !wasMovingForward) {
                                isValidMove = false;
                            } else if (isMovingBackward && !wasMovingBackward) {
                                isValidMove = false;
                            }
                        }
                    }

                    // gate check
                    if (!ignoreGates && current.IsGated && adjField.IsGated) {
                        isValidMove = false;
                    }
                    
                    // graph key check
                    if (!graph.ContainsKey(adjField)) {
                        isValidMove = false;
                    }

                    // visited check at this distance
                    if (visited.Contains((adjField, dist + 1))) {
                        isValidMove = false;
                    }

                    // check all conditions before enqueuing
                    if (isValidMove) {
                        queue.Enqueue((adjField, current, dist + 1));
                        visited.Add((adjField, dist + 1));                        
                    }
                }
            }
        }

        return result.ToArray();
    }
}