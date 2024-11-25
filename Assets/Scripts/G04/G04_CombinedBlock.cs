using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class G04_CombinedBlock : MonoBehaviour
{
    public static Action<G04_CombinedBlock> OnGridPlacement;
    public static Action<G04_CombinedBlock> OnCurrentValueChanged;
    [SerializeField] private BlockTier _tier;
    [SerializeField] private BlockType _type;
    [SerializeField] private Color _color;
    [SerializeField] private int _totalSize = 0;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _baseValue = 100;
    [SerializeField] private bool _isPremade = false;

    public List<G04_Block> GetBlocks { get { return _blocks; } }
    public bool GetIsOnGrid { get { return _isOnGrid; } }
    public BlockTier GetBlockTier { get { return _tier; } }  
    public BlockType GetBlockType { get { return _type; } }  
    public int GetBlockSize { get { return _totalSize; } }  
    public int GetBlockLevel { get { return _level; } }  
    public int GetBlockBaseValue { get { return _baseValue; } }  
    public float GetBlockCurrentValue { get { return _currentValue; } }  

    private List<G04_Block> _blocks = new List<G04_Block>();
    private bool _isPickedUp = false;
    private bool _isOnGrid = false;
    private G04_Grid _storageGrid;
    private SortingGroup _sortingGroup;
    private Vector3 _initialPos;
    [SerializeField] private float _currentValue = 0f;

    public enum BlockType {
        None,
        Blue,
        Red,
        Yellow,
        Green,
        Orange,
        Purple,
        White
    }

    public enum BlockTier {
        None,
        One,
        Two,
        Three
    }

    private void Awake() {
        _sortingGroup = GetComponent<SortingGroup>();
        _storageGrid = FindObjectOfType<G04_Grid>();
    }

    private void Start() {
        if (_isPremade) {
            var startBlocks = GetComponentsInChildren<G04_Block>();
            foreach (var block in startBlocks) {
                AddBlock(block);
                Debug.Log("Blocks count is " + _blocks.Count);
            }
        }

        _isOnGrid = IsGroupPosOnGrid(this.transform.position);
        if (_isOnGrid) {
            OnGridPlacement?.Invoke(this);
        }

        _totalSize = _blocks.Count;
        // temp solution for block value scaling
        SetBaseValue(_baseValue * (1 << (_totalSize - 1)));
        SetCurrentValue(_baseValue);
    }

    private void Update() {
        if (_isPickedUp) {
            UpdatePos();
        }
    }

    private void OnEnable() {
        G04_CombinedBlock.OnGridPlacement += G04_CombinedBlock_OnGridPlacement;
        G04_UI.OnTurnEnded += G04_UI_OnTurnEnded;
    }

    private void OnDisable() {
        G04_CombinedBlock.OnGridPlacement += G04_CombinedBlock_OnGridPlacement;
        G04_UI.OnTurnEnded -= G04_UI_OnTurnEnded;        
    }

    private void G04_CombinedBlock_OnGridPlacement(G04_CombinedBlock block) {
        if (_isOnGrid) {
            Debug.Log("Placed on grid called with receiver on grid");
            ResolveImmediateEffects();
        }
    }

    private void G04_UI_OnTurnEnded() {
        if (_isOnGrid) {
            ResolveTurnEffects();
            ResolveImmediateEffects();      
        }
    }

    private void UpdatePos() {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseWorldPos.z = this.transform.position.z;

        if (IsGroupPosOnGrid(mouseWorldPos)) {
            Vector3 offset = mouseWorldPos - this.transform.position;
            Vector3[] snappedBlocksPos = new Vector3[_blocks.Count];

            for (int i = 0; i < _blocks.Count; i++) {
                var newPos = _blocks[i].transform.position + offset;
                var snappedPos = _storageGrid.GetClosestCellPos(newPos);
                snappedBlocksPos[i] = snappedPos;
            }

            Vector3 newCenterPos = G04_BlockManager.GetCombinedBlockPosition(snappedBlocksPos);
            this.transform.position = newCenterPos;

            bool isGroupPosTaken = IsGroupPosTaken(this.transform.position);
            foreach (var block in _blocks) {
                block.ToggleEligible(!isGroupPosTaken);
            }
        } else {
            this.transform.position = mouseWorldPos;
            foreach (var block in _blocks) {
                block.ToggleEligible(true);
            }
        }
    }

    private Vector3 GetMouseWorldPosition() {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private bool IsGroupPosEligible() {
        foreach (G04_Block block in _blocks) {
            if (!_storageGrid.IsOnGrid(block.transform.position) || _storageGrid.IsPosTaken(block.transform.position)) {
                return false;
            }            
        }
        return true;
    }

    private bool IsGroupPosOnGrid(Vector3 centerPos) {
        foreach (var block in _blocks) {
            Vector3 blockPos = block.transform.position + centerPos - transform.position;
            if (!_storageGrid.IsOnGrid(blockPos)) {
                return false;
            }            
        }
        return true;
    }

    private bool IsGroupPosTaken(Vector3 centerPos) {
        foreach (var block in _blocks) {
            Vector3 blockPos = block.transform.position + centerPos - transform.position;
            if (_storageGrid.IsPosTaken(blockPos)) {
                return true;
            }            
        }
        return false;
    }
    
    public void AddBlock(G04_Block block) {
        _blocks.Add(block);
        block.CombinedBlock = this;
        block.transform.parent = this.transform;
        block.UpdateColor(_color);
    }

    public void OnPickUp() {
        // set all current block cells to empty (null)
        foreach (var block in _blocks) {           
            (int, int) blockCoor = _storageGrid.GetBlockCoor(block); // returns (-1, -1) if no match
            if (blockCoor.Item1 > -1) {
                _storageGrid.UpdateCellBlockInfo(null, blockCoor);
            }
        }
        
        _initialPos = this.transform.position;
        _sortingGroup.sortingOrder = 1;
        _isOnGrid = false;
        _isPickedUp = true;
    }

    public void OnDropDown() {
        if (IsGroupPosEligible()) {
            // place to new positions (eligible placement)
            foreach (G04_Block block in _blocks) {
                Vector3 blockWorldPos = block.transform.position;
                (int, int) closestCellCoor = _storageGrid.GetClosestCellCoor(blockWorldPos);

                _storageGrid.UpdateCellBlockInfo(block, closestCellCoor);
                block.ToggleEligible(true);
                _isOnGrid = true;
                OnGridPlacement?.Invoke(this);
            }            
        } else {
            // return to previous position (ineligible placement)
            this.transform.position = _initialPos;
            bool isOnGrid = IsGroupPosOnGrid(this.transform.position);

            foreach (var block in _blocks) {
                block.ToggleEligible(true);
                if (isOnGrid) {
                    (int, int) closestCellCoor = _storageGrid.GetClosestCellCoor(block.transform.position);
                    _storageGrid.UpdateCellBlockInfo(block, closestCellCoor);
                }
            }

            if (isOnGrid) {
                _isOnGrid = true;
                OnGridPlacement?.Invoke(this);
                Debug.Log("Is on grid invoked");
            }
        }

        _sortingGroup.sortingOrder = 0;
        _isPickedUp = false;
    }

    public void SetLevel(int newLevel) {
        _level = newLevel;
    }

    public void SetBaseValue(int baseValue) {
        _baseValue = baseValue;
    }

    public void SetCurrentValue(float newValue) {
        _currentValue = newValue;
        OnCurrentValueChanged?.Invoke(this);
    }

    public G04_BlockEffect[] GetBlockEffects() {
        var myEffects = GetComponents<G04_BlockEffect>();
        return myEffects;
    }


    private void ResolveImmediateEffects() {
        SetCurrentValue(_baseValue);

        G04_CombinedBlock[] adjComboBlocks = _storageGrid.GetAdjacentCombinedBlocks(this);
        Debug.Log("Adj combined blocks length: " + adjComboBlocks.Length);
        foreach (var combBlock in adjComboBlocks) {
            var effects = combBlock.GetBlockEffects().Where(x => x.GetResolveType == G04_BlockEffect.ResolveType.Immediate);
            foreach (var effect in effects) {
                effect.ResolveEffect(this);
            }
        }
    }

    private void ResolveTurnEffects() {
        G04_CombinedBlock[] adjComboBlocks = _storageGrid.GetAdjacentCombinedBlocks(this);        
        foreach (var combBlock in adjComboBlocks) {
            var effects = combBlock.GetBlockEffects().Where(x => x.GetResolveType == G04_BlockEffect.ResolveType.Turn);
            foreach (var effect in effects) {
                effect.ResolveEffect(this);
            }
        }
    }
}
