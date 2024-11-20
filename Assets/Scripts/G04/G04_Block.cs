using UnityEditor;
using UnityEngine;

public class G04_Block : MonoBehaviour
{
    public float GetBlockSizeX { get { return _blockSizeX; } }
    public float GetBlockSizeY { get { return _blockSizeY; } }
    private float _blockSizeX = 1f;
    private float _blockSizeY = 1f;
    private (int, int)[] _gridCells;

    private bool _isSelected = false;
    private bool _isDragging = false;
    private bool _isPlaced = false;
    //private Vector3 offset;
    
    private SpriteRenderer _spriteRenderer;
    private G04_BlockManager _blockManager;
    private G04_Grid _storageGrid;
    private Vector3 _initialPos;

    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _blockManager = FindObjectOfType<G04_BlockManager>();
        _storageGrid = FindObjectOfType<G04_Grid>();
    }

    private void OnMouseDown(){
        //Debug.Log("Entered OnMouseDown");
        if (!_isDragging){
            _isSelected = false;
            _blockManager.UpdateBlockSelection(this, _isSelected);
            _isDragging = true;
            //Debug.Log("isDragging set to true");
            _initialPos = transform.position;

            _storageGrid.UpdateCellBlockInfo(null, _storageGrid.GetClosestCellCoordinates(_initialPos));
        }
    }

    private void OnMouseDrag(){
        if (_isDragging){
            var currentMouseWorldPos = GetMouseWorldPosition();

            if (_storageGrid.IsOnGrid(currentMouseWorldPos)) {                    
                var closestCellPos = _storageGrid.GetClosestCellPos(currentMouseWorldPos);
                transform.position = new Vector3(closestCellPos.x, closestCellPos.y, -1f); 
            } else {
                transform.position = new Vector3(currentMouseWorldPos.x, currentMouseWorldPos.y, -1f);
            }

            if (_storageGrid.IsOnGrid(transform.position) && _storageGrid.IsPositionTaken(transform.position)) {
                _spriteRenderer.color = Color.red;            
            } else {
                _spriteRenderer.color = Color.white;
            }
        }
    }

    private void OnMouseUp() {
        _isDragging = false;
        if (!_storageGrid.IsOnGrid(transform.position) || _storageGrid.IsPositionTaken(transform.position)) {
            transform.position = _initialPos;
        } else {
            transform.position = _storageGrid.GetClosestCellPos(transform.position);
            _isPlaced = true;
        }

        if (_isPlaced) {
            _storageGrid.UpdateCellBlockInfo(this, _storageGrid.GetClosestCellCoordinates(transform.position));
        }

        _spriteRenderer.color = Color.white;
        
        Debug.Log("isDragging set to false");
    }

    private Vector3 GetMouseWorldPosition() {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public void ToggleSelection(bool isMaxCount) {
        if(!_isSelected && _storageGrid.IsOnGrid(transform.position) && !isMaxCount){
            _isSelected = true;
            _blockManager.UpdateBlockSelection(this, _isSelected);
            _spriteRenderer.color = Color.green;
        } else {
            _isSelected = false;
            _blockManager.UpdateBlockSelection(this, _isSelected);
            _spriteRenderer.color = Color.white;
        }
    }

    public void SetScale(float scaleX, float scaleY) {
        Vector3 newScale = new (scaleX, scaleY, 1);
        this.transform.localScale = newScale;
    }
}
