public interface IAIEnvState
{
    // Number of currently alive zombies
    // can be safely ignored for now
    // int EnemyCount();
}

public interface IAIEnvActions
{
    // Create an enemy described by genes at a random position
    void SpawnEnemy(Genotype genes);
}