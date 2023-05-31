using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class WaveManager : MonoBehaviour
{
    #region Structs

    [System.Serializable]
    public struct SpawnObjectStruct
    {
        public GameObject Prefab;

        [Range(0f, 1f)]
        public float SpawnProbability;
    }

    [System.Serializable]
    public struct WaveSpawnStruct
    {
        public int EnemiesToSpawn;
        public List<SpawnObjectStruct> SpawnObjects;
    }

    [System.Serializable]
    public struct WaveRewardsStruct
    {
        public List<RewardItem> RewardItems;
    }

    #endregion

    #region Parameters

    [SerializeField] List<WaveSpawnStruct> waves;
    [SerializeField] List<WaveRewardsStruct> rewards;
    [SerializeField] List<float> rewardsCountdown;
    [SerializeField] List<int> itemsToBlock;

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

    [SerializeField] private GameObject nextWaveCountdownPanel;
    [SerializeField] private GameObject waveClearedPanel;

    #endregion

    private void Awake()
    {
        poolContainer = new GameObject("Pool Container").transform;
    }

    private void Start()
    {
        NextWave();
    }

    public void NextWave()
    {
        StartCoroutine(NextWaveCoroutine());
    }

    private IEnumerator NextWaveCoroutine()
    {
        nextWaveCountdownPanel.SetActive(true);

        TextMeshProUGUI countdownText = nextWaveCountdownPanel.transform.GetComponentInChildren<TextMeshProUGUI>();

        for (int i = 3; i >= 0; i--)
        {
            if (i == 0)
            {
                countdownText.text = "Start!";
                // TO DO: ADD START SFX
            }
            else
            {
                countdownText.text = i + "...";
                // TO DO: ADD COUNTER SFX
            }

            yield return new WaitForSeconds(1f);
        }

        nextWaveCountdownPanel.SetActive(false);

        StartWave();
    }

    public void StartWave()
    {
        WaveSpawnStruct wave = waves[currentWave];

        currentKilledEnemies = 0;
        currentEnemiesToKill = wave.EnemiesToSpawn;

        GameManager.instance.GetEnemySpawner().SpawnEnemies(wave);
    }

    public void OnEnemyDeath()
    {
        currentKilledEnemies++;

        if (currentKilledEnemies == currentEnemiesToKill)
            WaveCleared();
    }

    private void WaveCleared()
    {
        StartCoroutine(WaveClearedCoroutine());
    }

    private IEnumerator WaveClearedCoroutine()
    {
        waveClearedPanel.SetActive(true);
        TextMeshProUGUI countdownText = waveClearedPanel.transform.GetComponentInChildren<TextMeshProUGUI>();

        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i + "...";

            // TO DO: ADD COUNTER SFX

            yield return new WaitForSeconds(1f);
        }

        List<Item> rewardItems = DrawRewardItems();

        float transitionTime = GameManager.instance.GetTransitionManager().RunTransition("DoubleWipe");

        yield return new WaitForSeconds(transitionTime);

        waveClearedPanel.SetActive(false);

        GameManager.instance.GetRewardsUI().OpenRewardsUI(rewardItems, rewardsCountdown[currentWave]);

        GameManager.instance.GetTransitionManager().FinishCurrentTransition();

        yield return new WaitForSeconds(transitionTime);
    }

    public void FinishWave()
    {
        GameManager.instance.GetRewardsUI().CloseRewardsUI();

        StartCoroutine(FinishWaveCoroutine());
    }

    private IEnumerator FinishWaveCoroutine()
    {
        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);

        float transitionTime = GameManager.instance.GetTransitionManager().RunTransition("DoubleWipe");

        yield return new WaitForSeconds(transitionTime);

        GameManager.instance.GetInventoryManager().UnblockBlockedItems();

        if (currentWave > 0 && currentWave % 10 == 0)
            CorpseCleanup();

        GameManager.instance.GetTransitionManager().FinishCurrentTransition();

        yield return new WaitForSeconds(transitionTime);

        bool blockedItems = false;

        if (itemsToBlock[currentWave] > 0)
        {
            GameManager.instance.GetInventoryUI().OpenPauseInventory();

            GameManager.instance.GetCinemachineShake().ShakeCameras(Config.CAMERASHAKE_EXPLOSION_AMPLITUDE, Config.CAMERASHAKE_EXPLOSION_DURATION);
            // TO DO: ADD SHAKE SFX

            yield return new WaitForSeconds(Config.CAMERASHAKE_EXPLOSION_DURATION * 2);

            transitionTime = GameManager.instance.GetTransitionManager().RunTransition("PaintSplash");
            // TO DO: ADD EVIL LAUGH SFX

            yield return new WaitForSeconds(transitionTime);

            blockedItems = GameManager.instance.GetInventoryManager().BlockRandomItems(itemsToBlock[currentWave]);

            GameManager.instance.GetTransitionManager().FinishCurrentTransition();

            yield return new WaitForSeconds(transitionTime);
        }

        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(true);

        if (blockedItems)
            GameManager.instance.GetInventoryUI().PauseGame();

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        currentWave++;
        
        yield return null;

        NextWave();
    }

    private List<Item> DrawRewardItems()
    {
        List<Item> rewardItems = new List<Item>();

        foreach (RewardItem rewardItem in rewards[currentWave].RewardItems)
        {
            for (int itemAmount = 0; itemAmount < rewardItem.Amount; itemAmount++)
            {
                if (Random.Range(0f, 1f) < rewardItem.Probability)
                {
                    Item drewRewardItem = Instantiate(rewardItem.Item);

                    drewRewardItem.RewardItemSetup(rewardItem);

                    rewardItems.Add(drewRewardItem);
                }
            }
        }

        return rewardItems;
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
