using EasyTransition;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region GameObjects

    [SerializeField] private GameObject player;
    [SerializeField] private SurfaceManager surfaceManager;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private CinemachineShake cinemachineShake;
    [SerializeField] private SFXManager sfxManager;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private InventoryDragDropSystem inventoryDragDropSystem;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TransitionManager transitionManager;
    [SerializeField] private ControlIconsManager controlIconsManager;
    [SerializeField] private BloodManager bloodManager;

    #region UI

    [SerializeField] private MainMenuUI mainMenu;
    [SerializeField] private WeaponDisplayUI weaponDisplayUI;
    [SerializeField] private PauseMenuUI pauseMenuUI;
    [SerializeField] private RewardsUI rewarsdUI;
    [SerializeField] private CrossHair crosshair;
    [SerializeField] private PlayerHealthUI playerHealthUI;

    #endregion

    #region Containers

    [SerializeField] private GameObject cameraContainer;
    public GameObject CameraContainer { get { return cameraContainer; } }

    [SerializeField] private GameObject effectContainer;
    public GameObject EffectContainer { get { return effectContainer; } }

    [SerializeField] private GameObject projectileContainer;
    public GameObject ProjectileContainer { get { return projectileContainer; } }

    [SerializeField] private GameObject disposableObjectsContainer;
    public GameObject DisposableObjectsContainer { get { return disposableObjectsContainer; } }

    [SerializeField] private GameObject poolContainer;
    public GameObject PoolContainer { get { return poolContainer; } }

    #endregion

    #endregion

    #region Logic Variables

    [SerializeField] private bool isOnMainMenu = false;

    private bool isOnRewardsUI = false;
    public bool IsOnRewardsUI { get { return isOnRewardsUI; } set { isOnRewardsUI = value; } }

    private bool isTeleporting;

    private bool isOnTransition = false;
    public bool IsOnTransition { get { return isOnTransition; } set { isOnTransition = value; } }

    #endregion

    #region Input

    private StarterAssetsInputs inputGameplay;
    private InputsUI inputUI;

    #endregion

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(surfaceManager.gameObject);
            Destroy(cinemachineShake.gameObject);
            Destroy(sfxManager.gameObject);
            Destroy(musicManager.gameObject);
            Destroy(inventoryManager.gameObject);
            Destroy(inventoryDragDropSystem.gameObject);
            Destroy(enemySpawner.gameObject);
            Destroy(waveManager.gameObject);
            Destroy(transitionManager.gameObject);
            Destroy(controlIconsManager.gameObject);
            Destroy(bloodManager.gameObject);

            Destroy(mainMenu.gameObject);
            Destroy(weaponDisplayUI.gameObject);
            Destroy(pauseMenuUI.gameObject);
            Destroy(rewarsdUI.gameObject);
            Destroy(crosshair.gameObject);
            Destroy(playerHealthUI.gameObject);

            Destroy(cameraContainer);
            Destroy(effectContainer);
            Destroy(projectileContainer);
            Destroy(poolContainer);
        }
        else
        {
            instance = this;

            inputGameplay = player.GetComponent<StarterAssetsInputs>();
            inputUI = player.GetComponent<InputsUI>();

            player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            inputGameplay.SetCursorLockState(false);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (!pauseMenuUI.IsGamePaused && IsAbleToPause() && inputGameplay.pause)
        {
            inputGameplay.pause = false;
            inputUI.pause = false;
            pauseMenuUI.PauseGame();
        }
        else if (pauseMenuUI.IsGamePaused && IsAbleToPause() && inputUI.pause)
        {
            inputGameplay.pause = false;
            inputUI.pause = false;
            pauseMenuUI.ResumeGame();
        }
    }

    private bool IsAbleToPause()
    {
        return  !isOnMainMenu && 
                !isOnRewardsUI && 
                !pauseMenuUI.IsOnKeyBindingsPanel &&
                !pauseMenuUI.IsOnFastSwapConfiguration &&
                !transitionManager.RunningTransition &&
                !isTeleporting &&
                !player.GetComponent<PlayerHealthAnimations>().IsOnHurtAnimation &&
                player.GetComponent<HealthManager>().IsAlive;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();

        crosshair.gameObject.SetActive(false);
        weaponDisplayUI.gameObject.SetActive(false);
        playerHealthUI.gameObject.SetActive(false);
        cinemachineShake.gameObject.SetActive(false);

        inventoryManager.gameObject.SetActive(false);

        pauseMenuUI.gameObject.SetActive(false);
        enemySpawner.gameObject.SetActive(false);
        waveManager.gameObject.SetActive(false);
        rewarsdUI.gameObject.SetActive(false);

        bloodManager.gameObject.SetActive(false);

        player.GetComponent<ThirdPersonController>().enabled = false;
        player.GetComponent<ThirdPersonShooterController>().enabled = false;

        switch (scene.name)
        {
            case Config.MAIN_MENU_SCENE_NAME:

                mainMenu.ResetMainMenu();
                mainMenu.gameObject.SetActive(true);

                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
                player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);

                break;

            case Config.ROOFTOP_SCENE_NAME:

                mainMenu.gameObject.SetActive(false);

                crosshair.gameObject.SetActive(true);

                weaponDisplayUI.gameObject.SetActive(true);
                weaponDisplayUI.UnequipWeapon();

                playerHealthUI.gameObject.SetActive(true);

                cinemachineShake.gameObject.SetActive(true);

                inventoryManager.gameObject.SetActive(true);
                inventoryManager.ResetProgress();

                pauseMenuUI.gameObject.SetActive(true);
                enemySpawner.gameObject.SetActive(true);
                rewarsdUI.gameObject.SetActive(true);

                bloodManager.gameObject.SetActive(true);

                player.GetComponent<HealthManager>().Resurrect();

                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
                player.GetComponent<StarterAssetsInputs>().SetCursorLockState(true);

                player.GetComponent<ThirdPersonController>().enabled = true;

                player.GetComponent<ThirdPersonShooterController>().enabled = true;
                player.GetComponent<ThirdPersonShooterController>().UnequipWeapon();

                StartCoroutine(WaitAndEnableWaveManager());

                break;
        }

        player.GetComponent<StarterAssetsInputs>().ResetInputs();
        player.GetComponent<InputsUI>().ResetInputs();

        levelLoader.FinishTransition();
    }

    private IEnumerator WaitAndEnableWaveManager()
    {
        yield return new WaitForSeconds(2f);

        waveManager.gameObject.SetActive(true);
    }

    public void ToMainMenu()
    {
        if (IsTeleporting())
            return;

        pauseMenuUI.ResumeGame();
        pauseMenuUI.ResetMenu();

        SetIsOnMainMenu(true);

        SetIsTeleporting(true);

        sfxManager.PlaySound(Config.GAME_OVER_SFX);

        levelLoader.LoadLevel(Config.MAIN_MENU_SCENE_NAME, Config.CROSSFADE_TRANSITION);

        pauseMenuUI.gameObject.SetActive(false);
    }

    #region Getters and Setters

    public bool IsOnMainMenu()
    {
        return isOnMainMenu;
    }

    public void SetIsOnMainMenu(bool value)
    {
        isOnMainMenu = value;
    }

    public bool IsTeleporting()
    {
        return isTeleporting;
    }

    public void SetIsTeleporting(bool value)
    {
        isTeleporting = value;
    }

    public SurfaceManager GetSurfaceManager()
    {
        return surfaceManager;
    }

    public LevelLoader GetLevelLoader()
    {
        return levelLoader;
    }

    public CinemachineShake GetCinemachineShake()
    {
        return cinemachineShake;
    }

    public SFXManager GetSFXManager()
    {
        return sfxManager;
    }

    public MusicManager GetMusicManager()
    {
        return musicManager;
    }

    public MainMenuUI GetMainMenuUI()
    {
        return mainMenu;
    }

    public WeaponDisplayUI GetWeaponDisplayUI()
    {
        return weaponDisplayUI;
    }

    public InventoryDragDropSystem GetInventoryDragDropSystem()
    {
        return inventoryDragDropSystem;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public InventoryManager GetInventoryManager()
    {
        return inventoryManager;
    }

    public PauseMenuUI GetPauseMenuUI()
    {
        return pauseMenuUI;
    }

    public EnemySpawner GetEnemySpawner()
    {
        return enemySpawner;
    }

    public WaveManager GetWaveManager()
    {
        return waveManager;
    }

    public RewardsUI GetRewardsUI()
    {
        return rewarsdUI;
    }

    public TransitionManager GetTransitionManager()
    {
        return transitionManager;
    }

    public CrossHair GetCrossHair()
    {
        return crosshair;
    }

    public ControlIconsManager GetControlIconsManager()
    {
        return controlIconsManager;
    }

    public BloodManager GetBloodManager()
    {
        return bloodManager;
    }

    public PlayerHealthUI GetPlayerHealthUI()
    {
        return playerHealthUI;
    }

    #endregion

    #region Gameplay Settings

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        sfxManager.PlaySound(Config.CLICK_SFX);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        sfxManager.PlaySound(Config.CLICK_SFX);
    }

    #endregion
}
