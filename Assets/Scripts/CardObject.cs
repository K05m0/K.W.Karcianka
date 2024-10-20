using UnityEditor.SceneManagement;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    public Card CardData;
    public GridManager gridManager;

    public virtual void SetUpCard(Card selectedCard)
    {
        CardData = selectedCard;
    }

    public virtual void PlaceOnGrid(int x, int y)
    {

        if (y <= -1)
        {
            y = 0;
        }
        else if (y > gridManager.gridHeight - 1)
        {
            y = gridManager.gridHeight - 1;
        }
        GridCell targetCell = gridManager.GetCell(x, y);

        if (targetCell != null && !targetCell.isOccupied)
        {
            transform.position = targetCell.transform.position;
            targetCell.isOccupied = true;
            targetCell.CardInCell = CardData;
        }
    }

    public virtual void MoveOnGrid(int deltaX, int deltaY)
    {
        // Oblicz aktualne współrzędne w siatce
        int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
        int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

        // Oblicz nowe współrzędne
        int newX = currentX + deltaX;
        int newY = currentY + deltaY;

        // Przemieszczanie na nową pozycję w gridzie
        PlaceOnGrid(newX, newY);
    }
}

