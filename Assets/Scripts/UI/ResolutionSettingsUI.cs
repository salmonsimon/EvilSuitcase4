using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSettingsUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private TextMeshProUGUI mainLabel;

    private Resolution[] resolutions;

    private bool initialized = false;

    private void Start()
    {
        AddResolutionOptionsToDropdown();

        initialized = true;
    }

    private void OnEnable()
    {
        mainLabel.color = Color.white;
    }

    private void AddResolutionOptionsToDropdown() 
    {
        resolutions = Screen.resolutions;

        PopulateDropdown(resolutions);
    }

    private void PopulateDropdown(Resolution[] resolutions)
    {
        resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.ToString();
            options.Add(option);

            if (resolution.Equals(Screen.currentResolution))
                break;

            currentResolutionIndex++;
        }

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution newResolution = resolutions[resolutionIndex];

        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);

        GameManager.instance.GetSFXManager().PlaySound(Config.CLICK_SFX);

        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }
}
