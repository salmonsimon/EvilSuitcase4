using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextToToggleBinder : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    public void UseToggle()
    {
        toggle.isOn = toggle.isOn ? false : true;
    }
}
