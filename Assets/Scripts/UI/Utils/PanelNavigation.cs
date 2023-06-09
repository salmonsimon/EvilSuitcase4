using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelNavigation : MonoBehaviour
{
    [SerializeField] private GameObject basePanel;
    public GameObject BasePanel { get { return basePanel; } }

    [SerializeField] private Selectable defaultSelected;
    public Selectable DefaultSelected { get { return defaultSelected; } }

    public delegate void OnEnableDelegate(GameObject newActivePanel);
    public event OnEnableDelegate OnEnableEvent;

    private void OnEnable()
    {
        if (OnEnableEvent != null)
            OnEnableEvent.Invoke(this.gameObject);

        if (defaultSelected)
            defaultSelected.Select();
    }
}
