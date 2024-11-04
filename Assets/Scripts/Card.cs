using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.WSA;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public class Card : MonoBehaviour
{
    public enum CardType { Enemy, Player }
    public CardType Type = CardType.Player;
    public bool InDeck = true;
    public bool OnBoard;
    public bool IsCliced;

    private GridController Controller;

    [Header("Stats")]
    public int Speed;
    public int Cost;
    public int MaxHealth;
    public int CurrHealth;

    [Header("InHandPosition")]
    [SerializeField, Range(-2f, 2f)] private float showOffset;
    [SerializeField, Range(0f, 3f)] private float showDuration;
    private Transform handPos;
    private Vector3 showCardPosition;
    private Vector3 initialPosition;

    [Header("Drag")]
    [SerializeField] private LayerMask cardLayer;
    private float zCoord;
    private Vector3 offset;
    private GridElement holder = null;
    Vector3 originalSize;

    public virtual void SetUpCard(GridController controller, CardType type = CardType.Player, Transform handTransform = null)
    {
        //reference
        Controller = controller;
        handPos = handTransform;
        //pos
        transform.SetParent(handPos, false);
        transform.localPosition = Vector3.zero;
        initialPosition = transform.position;
        originalSize = transform.localScale;
        showCardPosition = initialPosition + (transform.forward * showOffset);
        //Stats
        Type = type;
        CurrHealth = MaxHealth;
        InDeck = false;
    }

    //CARD MOUSE LOGIC
    public void OnMouseEnter()
    {
        if (InDeck)
            return;

        transform.DOPause();
        if (IsCliced)
            return;
        transform.DOMove(showCardPosition, showDuration);
    }

    public void OnMouseExit()
    {
        if (InDeck)
            return;

        transform.DOPause();
        if (IsCliced)
            return;
        transform.DOMove(initialPosition, showDuration * 0.5f);
    }

    private void OnMouseDown()
    {
        if (InDeck)
            return;

        if (OnBoard)
        {
            return;
        }
        IsCliced = true;

        // Obliczamy offset
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    private void OnMouseUp()
    {
        if (InDeck)
            return;

        BackCard();
    }

    private void PlaceCard()
    {
        if (InDeck)
            return;
    }
    private void BackCard()
    {
        if (InDeck)
            return;

        IsCliced = false;
        transform.SetParent(handPos);
        transform.localScale = originalSize;
        transform.DOPause();
        if (IsCliced)
            return;
        transform.DOMove(initialPosition, showDuration * 0.5f);
    }
    private void OnMouseDrag()
    {
        if (InDeck)
            return;

        // Przeciąganie karty
        transform.position = GetMouseWorldPos() + offset;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = ~cardLayer.value;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);


            if (hit.collider.TryGetComponent<GridElement>(out var grid))
            {
                transform.SetParent(grid.transform);
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                grid.isTargeted = true;
                if (holder != null && holder != grid.gameObject)
                {
                    /*holder.transform.GetChild(0).gameObject.SetActive(false);*/
                    holder = null;
                }
                grid.transform.GetChild(0).gameObject.SetActive(true);
                holder = grid;
                return;
            }
            else
            {
                transform.SetParent(handPos);
                transform.localScale = originalSize;
                transform.position = hit.point + new Vector3(0, 0.01f, 0);
            }
        }

        if (holder != null)
        {
            holder.isTargeted = false;
            holder = null;
        }


    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    //CARD ON BOARD LOGIC

    public virtual void OnSpawn()
    {

    }

    public virtual void OnCardTurn()
    {

    }

    public virtual void OnLoseFight()
    {

    }

    public virtual void OnDeath()
    {

    }

    public virtual void OnContact(Card InteractionCard)
    {

    }

    public virtual bool IsOnEdge()
    {
        return true;
    }

    /*public enum CardType { Enemy, Player }
    public CardType Type = CardType.Player;
    public string CardName;
    public int CardCost = 2;

    [Header("Stats")]
    public Vector2Int moveSpeed = new Vector2Int();
    [HideInInspector] public int CurrHp = 0;
    public int MaxHp = 0;
    public int AttackDmg;

    [HideInInspector] private CardObject cardObject;
    [HideInInspector] private GridManager gridManager;
    public static event EventHandler<CardDeathEventArgs> OnCardDeath;
    public static event EventHandler DefeatEvent;

    public Sprite cardSprite;
    public string description;

    public VisualEffect VFX;

    [SerializeField] private AudioClip spawn, death, attack;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
       audioSource = GetComponent<AudioSource>();
        CurrHp = MaxHp;
        gridManager = FindAnyObjectByType<GridManager>();
    }

    public virtual void SetUpCard(CardObject obj)
    {
        cardObject = obj;
    }

    public virtual void MakeCardTurn()
    {
        Move(GetAdjustedMoveSpeed());
    }

    public virtual void OnSpawn()
    {
        audioSource.PlayOneShot(spawn);
        Debug.Log($"{CardName} : spawned");
    }

    public virtual void OnLoseFight()
    {
        var yToBack = -GetAdjustedMoveSpeed().y;
        int direction = yToBack > 0 ? 1 : -1;  // Kierunek ruchu (w górę lub w dół)

        int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
        int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

        // Oblicz nowe współrzędne
        int newX = currentX;
        int newY = currentY + direction;

        if (gridManager.IsCellEmpty(newX, newY))
            Move(new Vector2Int(0, newY - currentY));
        else
            OnDeath();
    }

    public virtual void OnDeath()
    {
        audioSource.PlayOneShot(death);

        int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
        int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

        gridManager.GetCell(currentX, currentY).CardInCell = null;
        gridManager.GetCell(currentX, currentY).isOccupied = false;

        OnCardDeath?.Invoke(this, new CardDeathEventArgs(this));

        Destroy(gameObject);
    }

    public virtual void OnContact(Card InteractionCard)
    {
        DealDmg(InteractionCard);
    }

    // Funkcja poruszająca kartę o jeden krok na raz, z przerwaniem na kontakt
    public virtual void Move(Vector2Int moveValue)
    {
        int steps = Mathf.Abs(moveValue.y);  // Ilość kroków, jaką musimy wykonać
        int direction = moveValue.y > 0 ? 1 : -1;  // Kierunek ruchu (w górę lub w dół)

        // Oblicz aktualne współrzędne
        int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
        int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

        for (int i = 0; i < steps; i++)  // Iterujemy przez kolejne kroki
        {
            if (IsOnEdge())
            {
                if (Type == CardType.Player)
                {
                    OnCardDeath?.Invoke(this, new CardDeathEventArgs(this));
                    Destroy(gameObject);
                }
                else
                {
                    DefeatEvent?.Invoke(this, EventArgs.Empty);
                }
            }

            int newY = currentY + direction;  // Nowa współrzędna Y (każdy krok porusza o 1)

            if (!gridManager.IsCellEmpty(currentX, newY))
            {
                if (!gridManager.IsCellEmpty(currentX, newY))
                {
                    OnContact(gridManager.GetCardFromGrid(currentX, newY));
                }
                break;  // Kończymy ruch, jeśli napotkaliśmy przeszkodę lub innego przeciwnika
            }
            // Przesuwamy kartę na nowe współrzędne
            cardObject.MoveOnGrid(0, direction);

            // Aktualizacja obecnych współrzędnych
            currentY = newY;

            // Aktualizacja siatki
            gridManager.GetCell(currentX, currentY - direction).CardInCell = null;  // Poprzednia komórka jest pusta
            gridManager.GetCell(currentX, currentY).CardInCell = this;  // Nowa komórka jest zajęta
        }
    }

    public virtual void PreMoveSpecialAction()
    {
        Debug.Log("PreMove Special");
    }
    public virtual void AfterMoveSpecialAction()
    {
        Debug.Log("AfterMove Special");
    }

    public virtual bool IsOnEdge()
    {
        int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
        int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

        // Zastępujemy porównanie z moveSpeed.y sprawdzeniem typu karty
        if (Type == CardType.Player)
        {
            if (currentY == 4) // Player porusza się w górę (krawędź górna)
                return true;
            else
                return false;
        }
        else if (Type == CardType.Enemy)
        {
            if (currentY == 0) // Enemy porusza się w dół (krawędź dolna)
                return true;
            else
                return false;
        }
        else
        {
            Debug.LogError("Lipa");
            return false;
        }
    }

    public virtual bool OnGridLeave()
    {
        return false;
    }

    public virtual void DealDmg(Card target)
    {
        if (target == null)
            return;
        target.CurrHp -= AttackDmg;
        audioSource.PlayOneShot(spawn);
        if (target.CurrHp > 0)
        {
            target.OnLoseFight();
        }
        else
        {
            target.OnDeath();
        }

        int direction = Type == CardType.Player  ? 1 : -1;  // Kierunek ruchu (w górę lub w dół)
        cardObject.MoveOnGrid(0, direction);
    }

    // Metoda do przekształcania wartości moveSpeed w zależności od typu karty
    private Vector2Int GetAdjustedMoveSpeed()
    {
        if (Type == CardType.Enemy)
        {
            return new Vector2Int(moveSpeed.x, -Mathf.Abs(moveSpeed.y)); // Enemy porusza się w dół, więc wartość y jest ujemna
        }
        else
        {
            return new Vector2Int(moveSpeed.x, Mathf.Abs(moveSpeed.y)); // Player porusza się w górę, więc wartość y jest dodatnia
        }
    }*/
}
