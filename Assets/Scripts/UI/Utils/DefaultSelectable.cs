using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultSelectable : MonoBehaviour
{
    [SerializeField] private Selectable defaultSelectable;

    private void OnEnable()
    {
        defaultSelectable.Select();
    }
}
