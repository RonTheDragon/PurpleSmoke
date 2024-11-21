using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Spawn> _spawns = new List<Spawn>();
    private EnemyPooler _enemyPooler;

    private void Start()
    {
        // Get the EnemyPooler instance from the GameManager
        _enemyPooler = GameManager.Instance.GetEnemyPooler;

        // Start spawning for each spawn configuration
        foreach (Spawn spawn in _spawns)
        {
            StartCoroutine(SpawnEnemies(spawn));
        }
    }

    private System.Collections.IEnumerator SpawnEnemies(Spawn spawn)
    {
        // Initialize EnemiesLeftToSpawn if LimitedAmount is true
        if (spawn.LimitedAmount && spawn.EnemiesLeftToSpawn == 0)
        {
            spawn.EnemiesLeftToSpawn = Random.Range(spawn.MinEnemies, spawn.MaxEnemies + 1);
        }

        foreach (EnemyHealth e in spawn.SpawnedEnemies)
        {
            e.OnDeath += () => OnEnemyDeath(spawn, e);
        }

        while (true)
        {
            // Ensure we don't exceed the max number of enemies alive
            if (spawn.SpawnedEnemies.Count < spawn.MaxAmountAlive && (spawn.LimitedAmount && spawn.EnemiesLeftToSpawn > 0 || !spawn.LimitedAmount))
            {
                // Pick a random location to spawn from the list of spawn locations
                Transform spawnLocation = spawn.SpawnLocations[Random.Range(0, spawn.SpawnLocations.Count)];

                // Spawn the enemy and subscribe to its OnDeath event
                SpawnEnemyAtLocation(spawn, spawnLocation);

                // Now attempt to spawn extra enemies if possible
                SpawnExtraEnemies(spawn, spawnLocation);

                // If LimitedAmount is true, reduce EnemiesLeftToSpawn after spawning
                if (spawn.LimitedAmount)
                {
                    spawn.EnemiesLeftToSpawn--;
                }
            }

            // Check if the spawn should turn off (if LimitedAmount and EnemiesLeftToSpawn reaches 0)
            if (spawn.LimitedAmount && spawn.EnemiesLeftToSpawn <= 0)
            {
                yield break; // Stop spawning when the limit is reached
            }

            // Wait for the next spawn cycle based on the cooldown time
            float cooldownTime = Random.Range(spawn.MinCooldown, spawn.MaxCooldown);
            yield return new WaitForSeconds(cooldownTime);
        }
    }

    private void SpawnEnemyAtLocation(Spawn spawn, Transform spawnLocation)
    {
        // Get an enemy from the pool and place it at the spawn location
        EnemyHealth enemy = _enemyPooler.CreateOrSpawnFromPool(spawn.TagToSpawn, spawnLocation.position, Quaternion.identity);
        if (enemy != null)
        {
            enemy.Spawn();
            // Add the enemy to the list of currently spawned enemies
            spawn.SpawnedEnemies.Add(enemy);

            // Subscribe to the OnDeath event to clean up the enemy when it dies
            enemy.OnDeath += () => OnEnemyDeath(spawn, enemy);
        }
    }

    private void OnEnemyDeath(Spawn spawn, EnemyHealth enemy)
    {
        // Remove the enemy from the spawned list when it dies
        spawn.SpawnedEnemies.Remove(enemy);
    }

    private void SpawnExtraEnemies(Spawn spawn, Transform spawnLocation)
    {
        int maxExtraToSpawn = spawn.MaxAmountAlive - spawn.SpawnedEnemies.Count;
        int extraEnemiesToSpawn = Random.Range(spawn.MinExtraEnemiesSpawnAtOnce, spawn.MaxExtraEnemiesSpawnAtOnce + 1);
        extraEnemiesToSpawn = Mathf.Min(extraEnemiesToSpawn, maxExtraToSpawn);

        for (int i = 0; i < extraEnemiesToSpawn; i++)
        {
            spawnLocation = spawn.SpawnLocations[Random.Range(0, spawn.SpawnLocations.Count)];
            SpawnEnemyAtLocation(spawn, spawnLocation);
        }
    }

    public void TurnOff(string tag)
    {
        // Find the spawn configuration with the matching tag and stop it
        foreach (var spawn in _spawns)
        {
            if (spawn.ToggleTag == tag)
            {
                StopCoroutine(SpawnEnemies(spawn)); // Stop the spawn coroutine for this spawn
            }
        }
    }

    public void TurnOn(string tag)
    {
        // Find the spawn configuration with the matching tag and restart it
        foreach (var spawn in _spawns)
        {
            if (spawn.ToggleTag == tag)
            {
                StartCoroutine(SpawnEnemies(spawn)); // Restart the spawn coroutine for this spawn
            }
        }
    }

    [System.Serializable]
    protected class Spawn
    {
        public string TagToSpawn;         // The tag to identify which enemy to spawn
        public int MaxAmountAlive;        // The maximum number of enemies allowed to be alive at once
        public float MinCooldown, MaxCooldown; // Cooldown between spawns
        public int MinExtraEnemiesSpawnAtOnce, MaxExtraEnemiesSpawnAtOnce;
        public List<Transform> SpawnLocations; // Locations to spawn enemies at
        public List<EnemyHealth> SpawnedEnemies; // List to track the spawned enemies
        public bool LimitedAmount;
        public int MinEnemies, MaxEnemies, EnemiesLeftToSpawn;
        public string ToggleTag;
        public bool StartsOff;
    }
}
