using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item Scriptable Object", order = 0)]
public class ItemSO : ScriptableObject
{
    public enum ItemType
    {
        Ammo,
        Gun,
        Melee,
        Consumable
    }

    public string itemName;
    public ItemType itemType;
    public Transform prefab;
    public Transform visual;
    public Transform fastSwapVisual;
    public Transform gridVisual;
    public int width;
    public int height;
}
