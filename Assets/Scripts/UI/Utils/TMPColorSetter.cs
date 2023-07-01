using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPColorSetter : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
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
