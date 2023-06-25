using StarterAssets;
using System.Collections;
using System.Collections.Generic;
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

    private int currentKilledEnemies = 0;
    private int currentEnemiesToKill = 0;

    private bool initialized = false;

    #region Statistics Variables / Properties

    private int currentWave = 0;
    public int CurrentWave {  
        get { return currentWave; }
        private set
        {
            if (currentWave == value) return;

            currentWave = value;

            if (OnWaveValueChange != null)
                OnWaveValueChange();
        }
    }

    private int totalEnemiesKilled = 0;
    public int TotalEnemiesKilled { 
        get { return totalEnemiesKilled; } 
        private set
        {
            if (totalEnemiesKilled == value) return;

            totalEnemiesKilled = value;

            if (OnEnemyKilled != null)
                OnEnemyKilled();
        }
    
    }

    private int hitsReceived;
    public int HitsReceived { 
        get { return hitsReceived; }
        private set { 
            if (hitsReceived == value) return;

            hitsReceived = value;

            if (OnHitReceived != null)
                OnHitReceived();
        }
    }

    private float timeAlive = 0f;
    public float TimeAlive { 
        get { return timeAlive; }
        private set
        {
            if (timeAlive == value)
                return;

            timeAlive = value;

            if (OnTimeAliveChange != null)
                OnTimeAliveChange();
        } 
    }

    #endregion

    #endregion

    #region Object References

    [SerializeField] private Transform poolContainer;
    public Transform PoolContainer { get { return poolContainer; } }

    [SerializeField] private GameObject nextWaveCountdownPanel;
    [SerializeField] private GameObject waveClearedPanel;

    [SerializeField] private Animator waveProgressPanelAnimator;
    [SerializeField] private TextMeshProUGUI killRemainingVarText;

    #endregion

    #region Events and Delegates

    public delegate void OnEnemyKilledDelegate();
    public event OnEnemyKilledDelegate OnEnemyKilled;

    public delegate void OnHitReceivedDelegate();
    public event OnHitReceivedDelegate OnHitReceived;

    public delegate void OnWaveValueChangeDelegate();
    public event OnWaveValueChangeDelegate OnWaveValueChange;

    public delegate void OnTimeAliveChangeDelegate();
    public event OnTimeAliveChangeDelegate OnTimeAliveChange;

    #endregion

    private void ResetProgress()
    {
        CurrentWave = 0;
        TimeAlive = 0f;
        HitsReceived = 0;
        TotalEnemiesKilled = 0;

        currentEnemiesToKill = 0;
        currentKilledEnemies = 0;

        waveClearedPanel.SetActive(false);
        nextWaveCountdownPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (!initialized)
            return;

        ResetProgress();

        GameManager.instance.GetPlayer().GetComponent<HealthManager>().OnDamaged += HitReceived;
        GameManager.instance.GetPlayer().GetComponent<HealthManager>().OnDeath += Death;

        NextWave();
    }

    private void OnDisable()
    {
        if (!initialized)
            return;

        GameManager.instance.GetPlayer().GetComponent<HealthManager>().OnDamaged -= HitReceived;
        GameManager.instance.GetPlayer().GetComponent<HealthManager>().OnDeath -= Death;
    }

    private void Start()
    {
        GameManager.instance.GetPlayer().GetComponent<HealthManager>().OnDamaged += HitReceived;
        GameManager.instance.GetPlayer().GetComponent<HealthManager>().OnDeath += Death;

        NextWave();

        initialized = true;
    }

    private void HitReceived()
    {
        HitsReceived++;
    }

    private void Death()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!GameManager.instance.IsOnRewardsUI && !GameManager.instance.IsOnMainMenu())
            TimeAlive += Time.deltaTime;
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
                countdownText.text = "<color=red>S</color>TART!";
                GameManager.instance.GetSFXManager().PlaySound(Config.WAVE_START_SFX);
            }
            else
            {
                countdownText.text = i + "...";
                GameManager.instance.GetSFXManager().PlaySound(Config.COUNTDOWN_SFX);
            }

            yield return new WaitForSeconds(1f);
        }

        nextWaveCountdownPanel.SetActive(false);

        StartWave();
    }

    public void StartWave()
    {
        WaveSpawnStruct wave;

        if (currentWave < waves.Count)
            wave = waves[currentWave];
        else
        {
            wave = waves[waves.Count - 1];

            int extraEnemiesToSpawn = (currentWave - waves.Count) * 2;
            wave.EnemiesToSpawn += extraEnemiesToSpawn;
        }

        currentKilledEnemies = 0;
        currentEnemiesToKill = wave.EnemiesToSpawn;

        UpdateKillsRemainingVarText();
        ShowKillsRemainingPanel(true);

        GameManager.instance.GetEnemySpawner().SpawnEnemies(wave);
    }

    public void OnEnemyDeath()
    {
        currentKilledEnemies++;
        TotalEnemiesKilled++;

        UpdateKillsRemainingVarText();

        if (currentKilledEnemies == currentEnemiesToKill)
            WaveCleared();
    }

    private void UpdateKillsRemainingVarText()
    {
        int killsRemaining = currentEnemiesToKill - currentKilledEnemies;

        killRemainingVarText.text = killsRemaining.ToString();
    }

    public void ShowKillsRemainingPanel(bool show)
    {
        if (show)
            waveProgressPanelAnimator.SetTrigger(Config.CROSSFADE_START_TRIGGER);
        else
            waveProgressPanelAnimator.SetTrigger(Config.CROSSFADE_END_TRIGGER);
    }

    public void EnableKillsRemainingPanel(bool enable)
    {
        waveProgressPanelAnimator.gameObject.SetActive(enable);

        if (enable)
            waveProgressPanelAnimator.SetTrigger(Config.CROSSFADE_START_TRIGGER);
    }

    private void WaveCleared()
    {
        StartCoroutine(WaveClearedCoroutine());
        ShowKillsRemainingPanel(false);
    }

    private IEnumerator WaveClearedCoroutine()
    {
        waveClearedPanel.SetActive(true);
        TextMeshProUGUI countdownText = waveClearedPanel.transform.GetComponentInChildren<TextMeshProUGUI>();

        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i + "...";

            GameManager.instance.GetSFXManager().PlaySound(Config.COUNTDOWN_SFX);

            yield return new WaitForSeconds(1f);
        }

        GameManager.instance.GetSFXManager().PlaySound(Config.TRANSITION_START_SFX);

        float transitionTime = GameManager.instance.GetTransitionManager().RunTransition("DoubleWipe");

        yield return new WaitForSeconds(transitionTime + Config.MEDIUM_DELAY);

        List<Item> rewardItems = DrawRewardItems();

        waveClearedPanel.SetActive(false);

        float rewardCountdown = currentWave < waves.Count ? rewardsCountdown[currentWave] : 15f;

        GameManager.instance.GetRewardsUI().OpenRewardsUI(rewardItems, rewardCountdown);

        GameManager.instance.IsOnRewardsUI = true;

        GameManager.instance.GetSFXManager().PlaySound(Config.TRANSITION_END_SFX);

        GameManager.instance.GetTransitionManager().FinishCurrentTransition();

        yield return new WaitForSeconds(transitionTime);
    }

    public void FinishWave()
    {
        GameManager.instance.IsOnRewardsUI = false;

        GameManager.instance.GetRewardsUI().CloseRewardsUI();

        StartCoroutine(FinishWaveCoroutine());
    }

    private IEnumerator FinishWaveCoroutine()
    {
        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);

        GameManager.instance.GetSFXManager().PlaySound(Config.TRANSITION_START_SFX);
        float transitionTime = GameManager.instance.GetTransitionManager().RunTransition("DoubleWipe");

        yield return new WaitForSeconds(transitionTime);

        GameManager.instance.GetInventoryManager().UnblockBlockedItems();

        if (currentWave > 0 && currentWave % 3 == 0)
            CorpseCleanup();

        GameManager.instance.GetSFXManager().PlaySound(Config.TRANSITION_END_SFX);
        GameManager.instance.GetTransitionManager().FinishCurrentTransition();

        bool blockedItems = false;

        int itemsToBlockThisWave = currentWave < waves.Count ? itemsToBlock[currentWave] : itemsToBlock[waves.Count - 1];

        if (itemsToBlockThisWave > 0)
        {
            yield return new WaitForSeconds(transitionTime);

            GameManager.instance.GetPauseMenuUI().OpenPauseInventory();

            transitionTime = GameManager.instance.GetTransitionManager().RunTransition("PaintSplash");

            GameManager.instance.GetSFXManager().PlaySound(Config.BLOOD_SPLATTER_SFX);

            yield return new WaitForSeconds(transitionTime);

            blockedItems = GameManager.instance.GetInventoryManager().BlockRandomItems(itemsToBlockThisWave);

            GameManager.instance.GetTransitionManager().FinishCurrentTransition();

            yield return new WaitForSeconds(transitionTime);
        }

        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(true);

        if (blockedItems)
            GameManager.instance.GetPauseMenuUI().PauseGameAndOpenInventory();

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        CurrentWave++;
        
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

    public void CorpseCleanup()
    {
        foreach (Transform corpse in PoolContainer)
        {
            ObjectPool<GameObject> pool = corpse.GetComponent<PoolableObject>().ObjectPool;

            if (corpse.TryGetComponent(out HealthManager healthManager) && !healthManager.IsAlive)
                healthManager.Resurrect();

            if (corpse.gameObject.activeSelf)
            {
                corpse.gameObject.SetActive(false);

                pool.Release(corpse.gameObject);
            }
        }
    }
}
