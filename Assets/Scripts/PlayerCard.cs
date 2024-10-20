using UnityEngine;

public class PlayerCard : CardObject
{
    private Vector3 originalPosition; // Miejsce początkowe karty
    private Vector3 offset;
    private float zCoord;
    private PlayerManager playerManager;
    private GridManager gridManager; // Odwołanie do managera gridu
    private LayerMask mask = 6;
    private Transform targetedObject;
    [SerializeField] private LayerMask gridLayer = 1 << 7;
    private bool IsPlaced = false;
    void Start()
    {
        // Zakładamy, że gridManager jest w tej samej scenie
        gridManager = FindObjectOfType<GridManager>();
        playerManager = FindAnyObjectByType<PlayerManager>();
    }

    void OnMouseDown()
    {
        if (IsPlaced)
        {
            return;
        }
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,Mathf.Infinity,mask.value))
        {
            hit.transform.GetComponent<GridCell>().isTargeted = true;
        }
        // RayCast na Grida / przełączenie boola w nim zmienienie wyglądu grida
    }

    void OnMouseUp()
    {

        Debug.Log("stopDrag");
        // Znalezienie najbliższej komórki w gridzie

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red,15f);


        if (Physics.Raycast(ray,out var hit,Mathf.Infinity,gridLayer))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green,15f);

            GridCell nearestCell = hit.collider.gameObject.GetComponent<GridCell>();

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
                transform.SetParent(nearestCell.transform);
                transform.localScale = new Vector3(1, 1, 1);
                transform.localPosition = Vector3.zero;
                transform.rotation = Quaternion.identity;
                IsPlaced = true;


                // Oznacz komórkę jako zajętą
                nearestCell.isOccupied = true;
                nearestCell.CardInCell = CardData;
            }
            else
            {
                // Powrót do oryginalnej pozycji, jeśli komórka jest zajęta lub nie ma najbliższej komórki
                transform.position = originalPosition;
            }
        }
        else
        {
            Debug.DrawLine(ray.origin, hit.point, Color.magenta, 15f);

            // Powrót do oryginalnej pozycji, jeśli komórka jest zajęta lub nie ma najbliższej komórki
            transform.position = originalPosition;
        }
    }

    // Metoda do umieszczania karty w danej komórce gridu
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

            usedCard.OnSpawn();
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void PlaceOnGrid(int x, int y)
    {
        GridCell targetCell = gridManager.GetCell(x, y);

        if (targetCell != null && !targetCell.isOccupied)
        {
            transform.position = targetCell.transform.position;
            targetCell.isOccupied = true;
            targetCell.CardInCell = CardData;

        }
        else
        {
            transform.position = originalPosition; // Jeśli nie można umieścić, wróć do oryginalnej pozycji
        }
    }
}
