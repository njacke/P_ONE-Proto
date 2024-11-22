using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class G04_BlockManager : MonoBehaviour
{
    [SerializeField] GameObject _combinedBlockPrefab;
    private int _maxSelectedCombinedBlocks = 2;
    private List<G04_CombinedBlock> _selectedCombinedBlocks = new();
    private G04_Grid _storageGrid;
    private G04_CombinedBlock _pickedBlock;
    void Start() {
        _storageGrid = FindObjectOfType<G04_Grid>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            PickUpBlock();
        }

        if (Input.GetMouseButtonUp(0)) {
            PutDownBlock();
        }

        if (Input.GetMouseButtonDown(1)) {
            if (_pickedBlock == null) {
                SelectBlock();
            } else {
                _pickedBlock.transform.Rotate(0f, 0f, 90f);
            }
        }

        if (Input.GetMouseButtonDown(2)) {
            CombineBlocks();
        }        
    }

    private void PickUpBlock() {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);

        if (hit.collider != null) {
            Debug.Log("Hit: " + hit.collider.name);
            _pickedBlock = hit.collider.GetComponentInParent<G04_CombinedBlock>();
            if (_pickedBlock != null) {
                foreach (var combinedBlock in _selectedCombinedBlocks) {
                    foreach (var block in combinedBlock.GetBlocks) {
                        block.ToggleSelected(false);
                    }
                }

                _selectedCombinedBlocks.Clear();
                _pickedBlock.OnPickUp();
                
                Debug.Log("Block picked up.");
            }

        } else {
            Debug.Log("Nothing hit!");
        }
    }
    
    private void PutDownBlock() {
        if (_pickedBlock != null) {
            _pickedBlock.OnDropDown();
            _pickedBlock = null;
        }
    }

    private void SelectBlock() {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);

        if (hit.collider != null) {
            G04_CombinedBlock combinedBlock = hit.collider.GetComponentInParent<G04_CombinedBlock>();

            if (combinedBlock != null && combinedBlock.GetIsOnGrid) {
                if (_selectedCombinedBlocks.Contains(combinedBlock)) { // deselect if selected
                    foreach (var block in combinedBlock.GetBlocks) {
                        block.ToggleSelected(false);
                    }

                    _selectedCombinedBlocks.Remove(combinedBlock);
                }
                else if (_selectedCombinedBlocks.Count < _maxSelectedCombinedBlocks) { // select if not selected & not max selected
                    foreach (var block in combinedBlock.GetBlocks) {
                        block.ToggleSelected(true);
                    }

                    _selectedCombinedBlocks.Add(combinedBlock);

                }
                else { // return if max blocks selected
                    return;
                }

                Debug.Log("Selected blocks count: " + _selectedCombinedBlocks.Count);
            }
        }
    }

    public void CombineBlocks() {
        if(_selectedCombinedBlocks.Count == _maxSelectedCombinedBlocks) {
            if(_storageGrid.CheckCombinedBlocksAdjacent(_selectedCombinedBlocks[0], _selectedCombinedBlocks[1])) {
                var blocks = new List<G04_Block>();
                foreach (var combinedBlock in _selectedCombinedBlocks) {
                    blocks.AddRange(combinedBlock.GetBlocks);
                }

                Vector3[] blocksPos = blocks.Select(x => x.transform.position).ToArray();

                Vector3 newPos = GetCombinedBlockPosition(blocksPos);
                var newCombinedBlock = Instantiate(_combinedBlockPrefab, newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>();

                for (int i = 0; i < _selectedCombinedBlocks.Count; i++) {
                    var combinedBlock = _selectedCombinedBlocks[i];

                    for (int j = 0; j < combinedBlock.GetBlocks.Count; j++) {
                        newCombinedBlock.AddBlock(combinedBlock.GetBlocks[j]);
                        _storageGrid.UpdateCellBlockInfo(combinedBlock.GetBlocks[j], _storageGrid.GetBlockCoor(combinedBlock.GetBlocks[j]));
                    }

                    Destroy(combinedBlock.gameObject);
               
                }

                foreach (var block in newCombinedBlock.GetBlocks) {
                    block.ToggleSelected(false);
                }

                _selectedCombinedBlocks.Clear();

            } else {
                Debug.Log("Selected blocks are not adjacent.");
            }
        } else {
            Debug.Log(_maxSelectedCombinedBlocks + " need to be selected.");
        }
    }

    private Vector3 GetMouseWorldPosition() {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public static Vector3 GetCombinedBlockPosition(Vector3[] blocksPos) {
        if (blocksPos.Length == 0) return Vector3.zero;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var blockPos in blocksPos) {

            minX = Mathf.Min(minX, blockPos.x);
            maxX = Mathf.Max(maxX, blockPos.x);
            minY = Mathf.Min(minY, blockPos.y);
            maxY = Mathf.Max(maxY, blockPos.y);
        }

        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        //Debug.Log("X: " + centerX + "Y " + centerY);

        return new Vector3(centerX, centerY, 0f);
    }

}
