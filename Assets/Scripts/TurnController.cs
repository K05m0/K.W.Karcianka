using System;
using System.Collections;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public EnemyController enemyController; // Referencja do kontrolera przeciwników
    public PlayerManager playerManager;
    private int turnNumber = 1; // Numer aktualnej tury
    private enum TurnPhase { StartTurn, EnemyPrepare, EnemySpawn, EnemyAction, PlayerTurn, TurnEnd, PlayerAction }
    private TurnPhase currentPhase = TurnPhase.StartTurn;

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


                case TurnPhase.StartTurn:
                    Debug.Log($"Turn {turnNumber}: start phase");
                    currentPhase = TurnPhase.EnemyAction;
                    if (turnNumber == 1)
                    {
                        playerManager.StartGame();
                        break;
                    }
                    playerManager.DrawRandomCard();
                    playerManager.IncreaseMana();
                    playerManager.ResetMana();
                    break;

                case TurnPhase.EnemyAction:
                    Debug.Log($"Turn {turnNumber}: EnemyAction phase");
                    for (int i = enemyController.PlacedCard.Count - 1; i >= 0; i--)
                    {
                        yield return new WaitForSeconds(0.5f);
                        enemyController.PlacedCard[i].CardData.MakeCardTurn();
                    }

                    currentPhase = TurnPhase.EnemyPrepare;
                    break;

                case TurnPhase.EnemyPrepare:
                    Debug.Log($"Turn {turnNumber}: EnemyPrepare phase");
                    if (enemyController != null)
                        enemyController.PrepareNextWave(turnNumber); // Przygotowanie kolejnej fali
                    currentPhase = TurnPhase.EnemySpawn;
                    break;

                case TurnPhase.EnemySpawn:
                    Debug.Log($"Turn {turnNumber}: EnemySpawn phase");
                    if (enemyController != null)
                        enemyController.DecreaseTurnCounter(turnNumber); // Sprawdzamy, czy jednostki mają być spawnowane
                    currentPhase = TurnPhase.PlayerAction;
                    break;

                case TurnPhase.PlayerAction:
                    Debug.Log($"Turn {turnNumber}: PlayerTurn phase");
                    for (int i = playerManager.PlacedCard.Count - 1; i >= 0; i--)
                    {
                        yield return new WaitForSeconds(0.5f);
                        playerManager.PlacedCard[i].MakeCardTurn();
                    }

                    currentPhase = TurnPhase.PlayerTurn;
                    break;

                case TurnPhase.PlayerTurn:
                    Debug.Log($"Turn {turnNumber}: PlayerTurn phase");

                    yield return new WaitUntil(() => PlayerFinishedTurn());
                    currentPhase = TurnPhase.TurnEnd;
                    break;

                case TurnPhase.TurnEnd:
                    Debug.Log($"Turn {turnNumber}: TurnEnd phase");
                    turnNumber++;
                    currentPhase = TurnPhase.StartTurn;
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
public class CardDeathEventArgs : EventArgs
{
    public Card DeadCard { get; private set; }

    public CardDeathEventArgs(Card deadCard)
    {
        DeadCard = deadCard;
    }
}
