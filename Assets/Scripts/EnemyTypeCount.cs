[System.Serializable]
public class EnemyTypeCount
{
    public Card enemyType; // Typ przeciwnika (referencja do prefabrykatów przeciwników)
    public int count; // Liczba jednostek tego typu w fali

    public EnemyTypeCount(Card enemyType, int count)
    {
        this.enemyType = enemyType;
        this.count = count;
    }
}
