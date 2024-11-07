using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_Entity : MonoBehaviour
{
    [SerializeField] private Sprite[] _triangleSprites;
    [SerializeField] private Sprite[] _circleSprites;
    [SerializeField] private Sprite[] _squareSprites;
    [SerializeField] private Sprite[] _diamondSprites;

    public ShapeType GetShapeType { get { return _shapeType; } }
    public ColorType GetColorType { get { return _colorType; } }
    
    protected ShapeType _shapeType = ShapeType.None;
    protected ColorType _colorType = ColorType.None;

    private SpriteRenderer _spriteRenderer;
    private Dictionary<ShapeType, Sprite[]> _shapeSpriteMap;

    public enum ShapeType {
        None,
        Triangle,
        Circle,
        Square,
        Diamond
    }

    public enum ColorType {
        None,
        Red,
        Blue,
        Green,
        Yellow
    }

    protected virtual void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _shapeSpriteMap = new Dictionary<ShapeType, Sprite[]> {
            { ShapeType.Triangle, _triangleSprites },
            { ShapeType.Circle, _circleSprites },
            { ShapeType.Square, _squareSprites },
            { ShapeType.Diamond, _diamondSprites }
        };
    }

    protected void UpdateEntity(ShapeType shapeType, ColorType colorType) {
        if (_shapeType == ShapeType.None) {
            _shapeType = shapeType;
            _spriteRenderer.sprite = _shapeSpriteMap[_shapeType][0];
        }

        if (_colorType == ColorType.None && _shapeType != ShapeType.None) {
            Debug.Log("update entity color change called.");
            _colorType = colorType;
            int colorIndex = (int)_colorType; // sprite array order (index) needs to be the same as enum index!
            _spriteRenderer.sprite = _shapeSpriteMap[_shapeType][colorIndex];
        }
    }
}
