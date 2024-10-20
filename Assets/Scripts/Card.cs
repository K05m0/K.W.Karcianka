using System;
using UnityEngine;

[System.Serializable]
public class Card : MonoBehaviour
{
    public enum CardType { Enemy, Player }
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

    private void Awake()
    {
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
                    //Logika kończąca gre
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
    }
}
