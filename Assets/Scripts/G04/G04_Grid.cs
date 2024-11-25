using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class G04_Grid : MonoBehaviour
{
    public float GetGridMinX { get { return _gridMinX; } }
    public float GetGridMaxX { get { return _gridMaxX; } }
    public float GetGridMinY { get { return _gridMinY; } }
    public float GetGridMaxY { get { return _gridMaxY; } }
    public float GetCellSize { get { return _cellSize; } }

    [SerializeField] private int _gridSizeX = 4;
    [SerializeField] private int _gridSizeY = 4;
    [SerializeField] private float _cellSize = 1f;

    private Vector2[,] _gridWorldPos;
    private G04_Block[,] _gridBlocks;
    private G04_CombinedBlock[,] _gridCombinedBlocks;
    private float _gridMinX;
    private float _gridMaxX;
    private float _gridMinY;
    private float _gridMaxY;

    private void Awake() {
        _gridWorldPos = new Vector2[_gridSizeX, _gridSizeY];
        _gridBlocks = new G04_Block[_gridSizeX, _gridSizeY];
        _gridCombinedBlocks = new G04_CombinedBlock[_gridSizeX, _gridSizeY];

        _gridMinX = 0f - _gridSizeX / 2f;
        _gridMaxX = 0f + _gridSizeX / 2f;
        _gridMinY = 0f - _gridSizeY / 2f;
        _gridMaxY = 0f + _gridSizeY / 2f;

        // get Pos of bot left cell -> coordinates (0,0)
        float startPosX = _gridMinX + _cellSize / 2f;
        float startPosY = _gridMinY + _cellSize / 2f;

        var currentY = startPosY;

        for (int i = 0; i < _gridSizeX; i++) {
            var currentX = startPosX;
            for (int j = 0; j < _gridSizeY; j++) {
                _gridWorldPos[i, j] = new Vector2(currentX, currentY);
                currentX += _cellSize;
            }
            currentY += _cellSize;
        }
    }

    public bool IsPosTaken(Vector3 pos) {
        var closestCell = GetClosestCellCoor(pos);
        if (_gridBlocks[closestCell.Item1, closestCell.Item2] != null) {
            //Debug.Log("position taken");
            return true;
        }
        //Debug.Log("position not taken.");
        return false;
    }

    public (int, int) GetClosestCellCoor(Vector3 pos) {
        float closestDistance = float.MaxValue;
        (int, int) closestCell = (-1, -1);

        for (int i = 0; i < _gridWorldPos.GetLength(0); i++) {
            for (int j = 0; j < _gridWorldPos.GetLength(1); j++) {
                float distance = Vector2.Distance(_gridWorldPos[i, j], pos);

                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestCell = (i, j);
                }
            }
        }

        return closestCell;
    }

    public Vector3 GetClosestCellPos(Vector3 pos) {
        (int, int) closestCell = GetClosestCellCoor(pos);
        Vector3 closestPos = _gridWorldPos[closestCell.Item1, closestCell.Item2];

        return closestPos;
    }

    public (int, int) GetBlockCoor(G04_Block block) {
        (int, int) cellCoordinates = (-1, -1);

        for (int i = 0; i < _gridWorldPos.GetLength(0); i++) {
            for (int j = 0; j < _gridWorldPos.GetLength(1); j++) {
                if (_gridBlocks[i, j] == block) {
                    cellCoordinates = (i, j);
                }
            }
        }

        return cellCoordinates;
    }

    public void UpdateCellBlockInfo(G04_Block block, (int, int) cellCoordinates) {
        _gridBlocks[cellCoordinates.Item1, cellCoordinates.Item2] = block;

        if (block != null) {
            _gridCombinedBlocks[cellCoordinates.Item1, cellCoordinates.Item2] = block.CombinedBlock;
        } else {
            _gridCombinedBlocks[cellCoordinates.Item1, cellCoordinates.Item2] = null;
        }
    }

    public bool IsOnGrid(Vector3 pos){
        if (pos.x >= _gridMinX && pos.x <= _gridMaxX &&
            pos.y >= _gridMinY && pos.y <= _gridMaxY) {
                return true;
            }

        return false;
    }

    public bool CheckCombinedBlocksAdjacent (G04_CombinedBlock combinedBlock1, G04_CombinedBlock combinedBlock2) {
        List<(int, int)> block1coor = GetCombinedBlockCoor(combinedBlock1);
        List<(int, int)> block2coor = GetCombinedBlockCoor(combinedBlock2);

        //Debug.Log("Block 1 coords count: " + block1coor.Count);
        //Debug.Log("Block 2 coords count: " + block2coor.Count);

        //Debug.Log("Coordinates block 1 " + block1coor[0].Item1.ToString() + block1coor[0].Item2.ToString());
        //Debug.Log("Coordinates block 2 " + block2coor[0].Item1.ToString() + block2coor[0].Item2.ToString());

        foreach(var coor1 in block1coor) {
            foreach (var coor2 in block2coor) {
                if ((Mathf.Abs(coor1.Item1 - coor2.Item1) == 1 && coor1.Item2 == coor2.Item2) ||
                    (Mathf.Abs(coor1.Item2 - coor2.Item2) == 1 && coor1.Item1 == coor2.Item1)) {
                    return true;
                }
            } 
        }

        return false;
    }

    public List<(int, int)> GetCombinedBlockCoor(G04_CombinedBlock combinedBlock) {
        var coordsList = new List<(int, int)>();

        for (int i = 0; i < _gridCombinedBlocks.GetLength(0); i++) {
            for (int j = 0; j < _gridCombinedBlocks.GetLength(1); j++) {
                if (_gridCombinedBlocks[i, j] == combinedBlock) {
                    coordsList.Add((i, j));
                }
            }
        }

        return coordsList;
    }

    public G04_CombinedBlock[] GetAdjacentCombinedBlocks(G04_CombinedBlock combinedBlock) {
        var adjacentCombinedBlocks = new List<G04_CombinedBlock>();
        
        foreach (var block in combinedBlock.GetBlocks) {
            var adjBlocks = GetAdjacentBlocks(block);
            foreach (var adjBlock in adjBlocks) {
                if (adjBlock == null || adjBlock.CombinedBlock == combinedBlock || adjacentCombinedBlocks.Contains(adjBlock.CombinedBlock)) {
                    continue;
                } else {
                    adjacentCombinedBlocks.Add(adjBlock.CombinedBlock);
                }
            }
        }

        return adjacentCombinedBlocks.ToArray();
    }

    public G04_Block[] GetAdjacentBlocks(G04_Block block) {
        G04_Block[] blocks = new G04_Block[4];

        if (block == null) {
            Debug.Log("Block doesn't exist");
            return blocks;
        }

        (int, int)[] adjacentBlocksCoor= new (int, int)[4];

        (int, int) blockCoor = GetBlockCoor(block);
        adjacentBlocksCoor[0] = (blockCoor.Item1 + 1, blockCoor.Item2);
        adjacentBlocksCoor[1] = (blockCoor.Item1 - 1, blockCoor.Item2);
        adjacentBlocksCoor[2] = (blockCoor.Item1, blockCoor.Item2 + 1);
        adjacentBlocksCoor[3] = (blockCoor.Item1, blockCoor.Item2 - 1);

        for (int i = 0; i < blocks.Length; i++) {
            blocks[i] = GetBlock(adjacentBlocksCoor[i]);
        }

        return blocks;
    }

    public G04_Block GetBlock((int, int) blockCoor) {
        // if index out of range
        if (blockCoor.Item1 < 0 || blockCoor.Item1 >= _gridBlocks.GetLength(0) || blockCoor.Item2 < 0 || blockCoor.Item2 >= _gridBlocks.GetLength(1)) {
            return null;
        }

        return _gridBlocks[blockCoor.Item1, blockCoor.Item2];
    }

    public G04_CombinedBlock[] GetAllCombinedBlocksOnGrid() {
        var combinedBlocks = new List<G04_CombinedBlock>();

        foreach (var combinedBlock in _gridCombinedBlocks) {
            if (combinedBlock == null || combinedBlocks.Contains(combinedBlock)) {
                continue;
            } else {
                combinedBlocks.Add(combinedBlock);
            }
        }

        return combinedBlocks.ToArray();
    }
}
