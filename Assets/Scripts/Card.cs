[System.Serializable]
public class Card
{
    public string CardName;
    public int CardCost = 2;

    public Card(string cardName , int cardCost)
    {
        CardName = cardName;
        CardCost = cardCost;
    }
}
