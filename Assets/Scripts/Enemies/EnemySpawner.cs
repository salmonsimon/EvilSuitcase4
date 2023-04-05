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
    [SerializeField] private float maxDelayUntilNextSpawn = 2f;

    #region TO DO: CHANGE INTO WAVE MANAGER

    private Dictionary<int, ObjectPool<GameObject>> ObjectPools = new();
    private Transform poolContainer;

    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<float> enemySpawnProbabilities;

    // we will pass into this one only the pool dictionary and the numer of enemies to spawn
    // the pools themselves will be stored on the wave manager script

    #endregion


    private void Awake()
    {
        poolContainer = new GameObject("Pool Container").transform;
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        int i = 0;

        while (i < enemiesToSpawn)
        {
            float randomProbability = Random.Range(0f, 1f);

            NormalizeSpawnProbabilities(enemySpawnProbabilities);
            int enemyIndexToSpawn = GetEnemyIndexToSpawn(randomProbability, enemySpawnProbabilities);

            Vector3 randomSpawnPosition = GetRandomSpawnPosition().position;

            if (!ObjectPools.ContainsKey(enemyIndexToSpawn))
            {
                ObjectPools.Add(enemyIndexToSpawn, new ObjectPool<GameObject>(() => Instantiate(enemyPrefabs[enemyIndexToSpawn], poolContainer)));
            }

            GameObject spawnedEnemy = ObjectPools[enemyIndexToSpawn].Get();

            NavMeshHit Hit;
            if (NavMesh.SamplePosition(randomSpawnPosition, out Hit, 2f, -1))
            {

                spawnedEnemy.GetComponent<ZombieStateMachine>().Agent.Warp(Hit.position);
                spawnedEnemy.GetComponent<ZombieStateMachine>().Agent.enabled = true;
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {randomSpawnPosition}");
            }

            yield return new WaitForSeconds(Random.Range(0f, maxDelayUntilNextSpawn));

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
}
