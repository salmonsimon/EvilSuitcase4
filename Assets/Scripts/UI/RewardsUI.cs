using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsUI : MonoBehaviour
{
    [SerializeField] private Inventory mainInventory;

    [SerializeField] private Inventory consumableRewardsInventory;
    [SerializeField] private Inventory meleeWeaponRewardsInventory;
    [SerializeField] private Inventory gunsRewardsInventory;

    private void OnEnable()
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();
        int mainInventoryWidth = inventoryManager.InventoryWidth;
        int mainInventoryHeight = inventoryManager.InventoryHeight;

        mainInventory.InventorySetup(mainInventoryWidth, mainInventoryHeight);


    }
}
