using System.Collections.Generic;

[System.Serializable]
public class WaveConfig
{
    public string waveName; // Nazwa fali
    public List<EnemyTypeCount> enemiesToSpawn; // Lista typów przeciwników i ich liczba
    public int turnToPrepare; // W której turze powinna być przygotowana ta fala
    public bool spawnImmediately; // Czy jednostki mają pojawić się od razu w tej turze?
    public bool SpawnNow;

    public WaveConfig(string waveName, List<EnemyTypeCount> enemiesToSpawn, int turnToPrepare, bool spawnImmediately)
    {
        this.waveName = waveName;
        this.enemiesToSpawn = enemiesToSpawn;
        this.turnToPrepare = turnToPrepare;
        this.spawnImmediately = spawnImmediately;
        SpawnNow = spawnImmediately;
    }
}
