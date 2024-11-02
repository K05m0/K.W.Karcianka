using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]  // Sprawia, że skrypt działa także w trybie edycji
public class GridController : MonoBehaviour
{
    [SerializeField] private GridElement gridCellPrefab;
    public int GridWidth;
    public int GridHeight;
    public float CellWidthOffset;
    public float CellHeightOffset;

    //Data
    public GridElement[,] GridCoordinate;

    //refresh
    private int lastGridWidth;
    private int lastGridHeight;
    private float lastCellWidth;
    private float lastCellHeight;
    private bool needsRefresh = false;  // Flaga do opóźnienia odświeżania


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
        if (needsRefresh ||
            GridWidth != lastGridWidth ||
            GridHeight != lastGridHeight ||
            CellWidthOffset != lastCellWidth ||
            CellHeightOffset != lastCellHeight)
        {
            RefreshGrid();
            needsRefresh = false;  // Resetujemy flagę po odświeżeniu
        }
    }

    // Metoda do odświeżania gridu
    public void RefreshGrid()
    {
        // Aktualizujemy zapisane wartości parametrów
        lastGridWidth = GridWidth;
        lastGridHeight = GridHeight;
        lastCellWidth = CellWidthOffset;
        lastCellHeight = CellHeightOffset;

        ClearGrid();  // Usunięcie istniejących komórek gridu
        GenerateGrid();  // Ponowne wygenerowanie nowego gridu
    }

    public void GenerateGrid()
    {
        if (GridCoordinate != null && GridCoordinate.Length > 0)
        {
            ClearGrid();
        }

        GridCoordinate = new GridElement[GridWidth, GridHeight];
        Vector3 startPosition = transform.position;

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                GridElement cell = Instantiate(gridCellPrefab, transform);
                cell.SetUp();
                Vector3 cellPosition = new Vector3(startPosition.x + ((cell.GridSize.x * x) + CellWidthOffset),
                                                   startPosition.y,
                                                   startPosition.z + ((cell.GridSize.z * y) + CellWidthOffset));
                cell.gameObject.transform.localPosition = cellPosition;
                cell.transform.rotation = Quaternion.identity;
                cell.SetUpCoordinate(x, y);
                GridCoordinate[x, y] = cell;
            }
        }
    }
    private void ClearGrid()
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
        GridCoordinate = null;
    }

    // Grid Operation
    public void SetCellOccupied(int x , int y, Card card)
    {
        GridCoordinate[x,y].SetCellOccupied(card);
    }
    public void SetCellOccupied(GridElement cell, Card card)
    {
        Vector2Int position = cell.ElementCoordinate;
        SetCellOccupied(position.x, position.y, card);
    }
    public void SetCellEmpty(int x, int y)
    {
        GridCoordinate[x, y].SetCellEmpty();
    }
    public void SetCellEmpty(GridElement cell)
    {
        Vector2Int position = cell.ElementCoordinate;
        SetCellEmpty(position.x, position.y);
    }
    // return true if is not occupied;
    public bool IsSlotOccupied(int x, int y)
    {
        if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight)
        {
            Debug.LogWarning("Requested cell is out of bounds.");
            return false; // Zwróć false, jeśli współrzędne są poza zakresem
        }
        if (GridCoordinate[x,y].CardInCell != null)
        {
            return false;
        }

        return true; 
    }
    public bool IsSlotOccupied(GridElement cell)
    {
        return IsSlotOccupied(cell.ElementCoordinate.x, cell.ElementCoordinate.y);
    }
    public Card GetCardFromGrid(int x, int y)
    {
        if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight)
        {
            Debug.LogWarning("Requested cell is out of bounds.");
            return null; // Zwróć false, jeśli współrzędne są poza zakresem
        }

        if (GridCoordinate[x, y].CardInCell == null)
        {
            Debug.Log("Requested cell is empty");
            return null;
        }

        return GridCoordinate[x, y].CardInCell;
    }
    public Card GetCardFromGrid(GridElement cell)
    {
        Vector2Int position = cell.ElementCoordinate;
        return GetCardFromGrid(position.x, position.y);
    }
    public GridElement GetCell(int x, int y)
    {
        if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight)
        {
            Debug.LogWarning("Requested cell is out of bounds.");
            return null; // Zwróć null, jeśli współrzędne są poza zakresem
        }

        return GridCoordinate[x, y]; // Zwróć odpowiednią komórkę
    }
}
