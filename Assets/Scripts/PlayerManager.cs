﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Card Draw")]
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

    public void StartGame()
    {
        CardInHand = new Card[CardSlots.Count];
        StartDraw();
        ResetMana();
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
                    Card card = Instantiate(randCard, CardSlots[i]);
                    PlayerCard cardObject = card.AddComponent<PlayerCard>();
                    card.SetUpCard(cardObject);

                    cardObject.SetUpCard(card);
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
    public void ModifyPlacedCard(Card usedCard, bool isAdd, bool isMove)
    {
        if (isAdd)
        {
            if (!isMove)
            {

                if (PlacedCard.Contains(usedCard))
                {
                    Debug.LogError("this card is placed");
                    return;
                }
                PlacedCard.Add(usedCard);
                return;
            }

            PlacedCard.Add(usedCard);
            return;
        }
        else
        {
            if (!PlacedCard.Contains(usedCard))
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
        ManaInfromationEvent.current.ManaChange(CurrMana,MaxManaIsThisRound);
    }
    public void IncreaseMana()
    {
        if (MaxManaIsThisRound < MaxGameMana)
        {
            MaxManaIsThisRound++;
            ManaInfromationEvent.current.ManaChange(CurrMana,MaxManaIsThisRound);
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
            ManaInfromationEvent.current.ManaChange(-Cost,MaxManaIsThisRound);
            return true;
        }
        else
        {
            Debug.Log("To Low Mana");
            return false;
        }
    }

    private void Update()
    {
        Debug.Log(CurrMana);
    }
}


