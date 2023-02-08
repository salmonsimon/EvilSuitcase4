using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{

    #region Logic Variables

    private bool isGamePaused = false;
    public bool IsGamePaused { get { return isGamePaused; } }

    #endregion

    #region References

    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject settingsPanel;

    #endregion

    #region Audio Sliders References

    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    #endregion

    public void PauseGame()
    {
        GameManager.instance.GetSFXManager().PlaySound(Config.PAUSE_SFX);
        SetGamePaused(true);

        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        SetGamePaused(false);

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void SetGamePaused(bool value)
    {
        isGamePaused = value;

        if (value)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void ToMainMenu()
    {
        GameManager.instance.ToMainMenu();
    }

    #region Audio

    public void SetAudioSlidersVolumesPauseMenu()
    {
        musicVolumeSlider.value = Settings.Instance.musicVolume;
        sfxVolumeSlider.value = Settings.Instance.SFXVolume;
    }

    public void UpdateMusicVolume(float value)
    {
        GameManager.instance.GetMusicManager().UpdateVolume(value);
    }

    public void UpdateSFXVolume(float value)
    {
        GameManager.instance.GetSFXManager().UpdateVolume(value);
    }

    #endregion
}
