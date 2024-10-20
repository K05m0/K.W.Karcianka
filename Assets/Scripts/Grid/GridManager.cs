using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]  // Sprawia, że skrypt działa także w trybie edycji
public class GridManager : MonoBehaviour
{
    public GameObject gridCellPrefab; // Prefab komórki gridu
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float cellWidth = 1.0f;   // Szerokość komórki
    public float cellHeight = 1.0f;  // Wysokość komórki

    private GridCell[,] gridCells;
    private int lastGridWidth;
    private int lastGridHeight;
    private float lastCellWidth;
    private float lastCellHeight;
    private bool needsRefresh = false;  // Flaga do opóźnienia odświeżania

    // OnValidate jest wywoływana automatycznie przy każdej zmianie w inspektorze Unity
    void OnValidate()
    {
        if (gridCellPrefab != null)
        {
            // Ustaw flagę, że grid wymaga odświeżenia, ale nie rób tego od razu
            needsRefresh = true;
        }
    }

    // Sprawdzamy, czy grid ma się odświeżyć na podstawie dynamicznych zmian
    void Update()
    {
        // Jeśli potrzebne jest odświeżenie gridu (ustawione w OnValidate), odśwież teraz
        if (needsRefresh || gridWidth != lastGridWidth || gridHeight != lastGridHeight || cellWidth != lastCellWidth || cellHeight != lastCellHeight)
        {
            RefreshGrid();
            needsRefresh = false;  // Resetujemy flagę po odświeżeniu
        }
    }

    // Metoda do odświeżania gridu
    public void RefreshGrid()
    {
        // Aktualizujemy zapisane wartości parametrów
        lastGridWidth = gridWidth;
        lastGridHeight = gridHeight;
        lastCellWidth = cellWidth;
        lastCellHeight = cellHeight;

        ClearGrid();  // Usunięcie istniejących komórek gridu
        GenerateGrid();  // Ponowne wygenerowanie nowego gridu
    }

    // Generowanie gridu
    void GenerateGrid()
    {
        if (gridCells != null && gridCells.Length > 0)
        {
            ClearGrid();
        }

        gridCells = new GridCell[gridWidth, gridHeight];

        Vector3 startPosition = transform.position;  // Pobieramy pozycję startową z obiektu, który posiada tę klasę

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Ustawiamy pozycję każdej komórki bazując na pozycji obiektu z GridManager i rozmiarach komórek
                Vector3 cellPosition = new Vector3(startPosition.x + x * cellWidth, startPosition.y, startPosition.z + y * cellHeight);
                GameObject cell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, this.transform); // Dodajemy nowo wygenerowane komórki jako dzieci obiektu GridManager
                GridCell gridCell = cell.GetComponent<GridCell>();
                gridCell.SetCoordinates(x, y);
                gridCells[x, y] = gridCell;
            }
        }
    }

    // Usuwanie istniejących komórek gridu
    void ClearGrid()
    {
        // Usuwamy wszystkie dzieci (komórki) pod obiektem GridManager
        if (transform.childCount == 0)
            return; // Jeśli nie ma dzieci, nic nie rób

        var children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        foreach (var child in children)
        {
            // Używamy DestroyImmediate w edytorze
            if (Application.isPlaying)
            {
                Destroy(child); // Usuwanie podczas uruchomionej gry
            }
            else
            {
                DestroyImmediate(child); // Natychmiastowe usuwanie w edytorze
            }
        }

        // Czyszczenie tablicy gridCells
        gridCells = null;
    }

    // Metoda, która zwraca najbliższą komórkę gridu do danej pozycji
    public GridCell GetNearestCell(Vector3 worldPosition)
    {
        if (gridCells == null)
        {
            Debug.LogWarning("Grid cells are not initialized. Please generate the grid first.");
            return null;
        }

        float closestDistance = 1f;
        GridCell closestCell = null;

        foreach (GridCell cell in gridCells)
        {
            float distance = Vector3.Distance(cell.transform.position, worldPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCell = cell;
            }
        }

        return closestCell;
    }

    public GridCell GetCell(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            Debug.LogWarning("Requested cell is out of bounds.");
            return null; // Zwróć null, jeśli współrzędne są poza zakresem
        }

        return gridCells[x, y]; // Zwróć odpowiednią komórkę
    }

    // Metoda sprawdzająca, czy komórka o współrzędnych x i y jest pusta
    public bool IsCellEmpty(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            Debug.LogWarning("Requested cell is out of bounds.");
            return false; // Zwróć false, jeśli współrzędne są poza zakresem
        }

        if (!gridCells[x, y].isOccupied)
            return true;
        else
            return false; // Zwraca true, jeśli komórka jest pusta (null)
    }

    public Card GetCardFromGrid(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            Debug.LogWarning("Requested cell is out of bounds.");
            return null; // Zwróć false, jeśli współrzędne są poza zakresem
        }

        if(gridCells[x, y].CardInCell == null)
        {
            Debug.Log("Requested cell is empty");
            return null;
        }

        return gridCells[x, y].CardInCell;
    }
}
