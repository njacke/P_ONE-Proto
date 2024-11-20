using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class G04_Grid : MonoBehaviour
{
    public float GetGridMinX { get { return _gridMinX; } }
    public float GetGridMaxX { get { return _gridMaxX; } }
    public float GetGridMinY { get { return _gridMinY; } }
    public float GetGridMaxY { get { return _gridMaxY; } }

    [SerializeField] private int _gridSizeX = 4;
    [SerializeField] private int _gridSizeY = 4;
    [SerializeField] private float _cellSize = 1f;

    private Vector2[,] _gridCellsPos;
    private G04_Block[,] _gridCellsBlocks;
    private float _gridMinX;
    private float _gridMaxX;
    private float _gridMinY;
    private float _gridMaxY;

    private void Awake() {
        _gridCellsPos = new Vector2[_gridSizeX, _gridSizeY];
        _gridCellsBlocks = new G04_Block[_gridSizeX, _gridSizeY];

        _gridMinX = 0f - _gridSizeX / 2f;
        _gridMaxX = 0f + _gridSizeX / 2f;
        _gridMinY = 0f - _gridSizeY / 2f;
        _gridMaxY = 0f + _gridSizeY / 2f;

        // get Pos of top left cell -> coordinates (0,0)
        float startPosX = _gridMinX + _cellSize / 2f;
        float startPosY = _gridMaxY - _cellSize / 2f;

        var currentY = startPosY;

        for (int i = 0; i < _gridSizeX; i++) {
            var currentX = startPosX;
            for (int j = 0; j < _gridSizeY; j++) {
                _gridCellsPos[i, j] = new Vector2(currentX, currentY);
                currentX += _cellSize;
            }
            currentY -= _cellSize;
        }
    }

    public bool IsPositionTaken(Vector2 pos) {
        var closestCell = GetClosestCellCoordinates(pos);
        if (_gridCellsBlocks[closestCell.Item1, closestCell.Item2] != null) {
            Debug.Log("position taken");
            return true;
        }
        Debug.Log("position not taken.");
        return false;
    }

    public (int, int) GetClosestCellCoordinates(Vector2 pos) {
        float closestDistance = float.MaxValue;
        (int, int) closestCell = (-1, -1);

        for (int i = 0; i < _gridCellsPos.GetLength(0); i++) {
            for (int j = 0; j < _gridCellsPos.GetLength(1); j++) {
                float distance = Vector2.Distance(_gridCellsPos[i, j], pos);

                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestCell = (i, j);
                }
            }
        }

        return closestCell;
    }

    public Vector2 GetClosestCellPos(Vector2 pos) {
        (int, int) closestCell = GetClosestCellCoordinates(pos);
        Vector2 closestPos = _gridCellsPos[closestCell.Item1, closestCell.Item2];

        return closestPos;
    }

    public void UpdateCellBlockInfo(G04_Block block, (int, int) cellCoordinates) {
        _gridCellsBlocks[cellCoordinates.Item1, cellCoordinates.Item2] = block;
    }

    public bool IsOnGrid(Vector3 pos){
        if (pos.x >= _gridMinX && pos.x <= _gridMaxX &&
            pos.y >= _gridMinY && pos.y <= _gridMaxY) {
                return true;
            }
        return false;
    }

    public (int, int) GetBlockCellCoordinates(G04_Block block) {
        (int, int) cellCoordinates = (-1, -1);

        for (int i = 0; i < _gridCellsPos.GetLength(0); i++) {
            for (int j = 0; j < _gridCellsPos.GetLength(1); j++) {
                if (_gridCellsBlocks[i, j] == block) {
                    cellCoordinates = (i, j);
                }
            }
        }

        return cellCoordinates;
    }
}
