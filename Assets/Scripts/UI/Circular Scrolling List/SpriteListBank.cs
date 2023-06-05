using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteListBank : BaseListBank
{
    [SerializeField]
    private SpriteStringData[] _datas;

    public override IListContent GetListContent(int index)
    {
        return _datas[index];
    }

    public override int GetContentCount()
    {
        return _datas.Length;
    }
}

[Serializable]
public class SpriteStringData : IListContent
{
    [SerializeField]
    private Sprite _sprite;

    public Sprite sprite => _sprite;
}
