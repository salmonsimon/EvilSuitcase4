using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainInventoryItemInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemInfoText;

    public void UpdateItemInfoText(string itemInfo)
    {
        itemInfoText.text = itemInfo;
    }
}