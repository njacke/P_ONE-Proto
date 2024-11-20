using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G04_BlockManager : MonoBehaviour
{
    [SerializeField] GameObject _blockPrefab;
    private bool _hasClicked = false;
    private float _singleBlockSize = 1f;
    private int _maxSelectedBlocks = 2;
    private List<G04_Block> _selectedBlocks = new();
    private G04_Grid _storageGrid;

    void Start() {
        _storageGrid = FindObjectOfType<G04_Grid>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(1) && !_hasClicked) {
            RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
            
            if(hit.collider != null){
                G04_Block block = hit.collider.GetComponent<G04_Block>();

                if(block != null){
                    block.ToggleSelection(_selectedBlocks.Count == _maxSelectedBlocks);
                    _hasClicked = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(1)) {
            _hasClicked = false;
        }

        if (Input.GetMouseButtonDown(2)) {
            CombineBlocks();
        }        
    }

    public void UpdateBlockSelection(G04_Block block, bool value){
        if (value) {
            _selectedBlocks.Add(block);
        } else {
            _selectedBlocks.Remove(block);
        }
        Debug.Log("# of currently selected blocks: " + _selectedBlocks.Count);
    }

    public void CombineBlocks() {
        if(_selectedBlocks.Count == _maxSelectedBlocks){
            if(CheckBlocksAdjacent(_selectedBlocks[0], _selectedBlocks[1])) {  

                (float, float) blockSize = CalculateCombinedBlockSize();
                Vector3 blockPos = CalculateCombinedBlockPosition();

                var newBlock = Instantiate(_blockPrefab, blockPos, Quaternion.identity);
                newBlock.transform.localScale = new Vector3 (blockSize.Item1, blockSize.Item2, 1f);
                
                foreach(G04_Block block in _selectedBlocks) {
                    Destroy(block.gameObject);
                }

                _selectedBlocks.Clear();

            } else {
                Debug.Log("Selected blocks are not adjacent.");
            }
        } else {
            Debug.Log(_maxSelectedBlocks + " need to be selected.");
        }
    }

    private bool CheckBlocksAdjacent(G04_Block block1, G04_Block block2) {
        (int, int) block1Coor =  _storageGrid.GetBlockCellCoordinates(block1);
        (int, int) block2Coor =  _storageGrid.GetBlockCellCoordinates(block2);

        int coorDiff = block1Coor.Item1 - block2Coor.Item1 + (block1Coor.Item2 - block2Coor.Item2);

        if (Mathf.Abs(coorDiff) > 1) {
            return false;
        }

        return true;
    }

    private Vector3 GetMouseWorldPosition() {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private (float, float) CalculateCombinedBlockSize() {
        float cellCount = 0;
        (float, float) combinedSize = (0, 0);

        foreach (G04_Block block in _selectedBlocks) {
            float cells = block.GetBlockSizeX * block.GetBlockSizeY;
            cellCount += cells;
        }        

        if (cellCount == 2) {
            combinedSize.Item1 = 2;
            combinedSize.Item2 = 1;
        } else {
            combinedSize.Item1 = cellCount / 2;
            combinedSize.Item2 = cellCount / 2;
        }

        return combinedSize;
    }

    private Vector3 CalculateCombinedBlockPosition() {
        Vector3 combinedBlockPosition = Vector3.zero;

        foreach (G04_Block block in _selectedBlocks) {
            combinedBlockPosition += block.transform.position;
        }

        combinedBlockPosition /= _selectedBlocks.Count;
        return combinedBlockPosition;
    }
}
