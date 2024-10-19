using UnityEngine;

public abstract class CardObject : MonoBehaviour
{
    public Card CardData;



    public virtual void SetUpCard(Card selectedCard)
    {
        CardData = selectedCard;
    }


}

