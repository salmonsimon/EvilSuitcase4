using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarResetter : MonoBehaviour
{
    private Scrollbar scrollbar;

    [SerializeField] private float scrollbarDefaultValue;

    private bool initialized = false;

    private void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
        scrollbarDefaultValue = scrollbar.value;

        initialized = true;
    }

    private void OnEnable()
    {
        if (initialized)
            scrollbar.value = scrollbarDefaultValue;
    }
}
