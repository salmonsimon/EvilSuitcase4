using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<int> enemiesToSpawn;

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
        int j = 0;

        while (i < enemiesToSpawn.Count)
        {
            while (j < enemiesToSpawn[i])
            {
                Vector3 randomSpawnPosition = GetRandomSpawnPosition().position;

                if (!ObjectPools.ContainsKey(i))
                {
                    ObjectPools.Add(i, new ObjectPool<GameObject>(() => Instantiate(enemyPrefabs[i], poolContainer)));
                }

                GameObject spawnedEnemy = ObjectPools[i].Get();

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

                j++;
            }

            j = 0;
            i++;
        }
    }

    private Transform GetRandomSpawnPosition()
    {
        return spawnPositions[Random.Range(0, spawnPositions.Count)];
    }
}
