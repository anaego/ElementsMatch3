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
    public Element FireElementPrefab;
    public Element WaterElementPrefab;

    private Element[,] allElements;
    private float xOffset = 0.5f;
    private Dictionary<ElementType, Element> elementTypeMap;
    private LevelMap levelMap = new LevelMap();

    public static bool IsInputEnabled = true;

    public void Start()
    {
        // TODO load from a file
        levelMap.TypeMap = new ElementType[5, 2] {
            { ElementType.Water, ElementType.Water },
            { ElementType.None, ElementType.None },
            { ElementType.Water, ElementType.Fire },
            { ElementType.Fire, ElementType.None },
            { ElementType.Fire, ElementType.None }
        };
        allElements = new Element[levelMap.Columns, levelMap.Rows];
        elementTypeMap = new Dictionary<ElementType, Element> {
            { ElementType.Fire, FireElementPrefab },
            { ElementType.Water, WaterElementPrefab }
        };
        Setup();
    }

    private void Setup()
    {
        for (int i = 0; i <= levelMap.Columns; i++)
        {
            if (i > levelMap.TypeMap.GetLength(0) - 1) break;
            for (int j = 0; j <= levelMap.Rows; j++)
            {
                if (j > levelMap.TypeMap.GetLength(1) - 1) break;
                var elementType = levelMap.TypeMap[i, j];
                if (elementType != ElementType.None)
                {
                    var newElement = Instantiate(
                        elementTypeMap[elementType],
                        new Vector2(
                            i - (float)levelMap.Columns / 2 + xOffset,
                            j - (float)levelMap.Rows / 2),
                        Quaternion.identity);
                    newElement.Setup(this, i, j);
                    allElements[i, j] = newElement;
                }
            }
        }
    }

    internal void SwipeElement(Element element, float swipeAngle)
    {
        // TODO change z value of elems so that correct ones look on top
        Element otherElement = null;
        Vector2 elementPosition = element.transform.position;
        Vector2 otherElementPosition = element.transform.position;
        // Move right
        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            // if it's the rightmost element, can't swipe it
            if (element.Column == allElements.GetLength(0) - 1)
            {
                IsInputEnabled = true;
                return;
            }
            otherElement = allElements[element.Column + 1, element.Row];
            if (otherElement != null)
            {
                otherElement.Column -= 1;
            }
            else
            {
                otherElementPosition = new Vector2(elementPosition.x + 1, elementPosition.y);
            }
            element.Column += 1;
            allElements[element.Column, element.Row] = element;
            allElements[element.Column - 1, element.Row] = otherElement;
        }
        // Move up
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {
            // if it's the highest element, can't swipe it
            if (element.Row + 1 > allElements.GetLength(1) - 1)
            {
                IsInputEnabled = true;
                return;
            }
            otherElement = allElements[element.Column, element.Row + 1];
            // Can't move element up if there is no element on top of it
            if (otherElement == null)
            {
                IsInputEnabled = true;
                return;
            }
            otherElement.Row -= 1;
            element.Row += 1;
            allElements[element.Column, element.Row] = element;
            allElements[element.Column, element.Row - 1] = otherElement;
        }
        // Move left
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {
            // if it's the leftmost element, can't swipe it
            if (element.Column == 0)
            {
                IsInputEnabled = true;
                return;
            }
            otherElement = allElements[element.Column - 1, element.Row];
            if (otherElement != null)
            {
                otherElement.Column += 1;
            }
            else
            {
                otherElementPosition = new Vector2(elementPosition.x - 1, elementPosition.y);
            }
            element.Column -= 1;
            allElements[element.Column, element.Row] = element;
            allElements[element.Column + 1, element.Row] = otherElement;
        }
        // Move down
        else if (swipeAngle < -45 && swipeAngle >= -135)
        {
            // if it's the lowest element, can't swipe it
            if (element.Row == 0)
            {
                IsInputEnabled = true;
                return;
            }
            otherElement = allElements[element.Column, element.Row - 1];
            if (otherElement != null)
            {
                otherElement.Row += 1;
            }
            else
            {
                otherElementPosition = new Vector2(elementPosition.x, elementPosition.y - 1);
            }
            element.Row -= 1;
            allElements[element.Column, element.Row] = element;
            allElements[element.Column, element.Row] = otherElement;
        }
        // Actually visibly move elements
        if (otherElement != null)
        {
            otherElementPosition = otherElement.transform.position;
            StartCoroutine(MoveToPosition(otherElement.gameObject, elementPosition));
        }
        StartCoroutine(MoveToPosition(element.gameObject, otherElementPosition, () =>
        {
            Normalize();
            IsInputEnabled = true;
        }));
    }

    private IEnumerator MoveToPosition(GameObject objectToMove, Vector2 newPosition, Action onEnd = null, float time = 0.2f)
    {
        var elapsedTime = 0f;
        Vector2 startingPosition = objectToMove.transform.position;
        while (elapsedTime < time)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPosition, newPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Sync position at the end
        objectToMove.transform.position = newPosition;
        if (onEnd != null)
        {
            onEnd.Invoke();
        }
    }

    private void Normalize()
    {
        // TODO while there are floating elements / 3/more elems in a row/column - check
        bool movedElement = false;
        do
        {
            // Step 1 - move floating elements down
            movedElement = MoveFloatingElements();
            // TODO step 2 - find & destroy matches
        } while (movedElement);
    }

    private bool MoveFloatingElements()
    {
        bool movedElement = false;
        for (int i = 0; i < allElements.GetLength(0); i++) // columns
        {
            for (int j = 1; j < allElements.GetLength(1); j++) // rows
            {
                var element = allElements[i, j];
                if (element != null && allElements[i, j - 1] == null)
                {
                    allElements[i, j - 1] = element;
                    allElements[i, j] = null;
                    element.Row -= 1;
                    StartCoroutine(MoveToPosition(
                        element.gameObject,
                        new Vector2(element.transform.position.x, element.transform.position.y - 1)
                    ));
                    movedElement = true;
                }
            }
        }
        return movedElement;
    }
}
