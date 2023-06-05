using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AirFishLab.ScrollingList.Demo;
using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;
using UnityEngine.UI;

public class SpriteListBox : ListBox
{
    [SerializeField] private Image _image;

    protected override void UpdateDisplayContent(IListContent content)
    {
        var data = (SpriteStringData)content;
        _image.sprite = data.sprite;
    }
}
