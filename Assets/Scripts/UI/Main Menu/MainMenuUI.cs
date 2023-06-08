using Mono.Cecil.Cil;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    #region Panel References

    [Header("Panel References")]
    [SerializeField] private List<GameObject> panelList;
    [SerializeField] private GameObject backgroundPanel;

    [Header("Setting Panels")]
    [SerializeField] private GameObject gameplaySettingsPanel;
    [SerializeField] private GameObject keyBindingPanel;

    #endregion

    #region Audio Sliders References

    [Header("Audio")]
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    #endregion

    #region Object References

    private InputsUI inputUI;
    private StarterAssetsInputs inputGameplay;

    #endregion

    #region Variables

    private GameObject activePanel = null;

    private bool isOnKeyBindingPanel = false;
    private bool isOnMapConfirmationPanel = false;


    #endregion

    private void Awake()
    {
        foreach (GameObject panel in panelList)
            if (panel.TryGetComponent(out PanelNavigation panelNavigation))
                panelNavigation.OnEnableEvent += SetAsActivePanelOnEnable;
    }

    private void OnEnable()
    {
        ResetMainMenu();
    }

    private void OnDisable()
    {
        foreach (GameObject panel in panelList)
            if (panel.TryGetComponent(out PanelNavigation panelNavigation))
                panelNavigation.OnEnableEvent -= SetAsActivePanelOnEnable;
    }

    private void SetAsActivePanelOnEnable(GameObject newActivePanel)
    {
        activePanel = newActivePanel;
    }

    private void Start()
    {
        ResetMainMenu();

        inputUI = GameManager.instance.GetPlayer().GetComponent<InputsUI>();
        inputGameplay = GameManager.instance.GetPlayer().GetComponent<StarterAssetsInputs>();
    }

    private void PanelChange()
    {
        GameObject newActivePanel = activePanel.GetComponent<PanelNavigation>().BasePanel;

        activePanel.SetActive(false);
        newActivePanel.SetActive(true);

        activePanel = newActivePanel;
    }

    private void WelcomingScreenPanelChange()
    {
        GameObject newActivePanel = activePanel.GetComponent<PanelNavigation>().BasePanel;

        panelList[0].GetComponent<Animator>().SetTrigger(Config.ANIMATOR_HIDE_COUNTERS);
        StartCoroutine(WaitAndDisable(panelList[0], Config.BIG_DELAY * 2 + Config.SMALL_DELAY));

        activePanel = newActivePanel;
        StartCoroutine(WaitAndEnable(activePanel, Config.BIG_DELAY * 2));
    }

    private IEnumerator WaitAndDisable(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);
    }

    private IEnumerator WaitAndEnable(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (activePanel.Equals(panelList[0]))
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || 
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame)
            {
                WelcomingScreenPanelChange();
                inputUI.ResetInputs();
            }

            return;
        }


        if (!isOnKeyBindingPanel && !isOnMapConfirmationPanel && inputUI.pause)
        {
            PanelChange();

            inputUI.pause = false;
        }
        else if (isOnKeyBindingPanel && inputUI.pause)
        {
            CloseKeyBindingsPanel();
            inputUI.pause = false;
        }

        if (inputUI.enter)
        {
            var currentSelected = EventSystem.current.currentSelectedGameObject;

            if (currentSelected && currentSelected.activeSelf)
            {
                if (currentSelected.TryGetComponent(out Button selectedButton))
                    selectedButton.onClick.Invoke();
                else if (currentSelected.TryGetComponent(out TMP_Dropdown dropdown))
                {
                    if (!dropdown.IsExpanded)
                        dropdown.Show();
                    else
                        dropdown.Hide();
                }
                else if (currentSelected.TryGetComponent(out Toggle toggle))
                    toggle.isOn = toggle.isOn ? false : true;
            }

            inputUI.enter = false;
        }
    }

    public void SetNewActivePanel(int panelIndex)
    {
        activePanel = panelList[panelIndex];
    }

    public void SetIsOnMapConfirmationPanel(bool value)
    {
        isOnMapConfirmationPanel = value;

        inputGameplay.ResetInputs();
        inputUI.ResetInputs();

        if (!isOnMapConfirmationPanel)
            activePanel.GetComponent<PanelNavigation>().DefaultSelected.Select();
    }

    #region Main Buttons

    public void ResetMainMenu()
    {
        isOnKeyBindingPanel = false;
        isOnMapConfirmationPanel = false;

        foreach (GameObject panel in panelList)
            panel.SetActive(false);

        activePanel = panelList[0];

        activePanel.SetActive(true);

        if (inputGameplay)
            inputGameplay.ResetInputs();
        if (inputUI)
            inputUI.ResetInputs();
    }

    public void PlayGame(string sceneName)
    {
        inputGameplay.ResetInputs();
        inputUI.ResetInputs();

        GameManager.instance.SetIsOnMainMenu(false);

        GameManager.instance.GetLevelLoader().LoadLevel(sceneName, Config.CROSSFADE_TRANSITION);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion

    #region Key Binding

    public void KeyBindingsButton()
    {
        isOnKeyBindingPanel = true;

        EventSystem.current.SetSelectedGameObject(null);

        gameplaySettingsPanel.SetActive(false);
        keyBindingPanel.SetActive(true);

        activePanel = keyBindingPanel;
    }

    private void CloseKeyBindingsPanel()
    {
        isOnKeyBindingPanel = false;

        keyBindingPanel.SetActive(false);
        gameplaySettingsPanel.SetActive(true);

        activePanel = gameplaySettingsPanel;
        activePanel.GetComponent<PanelNavigation>().DefaultSelected.Select();
    }

    #endregion

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

    #region Gameplay Settings

    public void SetQuality(int qualityIndex)
    {
        GameManager.instance.SetQuality(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        GameManager.instance.SetFullscreen(isFullscreen);
    }

    #endregion
}
