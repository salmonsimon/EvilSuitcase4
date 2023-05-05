using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class WaveManager : MonoBehaviour
{
    #region Structs

    [System.Serializable]
    public struct SpawnObjectStruct
    {
        public GameObject Prefab;
        public float SpawnProbability;
    }

    [System.Serializable]
    public struct WaveSpawnStruct
    {
        public int EnemiesToSpawn;
        public List<SpawnObjectStruct> SpawnObjects;
    }

    #endregion

    #region Parameters

    [SerializeField] List<WaveSpawnStruct> waves;

    #endregion

    #region Variables

    private Dictionary<string, ObjectPool<GameObject>> spawnedObjectPoolsDictionary = new();
    public Dictionary<string, ObjectPool<GameObject>> SpawnedObjectPoolsDictionary { get { return spawnedObjectPoolsDictionary; } }

    private int currentWave = 0;
    private int currentKilledEnemies = 0;
    private int currentEnemiesToKill = 0;

    #endregion

    #region Object References

    private Transform poolContainer;
    public Transform PoolContainer { get { return poolContainer; } }

    #endregion

    private void Awake()
    {
        poolContainer = new GameObject("Pool Container").transform;
    }

    private void Start()
    {
        StartWave();
    }

    public void StartWave()
    {
        WaveSpawnStruct wave = waves[currentWave];

        currentKilledEnemies = 0;
        currentEnemiesToKill = wave.EnemiesToSpawn;

        GameManager.instance.GetEnemySpawner().SpawnEnemies(wave);

        currentWave++;
    }

    public void OnEnemyDeath()
    {
        currentKilledEnemies++;

        if (currentKilledEnemies == currentEnemiesToKill)
            WaveCleared();
    }

    private void WaveCleared()
    {
        // Here open the rewards inventory and all rewards obtained
    }

    private void CorpseCleanup()
    {
        foreach (Transform corpse in PoolContainer)
        {
            ObjectPool<GameObject> pool = corpse.GetComponent<PoolableObject>().ObjectPool;

            corpse.gameObject.SetActive(false);

            pool.Release(corpse.gameObject);
        }

    }
}
