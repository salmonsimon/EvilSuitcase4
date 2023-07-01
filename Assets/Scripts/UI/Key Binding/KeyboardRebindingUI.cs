using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyboardRebindingUI : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;

    private KeyboardMouseIcons keyboard;

    protected void OnEnable()
    {
        keyboard = GameManager.instance.GetControlIconsManager().Keyboard;

        var rebindUIComponents = transform.GetComponentsInChildren<RebindActionUI>();
        foreach (var component in rebindUIComponents)
        {
            component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
            component.UpdateBindingDisplay();
        }
    }

    protected void OnUpdateBindingDisplay(RebindActionUI component, string bindingDisplayString, string deviceLayoutName, string controlPath)
    {
        if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
            return;

        var icon = default(Sprite);

        if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard") || InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
            icon = keyboard.GetSprite(controlPath);

        var textComponent = component.bindingText;

        var imageGO = textComponent.transform.parent.Find("ActionBindingIcon");
        var imageComponent = imageGO.GetComponent<Image>();

        if (icon != null)
        {
            textComponent.gameObject.SetActive(false);
            imageComponent.sprite = icon;
            imageComponent.gameObject.SetActive(true);
        }
        else
        {
            textComponent.gameObject.SetActive(true);
            imageComponent.gameObject.SetActive(false);
        }
    }

    public void ResetAllBinding()
    {
        foreach (InputActionMap actionMap in inputActions.actionMaps)
            actionMap.RemoveAllBindingOverrides();

        PlayerPrefs.DeleteKey("rebinds");
    }
}
