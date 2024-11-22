using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class G04_CombinedBlock : MonoBehaviour
{
    public List<G04_Block> GetBlocks { get { return _blocks; } }
    public bool GetIsOnGrid { get { return _isOnGrid; } }

    private List<G04_Block> _blocks = new List<G04_Block>();
    private bool _isPickedUp = false;
    private bool _isOnGrid;
    private G04_Grid _storageGrid;
    private SortingGroup _sortingGroup;
    private Vector3 _initialPos;

    private void Awake() {
        _sortingGroup = GetComponent<SortingGroup>();
        _storageGrid = FindObjectOfType<G04_Grid>();

        Debug.Log(_isOnGrid);

        var startBlocks = GetComponentsInChildren<G04_Block>();
        foreach (var block in startBlocks) {
            _blocks.Add(block);
        }
    }

    private void Start() {
        _isOnGrid = IsGroupPosOnGrid(this.transform.position);
        
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
            foreach (G04_Block block in _blocks) {
                Vector3 blockWorldPos = block.transform.position;
                (int, int) closestCellCoor = _storageGrid.GetClosestCellCoor(blockWorldPos);

                _storageGrid.UpdateCellBlockInfo(block, closestCellCoor);
                block.ToggleEligible(true);
                _isOnGrid = true;
            }            
        } else {
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
            }
        }

        _sortingGroup.sortingOrder = 0;
        _isPickedUp = false;
    }
}
