﻿using UnityEngine;

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

    }

    public virtual void OnDeath()
    {

    }

    public virtual void OnContact(Card InteractionCard)
    {

    }

    public virtual void Move(Vector2Int moveValue)
    {
        // Oblicz aktualne współrzędne w siatce
        int currentX = Mathf.RoundToInt((transform.position.x - gridManager.transform.position.x) / gridManager.cellWidth);
        int currentY = Mathf.RoundToInt((transform.position.z - gridManager.transform.position.z) / gridManager.cellHeight);

        // Oblicz nowe współrzędne
        int newX = currentX + moveValue.x;
        int newY = currentY + moveValue.y;

        if (gridManager.IsCellEmpty(newX, newY))
            cardObject.MoveOnGrid(moveValue.x, moveValue.y);
        else
            OnContact(gridManager.GetCardFromGrid(newX, newY));
    }

    public virtual void OnSpecialAction()
    {

    }

    public virtual bool IsOnMapEdge()
    {
        return false;
    }

}
