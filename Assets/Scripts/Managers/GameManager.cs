using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
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

    #region UI

    [SerializeField] private MainMenuUI mainMenu;
    [SerializeField] private AmmoDisplayUI ammoDisplayUI;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private InventoryDragDropSystem inventoryDragDropSystem;

    #endregion

    #endregion

    #region Logic Variables

    [SerializeField] private bool isOnMainMenu = false;

    private bool isGamePaused;
    private bool isTeleporting;

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

            Destroy(mainMenu.gameObject);
            Destroy(ammoDisplayUI.gameObject);
            Destroy(inventoryUI.gameObject);
            Destroy(inventoryDragDropSystem.gameObject);
        }
        else
        {
            instance = this;
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
        if (!inventoryUI.IsGamePaused && !isOnMainMenu && Input.GetKeyDown(KeyCode.Escape))
            inventoryUI.PauseGame();
        else if (inventoryUI.IsGamePaused && !isOnMainMenu && Input.GetKeyDown(KeyCode.Escape))
            inventoryUI.ResumeGame();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /*
        switch (scene.name)
        {
            case Config.LOGIN_SCENE_NAME:

                mainMenu.gameObject.SetActive(false);
                pauseMenu.gameObject.SetActive(false);

                progressManager.gameObject.SetActive(false);
                currencyManager.gameObject.SetActive(false);

                leaderboardUI.gameObject.SetActive(false);

                break;

            case Config.MAIN_MENU_SCENE_NAME:

                mainMenu.ResetMainMenu();
                mainMenu.gameObject.SetActive(true);

                pauseMenu.gameObject.SetActive(false);

                progressManager.gameObject.SetActive(true);
                currencyManager.gameObject.SetActive(true);

                leaderboardUI.gameObject.SetActive(true);

                break;

            case Config.MAIN_SCENE_NAME:

                mainMenu.gameObject.SetActive(false);
                pauseMenu.gameObject.SetActive(true);

                progressManager.gameObject.SetActive(true);
                currencyManager.gameObject.SetActive(true);

                leaderboardUI.gameObject.SetActive(true);

                break;
        }

        levelLoader.FinishTransition();
        */
    }

    public void ToMainMenu()
    {
        inventoryUI.ResumeGame();

        SetIsOnMainMenu(true);

        levelLoader.LoadLevel(Config.MAIN_MENU_SCENE_NAME, Config.CROSSFADE_TRANSITION);

        inventoryUI.gameObject.SetActive(false);
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

    public AmmoDisplayUI GetAmmoDisplayUI()
    {
        return ammoDisplayUI;
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

    public InventoryUI GetInventoryUI()
    {
        return inventoryUI;
    }
    #endregion
}
