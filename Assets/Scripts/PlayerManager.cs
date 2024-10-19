using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [Header("Card Draw")]
    public PlayerCard cardPrefab;
    public int StartCardAmount = 3;
    public List<Card> AllCardsInDeck = new List<Card>();

    public List<Transform> CardSlots;
    public Card[] CardInHand;

    [Header("Mana")]
    public int CurrMana = 0;
    public int MaxManaIsThisRound = 3;
    public int MaxGameMana = 10;

    [Header("Placed Card")]
    public List<Card> PlacedCard = new List<Card>();

    private void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            Card card = new Card($"Card {i}", 2);
            AllCardsInDeck.Add(card);
        }
        CardInHand = new Card[CardSlots.Count];
        Debug.Log(CardSlots.Count);
        StartDraw();
        ResetMana();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawRandomCard();
            IncreaseMana();
            ResetMana();
        }
    }
    public void StartGame()
    {
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
        if (AllCardsInDeck.Count >= 1)
        {
            Card randCard = AllCardsInDeck[UnityEngine.Random.Range(0, AllCardsInDeck.Count)];
            for (int i = 0; i < CardSlots.Count; i++)
            {
                if (CardInHand[i] == null)
                {
                    PlayerCard cardObject = Instantiate(cardPrefab, CardSlots[i]);
                    cardObject.SetUpCard(randCard);
                    cardObject.gameObject.name = cardObject.name;
                    cardObject.transform.position = CardSlots[i].position;
                    CardInHand[i] = cardObject.CardData;
                    AllCardsInDeck.Remove(randCard);
                    return;
                }
            }
        }
    }

    //Use Card
    public void UseSlot(Card usedCard)
    {
        for (int i = 0; i < CardInHand.Length; i++)
        {
            if (CardInHand[i] != null && CardInHand[i].Equals(usedCard))
            {
                CardInHand[i] = null;

                Debug.Log("Znaleziono i zmieniono wartość na null: " + usedCard);
                break; // Możesz usunąć break, jeśli chcesz zmodyfikować wszystkie wystąpienia
            }
        }
    }
    public void ModifyPlacedCard(Card usedCard, bool isAdd)
    {
        if(isAdd)
        {
            if(PlacedCard.Contains(usedCard))
            {
                Debug.LogError("this card is placed");
                return;
            }

            PlacedCard.Add(usedCard);
            return;
        }
        else
        {
            if(!PlacedCard.Contains(usedCard))
            {
                Debug.LogError("this card not exits in list");
                return;
            }
            PlacedCard.Remove(usedCard);
            return;
        }
    }

    //Mana
    public void ResetMana()
    {
        CurrMana = MaxManaIsThisRound;
    }
    public void IncreaseMana()
    {
        if (MaxManaIsThisRound < MaxGameMana)
        {
            MaxManaIsThisRound++;
        }
        else
        {
            Debug.Log("We have Max Mana");
        }
    }
    public bool UseMana(int Cost)
    {
        if (Cost <= CurrMana)
        {
            CurrMana -= Cost;
            return true;
        }
        else
        {
            Debug.Log("To Low Mana");
            return false;
        }
    }


}


