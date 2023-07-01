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
        Color defaultColor;
        ColorUtility.TryParseHtmlString("#878787", out defaultColor);

        if (text)
            text.color = defaultColor;
    }

    public void SetSelectedColor()
    {
        if (text)
            text.color = Color.white;
    }
}
