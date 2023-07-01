using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelSelectedTextColorToDefault : MonoBehaviour
{
    [SerializeField] private List<Text> selectedTexts = new List<Text>();
    [SerializeField] private List<TextMeshProUGUI> selectedTMPTexts = new List<TextMeshProUGUI>();

    private void OnDisable()
    {
        foreach (Text text in selectedTexts)
            text.GetComponent<TextColorSetter>().SetDefaultColor();

        foreach (TextMeshProUGUI text in selectedTMPTexts)
            text.GetComponent<TMPColorSetter>().SetDefaultColor();

        selectedTexts.Clear();
        selectedTMPTexts.Clear();
    }

    public void AddToSelectedTexts(Text text)
    {
        selectedTexts.Add(text);
    }

    public void RemoveFromSelectedTexts(Text text)
    {
        selectedTexts.Remove(text);
    }

    public void AddToSelectedTMPTexts(TextMeshProUGUI text)
    {
        selectedTMPTexts.Add(text);
    }

    public void RemoveFromSelectedTMPText(TextMeshProUGUI text)
    {
        selectedTMPTexts.Remove(text);
    }
}
