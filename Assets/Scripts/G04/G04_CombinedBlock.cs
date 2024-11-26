using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class G04_CombinedBlock : MonoBehaviour
{
    public static Action<G04_CombinedBlock> OnGridPlacement;
    public static Action<G04_CombinedBlock> OnValuesChanged;

    [SerializeField] private BlockTier _tier;
    [SerializeField] private BlockType _type;
    [SerializeField] private Color _color;
    [SerializeField] private int _totalSize = 0;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _sizeValue = 100;
    [SerializeField] private bool _isPremade = false;
    [SerializeField] private float _multiplier = 1;

    public List<G04_Block> GetBlocks { get { return _blocks; } }
    public bool GetIsOnGrid { get { return _isOnGrid; } }
    public BlockTier GetBlockTier { get { return _tier; } }  
    public BlockType GetBlockType { get { return _type; } }  
    public int GetBlockSize { get { return _totalSize; } }  
    public int GetBlockLevel { get { return _level; } }  
    public float GetBlockBaseValue { get { return _baseValue; } }  
    public float GetBlockBonusValue { get { return _bonusValue; } }  
    public float GetBlockMultiplier {get { return _multiplier; } } 
    public float GetBlockTotalValue { get { return _totalValue; } }  

    private List<G04_Block> _blocks = new List<G04_Block>();
    private bool _isPickedUp = false;
    private bool _isOnGrid = false;
    private G04_Grid _grid;
    private SortingGroup _sortingGroup;
    private Vector3 _initialPos;
    private Quaternion _initialRotation;

    [SerializeField] private float _baseValue = 0;
    [SerializeField] private float _bonusValue = 0;
    [SerializeField] private float _totalValue = 0f;

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
    }

    private void Start() {
        _grid = G04_GameManager.Instance.GetGrid;

        if (_isPremade) {
            var startBlocks = GetComponentsInChildren<G04_Block>();
            foreach (var block in startBlocks) {
                AddBlock(block);
                //Debug.Log("Blocks count is " + _blocks.Count);
            }
        }

        _isOnGrid = IsGroupPosOnGrid(this.transform.position);
        if (_isOnGrid) {
            OnGridPlacement?.Invoke(this);
        }

        UpdateValues();      
    }

    private void Update() {
        if (_isPickedUp) {
            UpdatePos();
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
                var snappedPos = _grid.GetClosestCellPos(newPos);
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
            if (!_grid.IsOnGrid(block.transform.position) || _grid.IsPosTaken(block.transform.position)) {
                return false;
            }            
        }
        return true;
    }

    private bool IsGroupPosOnGrid(Vector3 centerPos) {
        foreach (var block in _blocks) {
            Vector3 blockPos = block.transform.position + centerPos - transform.position;
            if (!_grid.IsOnGrid(blockPos)) {
                return false;
            }            
        }
        return true;
    }

    private bool IsGroupPosTaken(Vector3 centerPos) {
        foreach (var block in _blocks) {
            Vector3 blockPos = block.transform.position + centerPos - transform.position;
            if (_grid.IsPosTaken(blockPos)) {
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

    public void OnSelect(bool isSelected) {
        foreach (var block in _blocks) {
            block.ToggleSelected(isSelected);
            if (isSelected) {
                _sortingGroup.sortingOrder = 1;
            } else {
                _sortingGroup.sortingOrder = 0;
            }
        }
    }

    public void OnPickUp() {
        // set all current block cells to empty (null)
        foreach (var block in _blocks) {           
            (int, int) blockCoor = _grid.GetBlockCoor(block); // returns (-1, -1) if no match
            if (blockCoor.Item1 > -1) {
                _grid.UpdateCellBlockInfo(null, blockCoor);
            }
        }
        
        _initialPos = this.transform.position;
        _initialRotation =this.transform.rotation;
        _sortingGroup.sortingOrder = 1;
        _isOnGrid = false;
        _isPickedUp = true;
    }

    public void OnDropDown() {
        if (IsGroupPosEligible()) {
            // place to new positions (eligible placement)
            foreach (G04_Block block in _blocks) {
                Vector3 blockWorldPos = block.transform.position;
                (int, int) closestCellCoor = _grid.GetClosestCellCoor(blockWorldPos);

                _grid.UpdateCellBlockInfo(block, closestCellCoor);
                block.ToggleEligible(true);
                _isOnGrid = true;
                OnGridPlacement?.Invoke(this);
            }            
        } else {
            // return to previous position (ineligible placement)
            this.transform.position = _initialPos;
            this.transform.rotation = _initialRotation;
            bool isOnGrid = IsGroupPosOnGrid(this.transform.position);

            foreach (var block in _blocks) {
                block.ToggleEligible(true);
                if (isOnGrid) {
                    (int, int) closestCellCoor = _grid.GetClosestCellCoor(block.transform.position);
                    _grid.UpdateCellBlockInfo(block, closestCellCoor);
                }
            }

            if (isOnGrid) {
                _isOnGrid = true;
                OnGridPlacement?.Invoke(this);
                //Debug.Log("Is on grid invoked");
            }
        }

        _sortingGroup.sortingOrder = 0;
        _isPickedUp = false;
    }

    public void SetLevel(int newLevel) {
        _level = newLevel;
    }

    public void SetBonusValue(float newValue) {
        _bonusValue = newValue;
    }

    public void SetMultiplier(float newValue) {
        _multiplier = newValue;
    }

    public void SetCurrentValue(float newValue) {
        _totalValue = newValue;
        //Debug.Log("New current value called: " + newValue);
        OnValuesChanged?.Invoke(this);
    }

    public void UpdateValues() {
        Debug.Log("Bonus Value pre-update:" + _bonusValue);
        _totalSize = _blocks.Count;
        _baseValue = _totalSize * _sizeValue;
        var currentValue = (_baseValue + _bonusValue) * _multiplier;
        SetCurrentValue(currentValue);
        Debug.Log("Bonus Value post-update:" + _bonusValue);
    }

    public G04_BlockEffect[] GetBlockEffects() {
        var myEffects = GetComponentsInChildren<G04_BlockEffect>();
        return myEffects;
    }
}
