using UnityEngine;

public class EnemyCard : CardObject
{
    public enum CardState { Preparing, Created }
    public CardState currentState = CardState.Preparing;

    private GridManager gridManager; // Odwołanie do managera gridu

    void Awake()
    {
        // Zakładamy, że gridManager jest w tej samej scenie
        gridManager = FindObjectOfType<GridManager>();
    }

    // Metoda do umieszczania karty w danej komórce gridu
    public void PlaceOnGrid(int x, int y)
    {
        GridCell targetCell = gridManager.GetCell(x, y);

        if (targetCell != null && !targetCell.isOccupied)
        {
            transform.position = targetCell.transform.position;
            transform.SetParent(targetCell.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            targetCell.isOccupied = true;
            currentState = CardState.Created; // Zmiana stanu na "stworzone"
        }
        else
        {
            Debug.LogWarning("Cell is occupied or does not exist.");
        }
    }

    // Metoda do przemieszczania karty w obrębie gridu
    public void MoveOnGrid(int deltaX, int deltaY)
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
