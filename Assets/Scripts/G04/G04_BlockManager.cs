using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G04_BlockManager : MonoBehaviour
{
    private bool _hasClicked = false;
    private float _singleBlockSize = 1f;
    private int _maxSelectedBlocks = 2;
    private List<G04_Block> _selectedBlocks = new();

    void Update() {
        if(Input.GetMouseButtonDown(1) && !_hasClicked) {
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
        if(value) {
            _selectedBlocks.Add(block);
        } else {
            _selectedBlocks.Remove(block);
        }
        Debug.Log("# of currently selected blocks: " + _selectedBlocks.Count);
    }

    public void CombineBlocks() {
        if(_selectedBlocks.Count == _maxSelectedBlocks){
            if(CheckBlocksAdjacent(_selectedBlocks[0], _selectedBlocks[1])) {  

                Vector3 combinedBlockSize = CalculateCombinedBlockSize();
                Vector3 combinedBlockPosition = CalculateCombinedBlockPosition();

                GameObject combinedBlock = new GameObject("CombinedBlock");
                combinedBlock.transform.position = combinedBlockPosition;

                BoxCollider2D collider = combinedBlock.AddComponent<BoxCollider2D>();
                collider.size = combinedBlockSize;

                SpriteRenderer renderer = combinedBlock.AddComponent<SpriteRenderer>();
                renderer.color = Color.white;

                foreach(G04_Block block in _selectedBlocks) {
                    Destroy(block.gameObject);
                }

                _selectedBlocks.Clear();

            } else{
                Debug.Log("Selected blocks are not adjacent.");
            }
        } else{
            Debug.Log(_maxSelectedBlocks + " need to be selected.");
        }
    }

    private bool CheckBlocksAdjacent(G04_Block block1, G04_Block block2) {
        bool isAdjacent = false;

        float deltaX = Math.Abs(block1.transform.position.x - block2.transform.position.x);
        float deltaY = Math.Abs(block1.transform.position.y - block2.transform.position.y);

        Debug.Log("deltaX is " + deltaX);
        Debug.Log("deltaY is " + deltaY);

        if(deltaX == _singleBlockSize && deltaY == 0f) {
            isAdjacent = true;
        } else if(deltaX == 0f && deltaY == _singleBlockSize) {
            isAdjacent = true;
        }

        return isAdjacent;
    }

    private Vector3 GetMouseWorldPosition() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector3 CalculateCombinedBlockSize() {
        Vector3 combinedBlockSize = Vector3.zero;
        foreach (G04_Block block in _selectedBlocks) {
            Vector2 blockColliderSize = block.GetComponent<BoxCollider2D>().size;
            combinedBlockSize += new Vector3(blockColliderSize.x, blockColliderSize.y, 0f);
        }
        return combinedBlockSize;
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
