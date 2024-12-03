using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class G05_Object : MonoBehaviour
{
    [SerializeField] private ObjectType _objectType;

    public enum ObjectType {
        None,
        Chest,
        Lever
    }

}
