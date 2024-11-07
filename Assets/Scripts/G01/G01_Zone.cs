using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G01_Zone : MonoBehaviour
{
    [SerializeField] private ZoneType _zoneType = ZoneType.None;
    [SerializeField] private G01_Entity.ShapeType _shapeType = G01_Entity.ShapeType.None;
    [SerializeField] private G01_Entity.ColorType _colorType = G01_Entity.ColorType.None;

    public ZoneType GetZoneType { get { return _zoneType; } }
    public G01_Entity.ColorType GetColorType { get { return _colorType; } }
    public G01_Entity.ShapeType GetShapeType { get { return _shapeType; } }

    public enum ZoneType {
        None,
        Color,
        Shape
    }
}
