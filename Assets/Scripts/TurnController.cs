using System.Collections;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public EnemyController enemyController; // Referencja do kontrolera przeciwników
    private int turnNumber = 1; // Numer aktualnej tury
    private enum TurnPhase { EnemyPrepare, EnemySpawn, EnemyAction, PlayerTurn, TurnEnd }
    private TurnPhase currentPhase = TurnPhase.EnemyPrepare;

    private void Start()
    {
        StartCoroutine(ExecuteTurn());
    }

    private IEnumerator ExecuteTurn()
    {
        while (true) // Główna pętla tur
        {
            switch (currentPhase)
            {
                case TurnPhase.EnemyPrepare:
                    Debug.Log($"Turn {turnNumber}: EnemyPrepare phase");
                    enemyController.PrepareNextWave(turnNumber); // Przygotowanie kolejnej fali
                    currentPhase = TurnPhase.EnemySpawn;
                    break;

                case TurnPhase.EnemySpawn:
                    Debug.Log($"Turn {turnNumber}: EnemySpawn phase");
                    enemyController.DecreaseTurnCounter(turnNumber); // Sprawdzamy, czy jednostki mają być spawnowane
                    currentPhase = TurnPhase.EnemyAction;
                    break;

                case TurnPhase.EnemyAction:
                    Debug.Log($"Turn {turnNumber}: EnemyAction phase");
                    // Tu możesz dodać ruchy i akcje przeciwników
                    yield return new WaitForSeconds(2f); // Przykładowy delay na wykonanie akcji
                    currentPhase = TurnPhase.PlayerTurn;
                    break;

                case TurnPhase.PlayerTurn:
                    Debug.Log($"Turn {turnNumber}: PlayerTurn phase");
                    // Czekamy na ruch gracza (np. ustawienie jednostek)
                    yield return new WaitUntil(() => PlayerFinishedTurn());
                    currentPhase = TurnPhase.TurnEnd;
                    break;

                case TurnPhase.TurnEnd:
                    Debug.Log($"Turn {turnNumber}: TurnEnd phase");
                    turnNumber++;
                    currentPhase = TurnPhase.EnemyPrepare;
                    break;
            }

            yield return null; // Odczekanie na koniec fazy
        }
    }

    // Metoda, która może być wywołana, gdy gracz skończy swoją turę
    private bool PlayerFinishedTurn()
    {
        // Można tutaj dodać logikę końca tury gracza, np. naciśnięcie przycisku
        return Input.GetKeyDown(KeyCode.Space); // Przykładowo naciśnięcie spacji kończy turę gracza
    }
}
