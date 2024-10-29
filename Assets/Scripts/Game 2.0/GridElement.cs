using UnityEngine;

public class GridElement : MonoBehaviour
{
    public Vector2Int ElementCoordinate;
    public Vector3 GridSize;
    public Card CardInCell;

    public void SetUp()
    {
        GridSize = transform.lossyScale * 10;
    }

    public void SetUpCoordinate(int x, int y)
    {
        ElementCoordinate = new Vector2Int(x, y);
    }

    public void SetCellOccupied(Card card)
    {
        if (CardInCell == null)
            CardInCell = card;
        else
            Debug.LogError("This slot is already occupied");
    }

    public void SetCellEmpty()
    {
        if (CardInCell != null)
            CardInCell = null;
        else
            Debug.LogError("This slot is Empty");
    }
}
