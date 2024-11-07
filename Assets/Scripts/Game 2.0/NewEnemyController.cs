using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NewEnemyController : MonoBehaviour
{
    public List<PreparePosition> positionToPrepare;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private List<WaveConfig> waves;

    public List<Card> PreperedCard = new List<Card>();
    public List<Card> PlacedCard = new List<Card>();

    public int currentWaveIndex = 0;

    private void Awake()
    {
        SpawnPreperedCard(waves[0].enemiesToSpawn);
    }

    public void PrepareNextWave(int currentWaveIndex)
    {
        if (currentWaveIndex > waves.Count - 1)
        {
            Debug.LogError($"currente wave is not implement, wave index: {currentWaveIndex}");
            return;
        }

        if (currentWaveIndex >= waves[currentWaveIndex].turnToPrepare)
        {
            WaveConfig currentWave = waves[currentWaveIndex];

            SpawnPreperedCard(currentWave.enemiesToSpawn);

            if (waves[currentWaveIndex].spawnImmediately)
            {
                PlacePreperedCardOnBoard();
            }
        }
    }

    private void SpawnPreperedCard(List<Card> enemiesToSpawn)
    {
        System.Random random = new System.Random();
        List<int> availablePosition = new List<int>();
        for (int i = 0; i <= positionToPrepare.Count - 1; i++)
        {
            availablePosition.Add(i);
            Debug.Log($"avaible position {availablePosition[i]}");
        }
        foreach (var card in enemiesToSpawn)
        {
            if (availablePosition.Count > 0) // Sprawdzenie, czy są dostępne pozycje
            {
                int randomIndex = random.Next(availablePosition.Count); // Losowanie indeksu
                int selectedPosition = availablePosition[randomIndex]; // Wybór pozycji
                availablePosition.RemoveAt(randomIndex); // Usunięcie wybranej pozycji z listy

                positionToPrepare[selectedPosition].CardOnPosition = card;
                Instantiate(card,positionToPrepare[selectedPosition].Position);
                card.transform.position = Vector3.zero;
                card.transform.rotation = Quaternion.identity;
            }
            else
            {
                Debug.Log("No available positions left.");
                break; // Opcjonalne: przerwanie, gdy nie ma więcej dostępnych pozycji
            }
        }
    }

    public void PlacePreperedCardOnBoard()
    {

    }
}

[Serializable]
public class PreparePosition
{
    public Transform Position;
    public Card CardOnPosition;
}
