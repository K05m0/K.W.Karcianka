using UnityEngine;

[System.Serializable]
public class Card : MonoBehaviour
{
    public string CardName;
    public int CardCost = 2;

    [Header("Stats")]
    public Vector2Int moveSpeed = new Vector2Int();
    [HideInInspector] public int CurrHp = 0;
    public int MaxHp = 0;
    public int AttackDmg;


    [HideInInspector] private CardObject cardObject;
    [HideInInspector] private GridManager gridManager;

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
        Move(moveSpeed);
    }

    public virtual void OnSpawn()
    {
        Debug.Log($"{CardName} : spawned");
    }

    public virtual void OnLoseFight()
    {
        var yToBack = -moveSpeed.y;

        int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
        int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

        // Oblicz nowe współrzędne
        int newX = currentX;
        int newY = currentY + yToBack;

        if (gridManager.IsCellEmpty(newX, newY))
            Move(new Vector2Int(0, newY - currentY));
        else
            OnDeath();
    }

    public virtual void OnDeath()
    {
        int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
        int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

        gridManager.GetCell(currentX,currentY).CardInCell = null;
        gridManager.GetCell(currentX, currentY).isOccupied = false;


        Destroy(gameObject);
    }

    public virtual void OnContact(Card InteractionCard)
    {
        DealDmg(InteractionCard);
    }

    public virtual void Move(Vector2Int moveValue)
    {
        if(IsOnEdge())
        {

        }
        else
        {

            // Oblicz aktualne współrzędne w siatce
            int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
            int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

            // Oblicz nowe współrzędne
            int newX = currentX + moveValue.x;
            int newY = currentY + moveValue.y;

            if (gridManager.IsCellEmpty(newX, newY))
            {
                gridManager.GetCell(currentX, currentY).CardInCell = null;
                gridManager.GetCell(currentX, currentY).isOccupied = false;
                cardObject.MoveOnGrid(moveValue.x, moveValue.y);
            }
            else
            {
                OnContact(gridManager.GetCardFromGrid(newX, newY));
            }
        }
    }

    public virtual void OnSpecialAction()
    {

    }

    public virtual bool IsOnEdge()
    {
        return false;
    }

    public virtual bool OnGridLeave()
    {
        return false;
    }

    public virtual void DealDmg(Card target)
    {
        target.CurrHp -= AttackDmg;
        if(target.CurrHp > 0)
        {
            target.OnLoseFight();
        }
        else
        {
            target.OnDeath();
        }
    }
}
