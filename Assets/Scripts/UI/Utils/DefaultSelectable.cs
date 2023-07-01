using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultSelectable : MonoBehaviour
{
    [SerializeField] private Selectable defaultSelectable;
    [SerializeField] private List<Text> defaultSelectedText;

    private void OnEnable()
    {
        defaultSelectable.Select();

        if (defaultSelectedText.Count > 0)
            foreach (Text text in defaultSelectedText)
                text.GetComponent<TextColorSetter>().SetSelectedColor();    
    }
}
