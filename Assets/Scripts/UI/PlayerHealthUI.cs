using Microlight.MicroBar;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Game Over Panel Reference")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Health Bar Reference")]
    [SerializeField] private MicroBar playerHealthBar;

    [Header("Post Processing Volume References")]
    [SerializeField] private CriticHealthGlobalVolumeAnimator criticalHealthGlobalVolume;
    public CriticHealthGlobalVolumeAnimator CriticalHealthGlobalVolume { get { return criticalHealthGlobalVolume; } }

    [SerializeField] private HurtGlobalVolumeAnimator hurtGlobalVolume;
    public HurtGlobalVolumeAnimator HurtGlobalVolume { get { return hurtGlobalVolume; } }

    private HealthManager playerHealthManager;

    private bool isOpeningMainMenu = false;

    private void OnEnable()
    {
        if (playerHealthManager)
        {
            playerHealthManager.OnCurrentHealthChange += Damaged;

            playerHealthBar.Initialize(playerHealthManager.MaxHitPoints);
        }

        if (gameOverPanel)
            gameOverPanel.SetActive(false);
    }

    private void OnDisable()
    {
        if (playerHealthManager)
            playerHealthManager.OnCurrentHealthChange -= Damaged;

        isOpeningMainMenu = false;
    }

    private void Start()
    {
        playerHealthManager = GameManager.instance.GetPlayer().GetComponent<HealthManager>();

        playerHealthBar.Initialize(playerHealthManager.MaxHitPoints);

        playerHealthManager.OnCurrentHealthChange += Damaged;

        gameOverPanel.SetActive(false);
    }

    private void Damaged()
    {
        playerHealthBar.UpdateHealthBar(playerHealthManager.CurrentHitPoints);
    }

    public void GameOver()
    {
        GameObject player = GameManager.instance.GetPlayer();
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);

        if (!gameOverPanel.activeSelf)
            gameOverPanel.SetActive(true);
        else
            gameOverPanel.GetComponent<Animator>().SetTrigger(Config.ANIMATOR_SHOW_COUNTERS);
    }

    public void ToMainMenu()
    {
        if (!isOpeningMainMenu || GameManager.instance.IsTeleporting())
            StartCoroutine(ToMainMenuCoroutine());
    }

    private IEnumerator ToMainMenuCoroutine()
    {
        isOpeningMainMenu = true;

        gameOverPanel.GetComponent<Animator>().SetTrigger(Config.ANIMATOR_HIDE_COUNTERS);

        yield return new WaitForSeconds(2f);

        foreach (Transform child in GameManager.instance.DisposableObjectsContainer.transform)
            Destroy(child.gameObject);

        GameManager.instance.ToMainMenu();

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        GameManager.instance.GetWaveManager().CorpseCleanup();
        GameManager.instance.GetBloodManager().BloodCleanup();
    }

    public void PlayGame()
    {
        if (GameManager.instance.IsTeleporting())
            return;

        StartCoroutine(PlayGameCoroutine());
    }

    private IEnumerator PlayGameCoroutine()
    {
        GameManager.instance.SetIsTeleporting(true);

        gameOverPanel.GetComponent<Animator>().SetTrigger(Config.ANIMATOR_HIDE_COUNTERS);

        yield return new WaitForSeconds(2f);

        GameManager.instance.GetSFXManager().PlaySound(Config.EVIL_LAUGH_SFX);

        foreach (Transform child in GameManager.instance.DisposableObjectsContainer.transform)
            Destroy(child.gameObject);

        GameManager.instance.GetLevelLoader().LoadLevel(SceneManager.GetActiveScene().name, Config.CROSSFADE_TRANSITION);

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        GameManager.instance.GetWaveManager().CorpseCleanup();
        GameManager.instance.GetBloodManager().BloodCleanup();

        GameManager.instance.GetPlayer().transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
    }
}
