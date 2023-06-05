using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlIcon : MonoBehaviour
{
    [SerializeField] private InputActionReference input;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        string controlPath = input.action.bindings[0].effectivePath.Split("/")[1];

        image.sprite = GameManager.instance.GetControlIconsManager().Keyboard.GetSprite(controlPath);
    }
}
