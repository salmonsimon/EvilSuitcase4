using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    #region Panel References

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    #endregion

    #region Audio Sliders References

    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    #endregion

    public void ResetMainMenu()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        GameManager.instance.SetIsOnMainMenu(false);

        GameManager.instance.GetLevelLoader().LoadLevel(Config.MAIN_SCENE_NAME, Config.CROSSFADE_TRANSITION);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #region Audio

    public void SetAudioSlidersVolumes()
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
