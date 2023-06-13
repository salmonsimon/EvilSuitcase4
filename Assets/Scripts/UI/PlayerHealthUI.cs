using Microlight.MicroBar;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private MicroBar playerHealthBar;
    [SerializeField] private GameObject gameOverPanel;

    private HealthManager playerHealthManager;

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
        StartCoroutine(ToMainMenuCoroutine());
    }

    private IEnumerator ToMainMenuCoroutine()
    {
        gameOverPanel.GetComponent<Animator>().SetTrigger(Config.ANIMATOR_HIDE_COUNTERS);

        yield return new WaitForSeconds(2f);

        GameManager.instance.ToMainMenu();
    }

    public void PlayGame()
    {
        if (GameManager.instance.IsTeleporting())
            return;

        StartCoroutine(PlayGameCoroutine());
    }

    private IEnumerator PlayGameCoroutine()
    {
        gameOverPanel.GetComponent<Animator>().SetTrigger(Config.ANIMATOR_HIDE_COUNTERS);

        yield return new WaitForSeconds(2f);

        GameManager.instance.SetIsTeleporting(true);

        GameManager.instance.GetSFXManager().PlaySound(Config.EVIL_LAUGH_SFX);

        GameManager.instance.GetLevelLoader().LoadLevel(SceneManager.GetActiveScene().name, Config.CROSSFADE_TRANSITION);
    }
}
