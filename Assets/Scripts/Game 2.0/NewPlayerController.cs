using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    [Header("reference")]
    public GridController Controller;

    [Header("Card Draw")]
    public int StartCardAmount = 3;
    public List<Card> AllCardsInDeck = new List<Card>();
    private Stack<Card> PhysicalDeck = new Stack<Card>(); // Stos fizycznych kart

    public Transform DeckPosition;  // Miejsce fizycznej talii
    public float CardStackOffset = 0.02f; // Odstęp między kartami w stosie
    public List<Transform> CardSlots;
    public Card[] CardInHand;

    public float DrawMoveTime = 0.5f;
    public float DrawRotateTime = 1f;

    [Header("Mana")]
    public int MaxGameMana;
    public int MaxManaInTurn;
    public int CurrAmountMana;
    public List<GameObject> ManaCoins;
    public GameObject ManaCoinPrefab;
    public Transform SpawnCoinPlace;
    public Transform CoinParent;
    public TextMeshProUGUI coinText;



    private void Awake()
    {
        CreatePhysicalDeck();
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            DrawRandomCard();
            Debug.Log(AllCardsInDeck.Count);
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            IncreaseMana();
            ResetMana();
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            UseMana(CurrAmountMana / 2);
        }

    }

    public int? GetRandomNonNullIndex()
    {
        // Tworzymy listę indeksów elementów, które nie są null
        var availableIndices = new List<int>();
        for (int i = 0; i < CardInHand.Length; i++)
        {
            if (CardInHand[i] != null)
            {
                availableIndices.Add(i);
            }
        }

        // Jeśli lista jest pusta, wszystkie elementy są null - zwracamy null
        if (availableIndices.Count == 0)
        {
            return null;
        }

        // Losujemy indeks z dostępnych
        int randomIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];
        return randomIndex;
    }

    // create card deck
    private void CreatePhysicalDeck()
    {
        // Tworzymy kopię listy AllCardsInDeck jako stos fizycznych kart
        foreach (Card card in AllCardsInDeck)
        {
            Card newCard = Instantiate(card, DeckPosition.position, Quaternion.identity, DeckPosition);
            newCard.transform.position += new Vector3(0, CardStackOffset * PhysicalDeck.Count, 0); // Ustawienie przesunięcia dla stosu
            newCard.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-10f, 10f)); // Random rotacja dla efektu wizualnego
            PhysicalDeck.Push(newCard);
        }
    }

    public void StartGame()
    {
        CardInHand = new Card[CardSlots.Count];
        StartDraw();
        ResetMana();
    }

    // Draw
    public void StartDraw()
    {
        for (int i = 0; i < StartCardAmount; i++)
        {
            DrawRandomCard();
        }
    }

    public void DrawRandomCard()
    {
        if (PhysicalDeck.Count <= 0)
            return;



        // Znajdujemy pierwsze wolne miejsce w ręce
        for (int i = 0; i < CardSlots.Count; i++)
        {
            if (CardInHand[i] != null)
                continue;

            // Pobieramy kartę z fizycznego stosu
            Card drawnCard = PhysicalDeck.Pop();

            // Ustawiamy docelową pozycję i animację dla wyciągniętej karty
            drawnCard.gameObject.name = drawnCard.GetType().ToString();

            drawnCard.transform.SetParent(CardSlots[i]);

            DG.Tweening.Sequence sequence = DOTween.Sequence();

            // Dodajemy animacje do sekwencji, które będą wykonywane równocześnie
            sequence.Append(drawnCard.transform.DOMove(CardSlots[i].position, DrawMoveTime)
                .SetEase(Ease.OutSine));
            sequence.Join(drawnCard.transform.DORotateQuaternion(CardSlots[i].rotation, DrawRotateTime)
                .SetEase(Ease.OutBack));

            // Dodajemy OnComplete na zakończenie sekwencji
            sequence.OnComplete(() =>
            {
                drawnCard.SetUpCard(Controller,this, Card.CardType.Player, CardSlots[i]);
            });

            CardInHand[i] = drawnCard;
            AllCardsInDeck.Remove(drawnCard); // Usuwamy kartę z listy kart w talii
            return;
        }
    }


    // Mana
    public void ResetMana()
    {
        CurrAmountMana = MaxManaInTurn;
        SpawnManaCoin();
        UpdateText();
    }

    public void IncreaseMana()
    {
        if (MaxManaInTurn < MaxGameMana)
        {
            MaxManaInTurn++;
        }
        else
        {
            Debug.Log("We have Max Mana");
        }
    }

    public bool UseMana(int Cost)
    {
        if (Cost <= CurrAmountMana)
        {
            List<GameObject> objToRemove = new List<GameObject>();
            CurrAmountMana -= Cost;
            for (int i = 0; i < Cost; i++)
            {
                objToRemove.Add(ManaCoins[i]);
            }

            ManaCoins.RemoveAll(item => objToRemove.Contains(item));

            foreach (var item in objToRemove)
            {
                Destroy(item);
            }
            UpdateText();
            return true;
        }
        else
        {
            Debug.Log("Too Low Mana");
            return false;
        }
    }

    public void SpawnManaCoin()
    {
        var coinsToSpawn = CurrAmountMana - ManaCoins.Count;
        for (int i = 0; i < coinsToSpawn; i++)
        {
            var obj = Instantiate(ManaCoinPrefab, SpawnCoinPlace.position, ManaCoinPrefab.transform.rotation, CoinParent);
            ManaCoins.Add(obj);

            // Generujemy małe losowe przesunięcie pozycji
            float offsetX = UnityEngine.Random.Range(-0.005f, 0.005f);
            float offsetY = UnityEngine.Random.Range(-0.005f, 0.005f);
            float offsetZ = UnityEngine.Random.Range(-0.005f, 0.005f);
            obj.transform.position += new Vector3(offsetX, offsetY, offsetZ);

            // Generujemy losową rotację
            float randomX = UnityEngine.Random.Range(-25, 25);
            float randomY = UnityEngine.Random.Range(-25, 25);
            float randomZ = UnityEngine.Random.Range(-25, 25);

            // Ustawiamy animację rotacji za pomocą DoTween
            obj.transform.DORotate(new Vector3(randomX, randomY, randomZ), 0.05f)
                     .SetEase(Ease.InOutQuad);    // Opcjonalne - określa rodzaj interpolacji
        }
    }

    private void UpdateText()
    {
        coinText.text = $"{CurrAmountMana}/{MaxManaInTurn}";
    }

    //RemoveCardFromHand
    public bool UseCard(Card selectedCard)
    {
        if (!CardInHand.Contains(selectedCard))
        {
            Debug.LogError("This card is not in hand");
            return false;
        }

        if (!UseMana(selectedCard.Cost))
        {
            Debug.LogError("This card is to expensive");
            return false;
        }

        int index = Array.IndexOf(CardInHand, selectedCard);
        CardInHand[index] = null;
        return true;
    }
}
