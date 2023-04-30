using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item Scriptable Object", order = 0)]
public class ItemScriptableObject : ScriptableObject
{
    [Header("Item Basic Information")]
    [SerializeField] private string itemName;
    public string ItemName { get { return itemName; } }

    [SerializeField] private ItemType itemType;
    public ItemType ItemType { get { return itemType; } }

    [Header("Item Dimensions")]
    [SerializeField] private int width;
    public int Width { get { return width; } }

    [SerializeField] private int height;
    public int Height { get { return height; } }

    [Header("Inventory Related Prefabs")]
    [SerializeField] private Transform gridVisual;
    public Transform GridVisual { get { return gridVisual; } }

    [SerializeField] private Transform itemPrefab;
    public Transform ItemPrefab { get { return itemPrefab; } }

    [SerializeField] private Transform itemVisual;
    public Transform ItemVisual { get { return itemVisual; } }

    [SerializeField] private Transform itemFastSwapVisual;
    public Transform ItemFastSwapVisual { get { return itemFastSwapVisual; } }
}
