using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class PlayerCard : CardObject
{
    private Vector3 originalPosition; // Miejsce początkowe karty
    private Vector3 originalScale; // Miejsce początkowe karty
    private quaternion originalRotation; // Miejsce początkowe karty
    private Vector3 offset;
    private float zCoord;
    private PlayerManager playerManager;
    [SerializeField] private LayerMask mask = (1 << 7) | (1 << 8);
    private Transform targetedObject;
    [SerializeField] private LayerMask gridLayer = 1 << 7;
    private bool IsPlaced = false;
    private GameObject holder = null;
    [SerializeField] private float offestUp = 0.15f;
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
        originalScale = transform.localScale;
        originalRotation = transform.rotation;

        // Obliczamy offset
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (holder != null)
        {
            holder.transform.GetChild(0).gameObject.SetActive(false);
            holder = null;
        }

        // Przeciąganie karty
        transform.position = GetMouseWorldPos() + offset;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.magenta, 3f);
            var newScale = originalScale * 0.5f;
            transform.localScale = newScale;
            transform.rotation = new Quaternion(0, 180, 0, 0);



            if (hit.collider.TryGetComponent<GridCell>(out var grid))
            {
                grid.isTargeted = true;
                if(holder != null && holder != grid.gameObject)
                {
                    holder.transform.GetChild(0).gameObject.SetActive(false);
                    holder = null;
                }
                grid.transform.GetChild(0).gameObject.SetActive(true);
                holder = grid.gameObject;
            }
        }
        // RayCast na Grida / przełączenie boola w nim zmienienie wyglądu grida
    }

    void OnMouseUp()
    {

        Debug.Log("stopDrag");
        // Znalezienie najbliższej komórki w gridzie

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 15f);


        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, gridLayer))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green, 15f);

            GridCell nearestCell = hit.collider.gameObject.GetComponent<GridCell>();

            if (holder != null && holder)
            {
                holder.transform.GetChild(0).gameObject.SetActive(false);
                holder = null;
            }

            // Sprawdzenie, czy komórka jest wolna i czy karta jest wystarczająco blisko
            if (nearestCell != null && !nearestCell.isOccupied)
            {
                if (!PlayCard(CardData))
                {
                    transform.position = originalPosition;
                    transform.localScale = originalScale;
                    transform.rotation = originalRotation;
                    return;
                }

                // Przenieś kartę do tej komórki
                PlaceOnGrid(nearestCell.coordinates.x, nearestCell.coordinates.y); // Użyj metody PlaceOnGrid
                transform.SetParent(nearestCell.transform);
                transform.localScale = new Vector3(1, 1, 1);
                transform.localPosition = Vector3.zero;
                transform.localRotation = new Quaternion(0,180,0,0);
                IsPlaced = true;


                // Oznacz komórkę jako zajętą
                nearestCell.isOccupied = true;
                nearestCell.CardInCell = CardData;
            }
            else
            {
                // Powrót do oryginalnej pozycji, jeśli komórka jest zajęta lub nie ma najbliższej komórki
                transform.position = originalPosition;
                transform.localScale = originalScale;
                transform.rotation = originalRotation;
            }
        }
        else
        {
            Debug.DrawLine(ray.origin, hit.point, Color.magenta, 15f);

            // Powrót do oryginalnej pozycji, jeśli komórka jest zajęta lub nie ma najbliższej komórki
            transform.position = originalPosition;
            transform.position = originalPosition;
            transform.localScale = originalScale;
            transform.rotation = originalRotation;
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
            transform.DOShakeRotation(0.3f, 15f, 0, 40, true);
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
            if (playerManager.PlacedCard.Contains(CardData))
            {
                playerManager.ModifyPlacedCard(CardData, true, true);

            }
            else
            {
                playerManager.ModifyPlacedCard(CardData, true, false);

            }
        }
        else
        {
            transform.position = originalPosition; // Jeśli nie można umieścić, wróć do oryginalnej pozycji
        }
    }
}
