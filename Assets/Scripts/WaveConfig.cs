using System.Collections.Generic;

[System.Serializable]
public class WaveConfig
{
    public string waveName;
    public List<EnemyTypeCount> enemiesToSpawn;
    public int turnToPrepare;
    public bool spawnImmediately;
    public bool SpawnNow;
    public bool IsWaveComplete = false; // Flaga informująca, czy fala jest zakończona

    public WaveConfig(string waveName, List<EnemyTypeCount> enemiesToSpawn, int turnToPrepare, bool spawnImmediately)
    {
        this.waveName = waveName;
        this.enemiesToSpawn = enemiesToSpawn;
        this.turnToPrepare = turnToPrepare;
        this.spawnImmediately = spawnImmediately;
        SpawnNow = spawnImmediately;
    }
}
