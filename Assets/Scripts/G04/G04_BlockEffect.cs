using UnityEngine;

public abstract class G04_BlockEffect : MonoBehaviour
{
    public ResolveType GetResolveType { get { return _resolveType; } }
    [SerializeField] private ResolveType _resolveType;
    protected G04_CombinedBlock _myBlock;

    public enum ResolveType {
        None,
        Immediate,
        Turn
    }

    private void Awake() {
        _myBlock = GetComponent<G04_CombinedBlock>();
    }

    public abstract void ResolveEffect(G04_CombinedBlock target);
}
