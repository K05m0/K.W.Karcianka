using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    [Header("reference")]
    public GridController Controller;

    [Header("Card Draw")]
    public int StartCardAmount = 3;
    public List<Card> AllCardsInDeck = new List<Card>();

    public List<Transform> CardSlots;
    public Card[] CardInHand;

    public void StartGame()
    {
        CardInHand = new Card[CardSlots.Count];
        StartDraw();
    }
    //Draw
    public void StartDraw()
    {
        for (int i = 0; i < StartCardAmount; i++)
        {
            DrawRandomCard();
        }
    }
    public void DrawRandomCard()
    {
        if (AllCardsInDeck.Count <= 0)
            return;

        Card randCard = AllCardsInDeck[UnityEngine.Random.Range(0, AllCardsInDeck.Count)];

        for (int i = 0; i < CardSlots.Count; i++)
        {
            if (CardInHand[i] != null)
                continue;


            Card card = Instantiate(randCard, CardSlots[i]);
            card.SetUpCard(Controller);
            /*Card card = Instantiate(randCard, CardSlots[i]);
            PlayerCard cardObject = card.AddComponent<PlayerCard>();
            card.SetUpCard(cardObject);

            cardObject.SetUpCard(card);
            cardObject.gameObject.name = cardObject.name;
            cardObject.transform.position = CardSlots[i].position;
            CardInHand[i] = cardObject.CardData;
            AllCardsInDeck.Remove(randCard);
            return;
*/
        }

    }
}
