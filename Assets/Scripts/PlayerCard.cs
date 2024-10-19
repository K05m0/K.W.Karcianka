using UnityEngine;

public class PlayerCard : CardObject
{
    private Vector3 originalPosition; // Miejsce początkowe karty
    private Vector3 offset;
    private float zCoord;
    private GridManager gridManager; // Odwołanie do managera gridu
    private PlayerManager playerManager;

    void Start()
    {
        // Zakładamy, że gridManager jest w tej samej scenie
        gridManager = FindObjectOfType<GridManager>();
        playerManager = FindAnyObjectByType<PlayerManager>();
    }

    void OnMouseDown()
    {
        // Zapamiętujemy początkową pozycję karty
        originalPosition = transform.position;

        // Obliczamy offset
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        // Przeciąganie karty
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        // Znalezienie najbliższej komórki w gridzie
        GridCell nearestCell = gridManager.GetNearestCell(transform.position);

        // Sprawdzenie, czy komórka jest wolna i czy karta jest wystarczająco blisko
        if (nearestCell != null && !nearestCell.isOccupied)
        {
            if (!PlayCard(CardData))
            {
                transform.position = originalPosition;
                return;
            }

            // Przenieś kartę do tej komórki
            PlaceOnGrid(nearestCell.coordinates.x, nearestCell.coordinates.y); // Użyj metody PlaceOnGrid

            // Oznacz komórkę jako zajętą
            nearestCell.isOccupied = true;
        }
        else
        {
            // Powrót do oryginalnej pozycji, jeśli komórka jest zajęta lub nie ma najbliższej komórki
            transform.position = originalPosition;
        }
    }

    // Metoda do umieszczania karty w danej komórce gridu
    private void PlaceOnGrid(int x, int y)
    {
        GridCell targetCell = gridManager.GetCell(x, y);

        if (targetCell != null && !targetCell.isOccupied)
        {
            transform.position = targetCell.transform.position;
            targetCell.isOccupied = true;
        }
        else
        {
            transform.position = originalPosition; // Jeśli nie można umieścić, wróć do oryginalnej pozycji
        }
    }

    // Metoda do przemieszczania karty w obrębie gridu
    public void MoveOnGrid(int deltaX, int deltaY)
    {
        int currentX = Mathf.RoundToInt(transform.position.x / gridManager.cellSize);
        int currentY = Mathf.RoundToInt(transform.position.z / gridManager.cellSize);

        int newX = currentX + deltaX;
        int newY = currentY + deltaY;

        PlaceOnGrid(newX, newY);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private bool PlayCard(Card usedCard)
    {
        if (playerManager.UseMana(usedCard.CardCost))
        {
            playerManager.UseSlot(usedCard);
            return true;
        }
        else
        {
            return false;
        }
    }
}
