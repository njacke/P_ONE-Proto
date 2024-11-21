using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class G04_CombinedBlock : MonoBehaviour
{
    public bool PickedUp { get; private set; }
    public Vector3 InitialPos { get; private set; }
    public List<G04_Block> GetBlocks { get { return _blocks; } }
    private List<G04_Block> _blocks = new List<G04_Block>();
    private G04_Grid _storageGrid;
    private SortingGroup _sortingGroup;

    private void Awake() {
        InitialPos = transform.position;
        _sortingGroup = GetComponent<SortingGroup>();

        var startBlocks = GetComponentsInChildren<G04_Block>();
        foreach (var block in startBlocks) {
            _blocks.Add(block);
        }

        _storageGrid = FindObjectOfType<G04_Grid>();
    }

    private void Update() {
        if (PickedUp) {
            UpdatePos();
        }
    }


    private void UpdatePos() {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 offset = GetCombinedBlockOffset(_blocks);
        Vector3 newPos = mouseWorldPos - offset;
        newPos.z = transform.position.z;

        Debug.Log($"Parent position before: {transform.position}, new position: {newPos}");

        if (IsGroupPosOnGrid(newPos)) {
            // calc snap pos for each block
            Vector3[] snappedPositions = new Vector3[_blocks.Count];
            for (int i = 0; i < _blocks.Count; i++) {
                snappedPositions[i] = _storageGrid.GetClosestCellPos(_blocks[i].transform.position + newPos - transform.position);
            }

            // calc average snap pos to the center
            Vector3 snappedCenter = Vector3.zero;
            foreach (var snappedPos in snappedPositions) {
                snappedCenter += snappedPos;
            }

            snappedCenter /= snappedPositions.Length;

            transform.position = snappedCenter;
            
            //Debug.Log("Mouse Position: " + mouseWorldPos);
            //Debug.Log("Offset: " + offset);
            //Debug.Log("Calculated Position: " + newPos);
            Debug.Log("Snapped Center: " + snappedCenter);

            if (IsGroupPosTaken(snappedPositions)) {
                foreach (var block in _blocks) {
                    block.ToggleEligible(false);
                }
            } else {
                foreach (var block in _blocks) {
                    block.ToggleEligible(true);
                }
            }
        } else {
            transform.position = newPos;

            foreach (var block in _blocks) {
                block.ToggleEligible(true);
            }
        }
    }

    public void AddBlock(G04_Block block) {
        _blocks.Add(block);
        block.CombinedBlock = this;
        block.transform.parent = this.transform;
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

    private bool IsGroupPosOnGrid(Vector3 pos) {
        foreach (G04_Block block in _blocks) {
            Vector3 blockPos = block.transform.position + pos - transform.position;
            if (!_storageGrid.IsOnGrid(blockPos)) {
                return false;
            }            
        }
        return true;
    }

    private bool IsGroupPosTaken(Vector3[] snappedPositions) {
        foreach (var snappedPos in snappedPositions) {
            if (_storageGrid.IsPosTaken(snappedPos)) {
                return true;
            }            
        }
        return false;
    }

    public static Vector3 GetCombinedBlockOffset(List<G04_Block> blocks) {
        if (blocks.Count == 0) {
            return Vector3.zero;
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var block in blocks) {
            Vector3 localPos = block.transform.localPosition;

            minX = Mathf.Min(minX, localPos.x);
            maxX = Mathf.Max(maxX, localPos.x);
            minY = Mathf.Min(minY, localPos.y);
            maxY = Mathf.Max(maxY, localPos.y);
        }

        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        Debug.Log("minX " + minX);
        Debug.Log("maxX " + maxX);
        Debug.Log("minY " + minY);
        Debug.Log("maxY " + maxY);

        return new Vector3(centerX, centerY, 0f);
    }

    public void OnPickUp() {
        InitialPos = this.transform.position;

        // update cells to empty(none)
        foreach (G04_Block block in _blocks) {
            (int, int) blockCoor = _storageGrid.GetBlockCoor(block); // returns (-1, -1) if no match
            //Debug.Log("Block coor: " + blockCoor);

            if (blockCoor.Item1 > -1) {
                //Debug.Log("Updating block coor info");
                _storageGrid.UpdateCellBlockInfo(null, blockCoor);
            }
        }

        _sortingGroup.sortingOrder = 1;
        PickedUp = true;
    }

    public void OnDropDown() {
        if (IsGroupPosEligible()) {
            foreach (G04_Block block in _blocks) {
                Vector3 blockWorldPos = block.transform.position;
                Vector3 closestCellPos = _storageGrid.GetClosestCellPos(blockWorldPos);
                (int, int) closestCellCoor = _storageGrid.GetClosestCellCoor(blockWorldPos);

                block.transform.position = closestCellPos;
                _storageGrid.UpdateCellBlockInfo(block, closestCellCoor);
                block.ToggleEligible(true);
            }            
        } else {
            this.transform.position = InitialPos;
            var isOnGrid = IsGroupPosOnGrid(this.transform.position);
            
            foreach (G04_Block block in _blocks) {
                block.ToggleEligible(true);

                if (isOnGrid) {
                    Vector3 blockWorldPos = block.transform.position;
                    (int, int) closestCellCoor = _storageGrid.GetClosestCellCoor(blockWorldPos);
                    _storageGrid.UpdateCellBlockInfo(block, closestCellCoor);
                }
            }
        }

        _sortingGroup.sortingOrder = 0;
        PickedUp = false;
    }
}
