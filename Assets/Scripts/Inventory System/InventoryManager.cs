using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int inventoryWidth;
    public int InventoryWidth { get { return inventoryWidth; }}

    [SerializeField] private int inventoryHeight;
    public int InventoryHeight { get { return inventoryHeight;} }

    [SerializedDictionary("Ammo Type", "currentAmmo")]
    public SerializedDictionary<AmmoType, int> AmmoDictionary;
    //public SerializedDictionary<AmmoType, int> AmmoDictionary { get { return ammoDictionary; }}

    [SerializeField] private List<Item> blockedItems = new List<Item>();
    public List<Item> BlockedItems { get { return blockedItems; }}

    [SerializeField] private Item[] fastSwapWeaponArray = new Item[8];
    public Item[] FastSwapWeaponArray { get { return fastSwapWeaponArray; } set { fastSwapWeaponArray = value; } }

    [SerializeField] private string savedInventory;
    public string SavedInventory { get { return savedInventory; }}
}
