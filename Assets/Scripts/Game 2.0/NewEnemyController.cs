using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NewEnemyController : MonoBehaviour
{
    public List<PreparePosition> positionToPrepare;

    [SerializeField] private GridController gridManager;
    [SerializeField] private List<WaveConfig> waves;

    public List<Card> PreperedCard = new List<Card>();
    public List<Card> PlacedCard = new List<Card>();

    public int currentWaveIndex = 0;

    private void Start()
    {
        PrepareNextWave(0);
    }

    public void PrepareNextWave(int currentWaveIndex)
    {
        if (currentWaveIndex > waves.Count - 1)
        {
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
        }
        foreach (var card in enemiesToSpawn)
        {
            if (availablePosition.Count > 0) // Sprawdzenie, czy są dostępne pozycje
            {
                int randomIndex = random.Next(availablePosition.Count); // Losowanie indeksu
                int selectedPosition = availablePosition[randomIndex]; // Wybór pozycji
                availablePosition.RemoveAt(randomIndex); // Usunięcie wybranej pozycji z listy
                var cardObj = Instantiate(card, positionToPrepare[selectedPosition].Position);
                cardObj.SetUpCard(gridManager, null, Card.CardType.Enemy);
                positionToPrepare[selectedPosition].CardOnPosition = cardObj;
                card.transform.position = Vector3.zero;
                card.transform.rotation = Quaternion.identity;
            }
            else
            {
                break; // Opcjonalne: przerwanie, gdy nie ma więcej dostępnych pozycji
            }
        }
    }

    public void PlacePreperedCardOnBoard()
    {
        for (int i = 0; i < positionToPrepare.Count; i++)
        {
            if (positionToPrepare[i].CardOnPosition == null)
                continue;
            GridElement gridElement = gridManager.GridCoordinate[i, gridManager.GridHeight - 1];
            positionToPrepare[i].CardOnPosition.PlaceCard(gridElement);
            Debug.Log(gridElement.ElementCoordinate);
        }
    }
}

[Serializable]
public class PreparePosition
{
    public Transform Position;
    public Card CardOnPosition;
}
