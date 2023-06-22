using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private float maxDelayUntilNextSpawn = 4f;

    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private int defaultSpawnCount = 100;

    private void Awake()
    {
        SpawnZombiesOnAwake();
    }

    private void SpawnZombiesOnAwake()
    {
        Dictionary<string, ObjectPool<GameObject>> objectPools = GameManager.instance.GetWaveManager().SpawnedObjectPoolsDictionary;
        Transform poolContainer = GameManager.instance.GetWaveManager().PoolContainer;

        foreach (GameObject enemy in enemyPrefabs)
        {
            for (int i = 0; i < defaultSpawnCount; i++)
            {
                if (!objectPools.ContainsKey(enemy.name))
                {
                    objectPools.Add(enemy.name, new ObjectPool<GameObject>(() => Instantiate(enemy, poolContainer)));
                }

                GameObject spawnedEnemy = objectPools[enemy.name].Get();

                spawnedEnemy.GetComponent<HealthManager>().OnDeath += GameManager.instance.GetWaveManager().OnEnemyDeath;

                spawnedEnemy.AddComponent<PoolableObject>();
                spawnedEnemy.GetComponent<PoolableObject>().ObjectPool = objectPools[enemy.name];

                spawnedEnemy.SetActive(false);
            }
        }

        EnemiesInitializationPoolRelease();
    }

    public void SpawnEnemies(WaveManager.WaveSpawnStruct wave)
    {
        List<GameObject> enemyPrefabs = new List<GameObject>();
        List<float> enemySpawnProbabilities = new List<float>();
        
        foreach (WaveManager.SpawnObjectStruct spawnObject in wave.SpawnObjects)
        {
            enemyPrefabs.Add(spawnObject.Prefab);
            enemySpawnProbabilities.Add(spawnObject.SpawnProbability);
        }

        StartCoroutine(SpawnEnemiesCoroutine(wave.EnemiesToSpawn, enemyPrefabs, enemySpawnProbabilities));
    }

    private IEnumerator SpawnEnemiesCoroutine(int enemiesToSpawn, List<GameObject> enemyPrefabs, List<float> enemySpawnProbabilities)
    {
        int i = 0;

        Dictionary<string, ObjectPool<GameObject>> objectPools = GameManager.instance.GetWaveManager().SpawnedObjectPoolsDictionary;
        Transform poolContainer = GameManager.instance.GetWaveManager().PoolContainer;

        while (i < enemiesToSpawn)
        {
            float randomProbability = Random.Range(0f, 1f);

            NormalizeSpawnProbabilities(enemySpawnProbabilities);
            int enemyIndexToSpawn = GetEnemyIndexToSpawn(randomProbability, enemySpawnProbabilities);

            Vector3 randomSpawnPosition = GetRandomSpawnPosition().position;

            string enemyToSpawnName = enemyPrefabs[enemyIndexToSpawn].name;

            if (!objectPools.ContainsKey(enemyToSpawnName))
            {
                objectPools.Add(enemyToSpawnName, new ObjectPool<GameObject>(() => Instantiate(enemyPrefabs[enemyIndexToSpawn], poolContainer)));
            }

            GameObject spawnedEnemy = objectPools[enemyToSpawnName].Get();

            if (!spawnedEnemy.activeSelf)
            {
                spawnedEnemy.SetActive(true);
            }
            else
            {
                spawnedEnemy.GetComponent<HealthManager>().OnDeath += GameManager.instance.GetWaveManager().OnEnemyDeath;

                spawnedEnemy.AddComponent<PoolableObject>();
                spawnedEnemy.GetComponent<PoolableObject>().ObjectPool = objectPools[enemyToSpawnName];
            }

            NavMeshHit Hit;
            if (NavMesh.SamplePosition(randomSpawnPosition, out Hit, 5f, -1))
            {
                spawnedEnemy.GetComponent<ZombieStateMachine>().Agent.Warp(Hit.position);
                spawnedEnemy.GetComponent<ZombieStateMachine>().Agent.enabled = true;
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {randomSpawnPosition}");
            }


            yield return new WaitForSeconds(Random.Range(1f, maxDelayUntilNextSpawn));

            i++;
        }
    }

    private Transform GetRandomSpawnPosition()
    {
        return spawnPositions[Random.Range(0, spawnPositions.Count)];
    }

    private int GetEnemyIndexToSpawn(float randomProbability, List<float> enemySpawnProbabilities)
    {
        int indexToSpawn = 0;

        for (int i = 0; i < enemySpawnProbabilities.Count; i++)
        {
            if (randomProbability < enemySpawnProbabilities[i])
            {
                indexToSpawn = i;
                return indexToSpawn;
            }

            randomProbability -= enemySpawnProbabilities[i];
        }

        return indexToSpawn;
    }

    private void NormalizeSpawnProbabilities(List<float> enemySpawnProbabilities)
    {
        float totalProbability = enemySpawnProbabilities.Sum();

        for (int i = 0; i < enemySpawnProbabilities.Count; i++)
        {
            enemySpawnProbabilities[i] /= totalProbability;
        }
    }

    private void EnemiesInitializationPoolRelease()
    {
        Transform poolContainer = GameManager.instance.GetWaveManager().PoolContainer;

        foreach (Transform enemy in poolContainer)
        {
            ObjectPool<GameObject> pool = enemy.GetComponent<PoolableObject>().ObjectPool;
            pool.Release(enemy.gameObject);
        }
    }
}
