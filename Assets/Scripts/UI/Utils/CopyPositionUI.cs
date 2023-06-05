using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPositionUI : MonoBehaviour
{
    [SerializeField] private RectTransform lookAt;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.localPosition = lookAt.localPosition;
    }
}
