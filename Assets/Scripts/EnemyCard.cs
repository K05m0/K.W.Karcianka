using UnityEngine;

public class EnemyCard : CardObject
{
    public enum CardState { Preparing, Created }
    public CardState currentState = CardState.Preparing;

    void Awake()
    {
        // Zakładamy, że gridManager jest w tej samej scenie
        gridManager = FindObjectOfType<GridManager>();
    }
}
