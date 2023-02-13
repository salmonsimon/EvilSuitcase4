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

    [SerializeField] private SurfaceManager surfaceManager;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private CinemachineShake cinemachineShake;
    [SerializeField] private SFXManager sfxManager;
    [SerializeField] private MusicManager musicManager;

    #region UI

    [SerializeField] private MainMenuUI mainMenu;
    [SerializeField] private PauseUI pauseMenu;
    [SerializeField] private AmmoDisplayUI ammoDisplayUI;

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
            Destroy(surfaceManager.gameObject);
            Destroy(cinemachineShake.gameObject);
            Destroy(sfxManager.gameObject);
            Destroy(musicManager.gameObject);

            Destroy(mainMenu.gameObject);
            Destroy(pauseMenu.gameObject);
            Destroy(ammoDisplayUI.gameObject);
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
        // TO DO: CHANGE TO NEW INPUT
        /*
        if (!pauseMenu.IsGamePaused && !isOnMainMenu && Input.GetKeyDown(KeyCode.Escape))
            pauseMenu.PauseGame();
        else if (pauseMenu.IsGamePaused && !isOnMainMenu && Input.GetKeyDown(KeyCode.Escape))
            pauseMenu.ResumeGame();
        */
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
        pauseMenu.ResumeGame();

        SetIsOnMainMenu(true);

        levelLoader.LoadLevel(Config.MAIN_MENU_SCENE_NAME, Config.CROSSFADE_TRANSITION);

        pauseMenu.gameObject.SetActive(false);
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

    public PauseUI GetPauseUI()
    {
        return pauseMenu;
    }

    public MainMenuUI GetMainMenuUI()
    {
        return mainMenu;
    }

    public AmmoDisplayUI GetAmmoDisplayUI()
    {
        return ammoDisplayUI;
    }

    #endregion
}
