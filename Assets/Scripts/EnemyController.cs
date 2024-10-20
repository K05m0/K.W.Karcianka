using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<PreparePosition> positionToPrepare;

    [SerializeField] private List<WaveConfig> waves; // Lista fal konfigurowana z poziomu inspektora

    private int currentWaveIndex = 0; // Indeks obecnej fali
    private int indexWavetoSpawner = 0;
    private bool spawnInNextTurn = false; // Flaga określająca, czy jednostki mają pojawić się w następnej turze

    public List<Card> PreperedCard = new List<Card>();
    public List<EnemyCard> PlacedCard = new List<EnemyCard>();

    private GridManager gridManager;

    private void OnEnable()
    {
        Card.OnCardDeath += HandleCardDeath;
    }

    private void OnDisable()
    {
        Card.OnCardDeath -= HandleCardDeath;
    }

    private void HandleCardDeath(object sender, CardDeathEventArgs e)
    {
        // Obsługa zdarzenia - np. usunięcie karty z listy PlacedCard
        ModifyPlacedCard(new List<Card>() { e.DeadCard }, false);
        Debug.Log("Karta zmarła: " + e.DeadCard);
    }

    private void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();
    }
    //Debug
    /*    private void Awake()
        {
            Card enemy1 = new Card("enemy1", 0);
            Card enemy2 = new Card("enemy2", 0);

            EnemyTypeCount type1inWave1 = new EnemyTypeCount(enemy1, 1);
            WaveConfig wave1 = new WaveConfig("wave1", new List<EnemyTypeCount>() { type1inWave1 }, 1, false); // 1 tura na przygotowanie

            EnemyTypeCount type1inWave2 = new EnemyTypeCount(enemy1, 2);
            EnemyTypeCount type2inWave2 = new EnemyTypeCount(enemy2, 1);
            WaveConfig wave2 = new WaveConfig("wave2", new List<EnemyTypeCount>() { type1inWave2, type2inWave2 }, 3, false); // 3 tura na przygotowanie

            waves = new List<WaveConfig> { wave1, wave2 };
        }*/

    public void PrepareNextWave(int currentTurn)
    {
        if (currentWaveIndex < waves.Count && currentTurn == waves[currentWaveIndex].turnToPrepare)
        {
            WaveConfig currentWave = waves[currentWaveIndex];
            List<Card> cards = new List<Card>();

            // Dodajemy karty przeciwników z bieżącej fali do listy przygotowanych kart
            ModifyPreperedCard(currentWave.enemiesToSpawn, true);


            // Jeśli spawnImmediately jest true, jednostki od razu wchodzą na planszę
            if (currentWave.spawnImmediately)
            {
                SpawnPreparedCards();
            }
        }
    }

    public void SpawnPreparedCards()
    {
        ModifyPlacedCard(PreperedCard, true); // Przenosimy przygotowane karty na planszę
        ModifyPreperedCard(PreperedCard, false);// spawnInNextTurn = false; // Resetujemy flagę, bo jednostki zostały już umieszczone
        indexWavetoSpawner++;
    }

    public void DecreaseTurnCounter(int currentTurn)
    {
        if (IndexExists(waves, indexWavetoSpawner))

            // Sprawdzamy, czy powinniśmy spawnować jednostki w tej turze
            if (waves[indexWavetoSpawner].SpawnNow)
            {
                if (!waves[indexWavetoSpawner].spawnImmediately)
                    SpawnPreparedCards();
            }
            else
            {
                waves[indexWavetoSpawner].SpawnNow = true;
            }

        // Sprawdzamy, czy powinniśmy przejść do następnej fali
        if (currentWaveIndex < waves.Count - 1)
        {
            currentWaveIndex++;
        }
    }

    public void ModifyPreperedCard(List<Card> cardToAdd, bool isAdd)
    {
        // Jeśli dodajemy karty
        if (isAdd)
        {
            foreach (Card card in cardToAdd)
            {
                if (!PreperedCard.Contains(card))
                {
                    // Znajdź wszystkie dostępne pozycje, które nie są jeszcze zajęte
                    var availablePositions = positionToPrepare
                                              .Where(pos => pos.selectedCard == null)
                                              .ToList();

                    if (availablePositions.Count == 0)
                    {
                        Debug.LogWarning("Brak dostępnych pozycji do umieszczenia karty.");
                        return; // Wyjście, jeśli nie ma dostępnych miejsc
                    }

                    // Wybieramy losową dostępną pozycję
                    var randomPos = UnityEngine.Random.Range(0, availablePositions.Count);
                    var selectedPosition = availablePositions[randomPos];

                    // Dodajemy kartę do listy przygotowanych i instancjujemy ją na wybranej pozycji

                    Card cardIstance = Instantiate(card, selectedPosition.spawnPosition);
                    EnemyCard cardObject = cardIstance.AddComponent<EnemyCard>();
                    cardIstance.SetUpCard(cardObject);
                    cardObject.SetUpCard(cardIstance);
                    selectedPosition.selectedCard = cardObject;

                    cardIstance.OnSpawn();

                    PreperedCard.Add(cardIstance);

                }
            }
        }
        // Jeśli usuwamy karty
        else
        {
            List<Card> cardsToRemove = new List<Card>();

            foreach (Card card in cardToAdd)
            {
                if (PreperedCard.Contains(card))
                {
                    foreach (var pos in positionToPrepare)
                    {
                        if (pos?.selectedCard?.CardData == card)
                        {
                            Destroy(pos.selectedCard.gameObject); // Usuwamy obiekt karty
                            pos.selectedCard = null; // Zwalniamy pozycję
                            cardsToRemove.Add(card);
                            break;
                        }
                    }
                }
            }

            // Usuwamy karty po zakończeniu iteracji
            foreach (Card card in cardsToRemove)
            {
                PreperedCard.Remove(card);
            }
        }
    }


    public void ModifyPlacedCard(List<Card> cardToAdd, bool isAdd)
    {
        foreach (var card in cardToAdd)
        {
            // Dodawanie kart
            if (isAdd && !PlacedCard.Any(x => x.CardData == card))
            {
                Card cardIstance = Instantiate(card);
                EnemyCard cardObject = cardIstance.AddComponent<EnemyCard>();
                cardIstance.SetUpCard(cardObject);
                cardObject.SetUpCard(cardIstance);

                // Znalezienie indeksu pozycji, na którą karta ma być ustawiona
                var index = positionToPrepare
                    .Where(x => x.selectedCard != null && x.selectedCard.CardData == card)
                    .Select(x => positionToPrepare.IndexOf(x))
                    .FirstOrDefault();

                cardObject.PlaceOnGrid(index, 4);

                cardIstance.transform.SetParent(gridManager.GetCell(index, 4).transform);
                cardIstance.transform.localScale = Vector3.one;
                cardIstance.transform.rotation = Quaternion.identity;

                PlacedCard.Add(cardObject);
            }
            // Usuwanie kart
            else if (!isAdd && PlacedCard.Any(x => x.CardData == card))
            {
                // Lista kart do usunięcia
                List<EnemyCard> cardsToRemove = new List<EnemyCard>();

                foreach (var placedCard in PlacedCard.Where(x => x.CardData == card))
                {
                    // Usunięcie obiektu karty z gry (zniszczenie obiektu)
                    Destroy(placedCard.gameObject);
                    cardsToRemove.Add(placedCard); // Zbieramy karty do usunięcia
                }

                // Usunięcie kart z listy PlacedCard
                foreach (var cardToRemove in cardsToRemove)
                {
                    PlacedCard.Remove(cardToRemove);
                }
            }
        }

    }

    private bool IndexExists<T>(List<T> list, int index)
    {
        return index >= 0 && index < list.Count; // Sprawdzanie, czy indeks jest w poprawnym zakresie
    }
}

[System.Serializable]
public class PreparePosition
{
    public Transform spawnPosition;
    public EnemyCard selectedCard;
}