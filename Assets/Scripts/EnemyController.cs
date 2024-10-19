using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private List<WaveConfig> waves; // Lista fal konfigurowana z poziomu inspektora

    private int currentWaveIndex = 0; // Indeks obecnej fali
    private int indexWavetoSpawner = 0;
    private bool spawnInNextTurn = false; // Flaga określająca, czy jednostki mają pojawić się w następnej turze

    public List<Card> PreperedCard = new List<Card>();
    public List<Card> PlacedCard = new List<Card>();

    private void Awake()
    {
        Card enemy1 = new Card("enemy1", 0);
        Card enemy2 = new Card("enemy2", 0);

        EnemyTypeCount type1inWave1 = new EnemyTypeCount(enemy1, 1);
        WaveConfig wave1 = new WaveConfig("wave1", new List<EnemyTypeCount>() { type1inWave1 }, 1, true); // 1 tura na przygotowanie

        EnemyTypeCount type1inWave2 = new EnemyTypeCount(enemy1, 2);
        EnemyTypeCount type2inWave2 = new EnemyTypeCount(enemy2, 1);
        WaveConfig wave2 = new WaveConfig("wave2", new List<EnemyTypeCount>() { type1inWave2, type2inWave2 }, 8, true); // 3 tura na przygotowanie

        waves = new List<WaveConfig> { wave1, wave2 };
    }

    public void PrepareNextWave(int currentTurn)
    {
        if (currentWaveIndex < waves.Count && currentTurn == waves[currentWaveIndex].turnToPrepare)
        {
            WaveConfig currentWave = waves[currentWaveIndex];

            // Dodajemy karty przeciwników z bieżącej fali do listy przygotowanych kart
            foreach (var enemyConfig in currentWave.enemiesToSpawn)
            {
                for (int i = 0; i < enemyConfig.count; i++)
                {
                    Card newIstance = new Card(enemyConfig.enemyType.CardName, enemyConfig.enemyType.CardCost);
                    ModifyPreperedCard(new List<Card> { newIstance }, true);
                }
            }

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
        PreperedCard.Clear(); // Opróżniamy listę przygotowanych kart
        spawnInNextTurn = false; // Resetujemy flagę, bo jednostki zostały już umieszczone
        indexWavetoSpawner++;
    }

    public void DecreaseTurnCounter(int currentTurn)
    {
        if(IndexExists(waves, indexWavetoSpawner))

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
        foreach (Card card in cardToAdd)
        {
            if (isAdd && !PreperedCard.Contains(card))
            {
                PreperedCard.Add(card);
            }
            else if (!isAdd && PreperedCard.Contains(card))
            {
                PreperedCard.Remove(card);
            }
        }
    }

    public void ModifyPlacedCard(List<Card> cardToAdd, bool isAdd)
    {
        foreach (Card card in cardToAdd)
        {
            if (isAdd && !PlacedCard.Contains(card))
            {
                PlacedCard.Add(card);
            }
            else if (!isAdd && PlacedCard.Contains(card))
            {
                PlacedCard.Remove(card);
            }
        }
    }

    private bool IndexExists<T>(List<T> list, int index)
    {
        return index >= 0 && index < list.Count; // Sprawdzanie, czy indeks jest w poprawnym zakresie
    }
}
