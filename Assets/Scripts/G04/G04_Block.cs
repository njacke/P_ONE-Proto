using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class G04_Block : MonoBehaviour
{
    private bool _isSelected = false;
    private bool _isDragging = false;
    //private Vector3 offset;
    
    private SpriteRenderer _spriteRenderer;
    private G04_BlockManager _blockManager;
    private Vector3 _initialPos;

    private float _xMin;
    private float _xMax;
    private float _yMin;
    private float _yMax;
    private float _padding = 1f;
    private float _gridCellSize = 1f;
    private float _gridXMin = -2f;
    private float _gridXMax = 2f;
    private float _gridYMin = -2f;
    private float _gridYMax = 2f;
    private float _gridSnapOffset = 0.5f;

    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _blockManager = FindObjectOfType<G04_BlockManager>();

        SetUpMoveBoundries();
    }

    private void OnMouseDown(){
        Debug.Log("Entered OnMouseDown");
        if (!_isDragging){
            _isSelected = false;
            _blockManager.UpdateBlockSelection(this, _isSelected);
            _isDragging = true;
            Debug.Log("isDragging set to true");
            _initialPos = transform.position;
            //var mouseWorldPos = GetMouseWorldPosition();
            //offset = transform.position - new Vector3 (mouseWorldPos.x, mouseWorldPos.y, 0f);
        }
    }

    private void OnMouseDrag(){
        if(_isDragging){
            var currentMouseWorldPos = GetMouseWorldPosition();
            var newPosX = Mathf.Clamp(currentMouseWorldPos.x, _xMin, _xMax);
            var newPosY = Mathf.Clamp(currentMouseWorldPos.y, _yMin, _yMax);

            if(IsOnGrid()){                    
                transform.position = SnapPositionToGrid(new Vector3 (newPosX, newPosY, -1f));
            }
            else{
                transform.position = new Vector3(newPosX, newPosY, -1f);
            }

            if(IsPositionTaken(transform.position)){
                _spriteRenderer.color = Color.red;            
            }
            else{
                _spriteRenderer.color = Color.white;
            }
            
            //transform.position = GetMouseWorldPosition() + offset;

        }
    }

    private void OnMouseUp(){
        _isDragging = false;
        if(!IsOnGrid() || IsPositionTaken(transform.position)){
            transform.position = _initialPos;
        }
        else{
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }

        _spriteRenderer.color = Color.white;
        
        Debug.Log("isDragging set to false");
    }

    private Vector3 GetMouseWorldPosition(){
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private bool IsOnGrid(){
        if(transform.position.x >= _gridXMin && transform.position.x <= _gridXMax &&
            transform.position.y >= _gridYMin && transform.position.y <= _gridYMax){
                return true;
            }
        return false;
    }

    private Vector3 SnapPositionToGrid(Vector3 position){
        float snappedX = Mathf.Floor(position.x / _gridCellSize) * _gridCellSize;
        float snappedY = Mathf.Floor(position.y / _gridCellSize) * _gridCellSize;
        return new Vector3(snappedX + _gridSnapOffset, snappedY + _gridSnapOffset, position.z);
    }

    private bool IsPositionTaken(Vector3 position){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders){
            if (collider.gameObject != gameObject){
                return true;
            }
        }
        return false;
    }

    // change so padding is set based on block type
    private void SetUpMoveBoundries(){
        Camera gameCamera = Camera.main;
        _xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + _padding;
        _xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - _padding;
        _yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + _padding;
        _yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - _padding;
    }

    public void ToggleSelection(bool isMaxCount){
        if(!_isSelected && IsOnGrid() && !isMaxCount){
            _isSelected = true;
            _blockManager.UpdateBlockSelection(this, _isSelected);
            _spriteRenderer.color = Color.green;
        }
        else{
            _isSelected = false;
            _blockManager.UpdateBlockSelection(this, _isSelected);
            _spriteRenderer.color = Color.white;
        }
    }
}
