using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int coordinates;  // Koordynaty (x, y) komórki
    public bool isOccupied = false; // Czy komórka jest zajęta czy nie

    // Metoda opcjonalna do ustawienia koordynatów podczas tworzenia gridu
    public void SetCoordinates(int x, int y)
    {
        coordinates = new Vector2Int(x, y);
    }
}
