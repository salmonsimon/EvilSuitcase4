using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlIconsManager : MonoBehaviour
{
    [SerializeField] private KeyboardMouseIcons keyboard;
    public KeyboardMouseIcons Keyboard { get { return keyboard; } }
}
