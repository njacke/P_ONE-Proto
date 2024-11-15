using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class G03_NpcManager : Singleton<G03_NpcManager>
{
    [SerializeField] private float _startSpawnCd = 1f;
    [SerializeField] private G03_NPC[] _npcPrefabs;
    [SerializeField] Transform[] _hostileSpawnPoints;
    [SerializeField] Transform[] _friendlySpawnPoints;
    private Dictionary<G03_Objective, Vector3> _objectiveSpawnPointMap = new();
    private float _currentSpawnCd;

    protected override void Awake() {
        base.Awake();
        _currentSpawnCd = _startSpawnCd;        
    }

    private void Start() {
        MapObjectivesToSpawnPoints();
    }

    private void Update() {
        _currentSpawnCd -= Time.deltaTime;
        if (_currentSpawnCd <= 0f) {
            SpawnNpc(G03_NPC.NpcStatus.Hostile);
            SpawnNpc(G03_NPC.NpcStatus.Friendly);
            _currentSpawnCd = _startSpawnCd;
        }
    }

    private void SpawnNpc(G03_NPC.NpcStatus npcStatus) {
        List<G03_Objective> eligibleObjectives = null;
        G03_Objective objective = null;

        if (npcStatus == G03_NPC.NpcStatus.Hostile) {
            eligibleObjectives = G03_GameManager.Instance.GetGameObjectives
                                .Where(x => !x.GetIsBase && x.GetObjectiveType == G03_Objective.ObjectiveType.Friendly)
                                .ToList();      
        } else if (npcStatus == G03_NPC.NpcStatus.Friendly) {
            eligibleObjectives = G03_GameManager.Instance.GetGameObjectives
                                .Where(x => !x.GetIsBase && x.GetObjectiveType == G03_Objective.ObjectiveType.Hostile)
                                .ToList();  
        }

        if (eligibleObjectives.Count == 0) {
            objective = GetBaseObjective(npcStatus);
        } else {
            int rndIndex = UnityEngine.Random.Range(0, eligibleObjectives.Count);
            objective = eligibleObjectives[rndIndex];
        }

        if (objective != null) {
            int rndIndex = UnityEngine.Random.Range(0, _npcPrefabs.Length);
            var newNpc = Instantiate(_npcPrefabs[rndIndex], _objectiveSpawnPointMap[objective], Quaternion.identity);
            newNpc.CurrentNpcStatus = npcStatus;
            newNpc.CurrentGoalObjective = objective;
            newNpc.BaseObjective = GetBaseObjective(npcStatus);
        }
    }

    private void MapObjectivesToSpawnPoints() {
        G03_Objective[] _currentObjectives = G03_GameManager.Instance.GetGameObjectives;

        foreach (var objective in _currentObjectives) {
            List<Transform> _eligibleSpawnPoints = new();
            float? minDistance = null;
            Vector3? closestSpawn = null;

            if (objective.GetObjectiveType == G03_Objective.ObjectiveType.Friendly) {
                _eligibleSpawnPoints = _hostileSpawnPoints.ToList();
            } else if (objective.GetObjectiveType == G03_Objective.ObjectiveType.Hostile) {
                _eligibleSpawnPoints = _friendlySpawnPoints.ToList();
            }

            if (_eligibleSpawnPoints.Count != 0) {
                foreach (var spawnPoint in _eligibleSpawnPoints) {
                    var spawnPointDist = Vector3.Distance(objective.transform.position, spawnPoint.position);
                    if (minDistance == null) {
                        minDistance = spawnPointDist;
                        closestSpawn = spawnPoint.position;
                    } else if (minDistance > Vector3.Distance(objective.transform.position, spawnPoint.position)) {
                        minDistance = spawnPointDist;
                        closestSpawn = spawnPoint.position;
                    }
                }                
            }

            _objectiveSpawnPointMap.Add(objective, closestSpawn.GetValueOrDefault());
        }
    }

    public G03_Objective GetBaseObjective(G03_NPC.NpcStatus npcStatus) {
        G03_Objective baseObjective = null;
        G03_Objective[] _currentObjectives = G03_GameManager.Instance.GetGameObjectives;

        if (npcStatus == G03_NPC.NpcStatus.Hostile) {
            baseObjective = _currentObjectives
                .Where(x => x.GetIsBase == true && x.GetObjectiveType == G03_Objective.ObjectiveType.Friendly)
                .FirstOrDefault();
        } else if (npcStatus == G03_NPC.NpcStatus.Friendly) {
            baseObjective = _currentObjectives
                .Where(x => x.GetIsBase == true && x.GetObjectiveType == G03_Objective.ObjectiveType.Hostile)
                .FirstOrDefault();
        }

        return baseObjective;
    }

}
