using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Element : MonoBehaviour
{
    public ElementType Type;
    public int Column;
    public int Row;

    public float SwipeAngle = 0;
    public bool CanDestroy = true;

    private Board grid;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    public void Setup(Board grid, int column, int row)
    {
        this.grid = grid;
        Column = column;
        Row = row;
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (!Board.IsInputEnabled) return;
        Board.IsInputEnabled = false;
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SwipeAngle =
            Mathf.Atan2(
                finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x)
            * 180 / Mathf.PI;
        grid.SwipeElement(this, SwipeAngle);
    }
}
