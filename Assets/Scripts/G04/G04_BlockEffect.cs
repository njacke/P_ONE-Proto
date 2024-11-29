using UnityEngine;

public abstract class G04_BlockEffect : MonoBehaviour
{
    public TargetType GetTargetType { get { return _targetType; } }
    public ResolveType GetResolveType { get { return _resolveType; } }
    [SerializeField] private TargetType _targetType;
    [SerializeField] private ResolveType _resolveType;
    protected G04_CombinedBlock _myBlock;
    protected G04_Grid _grid;

    public enum TargetType {
        None,
        Self,
        Other
    }

    public enum ResolveType {
        None,
        Immediate,
        Turn
    }

    private void Awake() {
        _myBlock = GetComponent<G04_CombinedBlock>();
    }

    private void Start() {
        _grid = G04_GameManager.Instance.GetGrid;
    }

    public abstract void ResolveEffect(G04_CombinedBlock target, int effectLevel);
}
