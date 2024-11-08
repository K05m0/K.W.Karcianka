using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]  // Sprawia, że skrypt działa także w trybie edycji
public class GridController : MonoBehaviour
{
    [SerializeField] private GridElement gridCellPrefab;
    [SerializeField] private float cellElementSize;
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

    [Header("Place")]
    public List<Vector2Int> avaibleGridCellForPlayer = new List<Vector2Int>();
    [SerializeField] private Color hightLightColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color disableColor;


    void OnValidate()
    {
        if (gridCellPrefab != null)
        {
            // Ustaw flagę, że grid wymaga odświeżenia, ale nie rób tego od razu
            needsRefresh = true;
        }
    }

    private void Awake()
    {
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
                cell.gameObject.transform.localScale = cell.gameObject.transform.localScale * cellElementSize;
                cell.SetUp();
                Vector3 cellPosition = new Vector3(startPosition.x + ((cell.GridSize.x + CellWidthOffset) * x),
                                                   startPosition.y,
                                                   startPosition.z + ((cell.GridSize.z + CellHeightOffset) * y));

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
    public void SetCellOccupied(int x, int y, Card card)
    {
        GridCoordinate[x, y].SetCellOccupied(card);
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
        if (GridCoordinate[x, y].CardInCell != null)
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
    public void HightlightGrid(Component sender, object data)
    {
        if (sender is not Card element)
            return;
        if (data is not bool isCliced)
            return;


        // Najpierw znajdź elementy, które mają koordynaty zgodne z `avaibleGridCellForPlayer`
        // Zamiana GridCoordinate na listę jednowymiarową
        var allCells = GridCoordinate.Cast<GridElement>().ToList();

        // Filtruj elementy, które mają koordynaty zgodne z avaibleGridCellForPlayer
        var matchingCells = allCells
            .Where(cell => avaibleGridCellForPlayer.Any(x => x.x == cell.ElementCoordinate.x && x.y == cell.ElementCoordinate.y))
            .ToList();

        // Elementy, których koordynaty się nie pokrywają
        var nonMatchingCells = allCells.Except(matchingCells).ToList();

        // Przetwarzanie elementów, których koordynaty się pokrywają
        foreach (var cell in matchingCells)
        {
            var renderer = cell.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = isCliced ?  hightLightColor : normalColor;
            }
        }

        foreach (var cell in nonMatchingCells)
        {
            var renderer = cell.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = isCliced ?  disableColor : normalColor;
            }
        }

    }
}
/*            var disableGridCell = avaibleGridCellForPlayer.FirstOrDefault(x => x.x != cell.ElementCoordinate.x || x.y != cell.ElementCoordinate.y);
*/
/*            else if (disableGridCell != null)
            {
                var renderer = cell.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = isCliced ? disableColor : normalColor;
                }
            }*/