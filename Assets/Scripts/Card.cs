using UnityEngine;

[System.Serializable]
public class Card : MonoBehaviour
{
    public string CardName;
    public int CardCost = 2;

    public Card(string cardName, int cardCost)
    {
        CardName = cardName;
        CardCost = cardCost;
    }

    public virtual void MakeCardTurn()
    {

    }

    public virtual void OnSpawn()
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

    public virtual void Move()
    {

    }

    public virtual void OnSpecialAction()
    {

    }

    public virtual bool IsOnMapEdge()
    {
        return false;
    }

}
