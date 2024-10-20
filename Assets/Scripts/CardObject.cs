using UnityEditor.SceneManagement;
using UnityEngine;

public  class CardObject : MonoBehaviour
{
    public Card CardData;

    public virtual void SetUpCard(Card selectedCard)
    {
        CardData = selectedCard;
    }


}

