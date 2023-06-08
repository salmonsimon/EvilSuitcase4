using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColorSetter : MonoBehaviour
{
    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    public void SetDefaultColor()
    {
        text.color = Color.white;
    }

    public void SetSelectedColor()
    {
        Color selectedColor;
        ColorUtility.TryParseHtmlString("#878787", out selectedColor);

        if (text)
            text.color = selectedColor;
    }
}
