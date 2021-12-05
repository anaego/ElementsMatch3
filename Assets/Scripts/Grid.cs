using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    None,
    Fire,
    Water
}

public class Grid : MonoBehaviour
{
    public int Columns = 5;
    public int Rows = 6;
    public Element FireElementPrefab;
    public Element WaterElementPrefab;

    private float xOffset = 0.5f;
    private Dictionary<ElementType, Element> elementTypeMap;
    private Element[,] allElements;
    private ElementType[,] typeMap;

    public void Start()
    {
        allElements = new Element[Columns, Rows];
        elementTypeMap =  new Dictionary<ElementType, Element> {
            { ElementType.Fire, FireElementPrefab },
            { ElementType.Water, WaterElementPrefab }
        };
        // TODO load from a file
        typeMap = new ElementType[5, 2] { 
            { ElementType.Water, ElementType.Water }, 
            { ElementType.None, ElementType.None }, 
            { ElementType.Water, ElementType.Fire }, 
            { ElementType.Fire, ElementType.None }, 
            { ElementType.Fire, ElementType.None }
        };
        Setup();
    }

    private void Setup()
    {
        for (int i = 0; i <= Columns; i++)
        {
            if (i > typeMap.GetLength(0) - 1) break;
            for (int j = 0; j <= Rows; j++)
            {
                if (j > typeMap.GetLength(1) - 1) break;
                var elementType = typeMap[i, j];
                if (elementType != ElementType.None)
                {
                    Instantiate(
                        elementTypeMap[elementType], 
                        new Vector2(i - (float)Columns / 2 + xOffset, j - (float)Rows / 2), 
                        Quaternion.identity);
                }
            }
        }
    }
}
