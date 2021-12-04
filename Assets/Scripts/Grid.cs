using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Fire,
    Water
}

[Serializable]
public struct ElementPrefabType
{
    public ElementType Type;
    public GameObject Element;
}

public class Grid : MonoBehaviour
{
    public int XDimension = 4;
    public int YDimension = 6;
    public ElementPrefabType[] ElementPrefabTypeMap;

    //private Dictionary<ElementType, GameObject> elementPrefabTypeMap;

}
