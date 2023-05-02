using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using StarterAssets;

public class InventoryManager : MonoBehaviour
{
    #region Inventory Configuration

    [Header("Inventory Configuration")]
    [SerializeField] private int inventoryWidth;
    public int InventoryWidth { get { return inventoryWidth; }}

    [SerializeField] private int inventoryHeight;
    public int InventoryHeight { get { return inventoryHeight;} }

    #endregion

    #region Ammo Related Variables

    [Header("Ammo Related Variables")]
    [SerializedDictionary("Ammo Type", "Current Stocked Ammo")]
    public SerializedDictionary<AmmoType, int> StockedAmmoDictionary;

    [SerializedDictionary("Ammo Type", "Ammo Item Stack")]
    public SerializedDictionary<AmmoType, List<AmmoItem>> AmmoItemListDictionary;

    #endregion

    #region Weapon Fast Swap Related Variables

    [Header("Weapon Fast Swap Related Variables")]
    [SerializeField] private EquipableItem[] fastSwapWeaponArray = new EquipableItem[8];
    public EquipableItem[] FastSwapWeaponArray { get { return fastSwapWeaponArray; } set { fastSwapWeaponArray = value; OnFastSwapConfigurationChange(); } }

    [SerializeField] private EquipableItem equippedItem;
    public EquipableItem EquippedItem { get { return equippedItem; } set { equippedItem = value; } }

    [SerializeField] private int fastSwapIndex = 0;
    public int FastSwapIndex { get { return fastSwapIndex; } }

    [SerializeField] private List<int> fastSwapIndexes;
    [SerializeField] private int currentEquippedWeaponShortcutIndex = -1;

    #endregion

    #region Inventory Information Variables

    [Header("Inventory Information Variables")]
    [SerializeField] private List<Item> blockedItems = new List<Item>();
    public List<Item> BlockedItems { get { return blockedItems; } }

    [SerializeField] private string savedInventory;
    public string SavedInventory { get { return savedInventory; } set { savedInventory = value; } }

    #endregion

    #region Object References

    private StarterAssetsInputs playerInput;
    private ThirdPersonShooterController playerThirdPersonShooterController;

    #endregion

    #region Events and Delegates

    public delegate void OnStockedAmmoChangeDelegate();
    public event OnStockedAmmoChangeDelegate OnStockedAmmoChange;

    public void StockedAmmoChange()
    {
        OnStockedAmmoChange();
    }

    #endregion

    private void Start()
    {
        playerInput = GameManager.instance.GetPlayer().GetComponent<StarterAssetsInputs>();
        playerThirdPersonShooterController = GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>();
    }

    private void Update()
    {
        if (playerThirdPersonShooterController.IsReloading || !playerThirdPersonShooterController.IsAbleToReload)
            return;

        if (!GameManager.instance.GetInventoryUI().IsGamePaused && 
            HasFastSwapWeapons() && 
            playerInput.weaponShortcut > -1)
        {
            EquipableItem equipableItem = FastSwapWeaponArray[playerInput.weaponShortcut];

            if (equipableItem && (currentEquippedWeaponShortcutIndex != playerInput.weaponShortcut))
            {
                equipableItem.Equip();
                currentEquippedWeaponShortcutIndex = playerInput.weaponShortcut;
                GameManager.instance.GetInventoryUI().ShowFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);
            }

            playerInput.weaponShortcut = -1;
        }
        else if (!GameManager.instance.GetInventoryUI().IsGamePaused &&
                 HasFastSwapWeapons() &&
                 fastSwapIndexes.Count > 1 &&
                 playerInput.scrollWheel != Vector2.zero)
        {
            if (playerInput.scrollWheel.y > 0)
            {
                fastSwapIndex--;

                if (fastSwapIndex < 0)
                    fastSwapIndex = fastSwapIndexes.Count - 1;
            }
            else
            {
                fastSwapIndex++;
                fastSwapIndex %= fastSwapIndexes.Count;
            }

            int indexOfWeaponToEquip = fastSwapIndexes[fastSwapIndex];

            if (currentEquippedWeaponShortcutIndex != indexOfWeaponToEquip)
            {
                fastSwapWeaponArray[indexOfWeaponToEquip].Equip();
                currentEquippedWeaponShortcutIndex = indexOfWeaponToEquip;
                GameManager.instance.GetInventoryUI().ShowFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);
            }

            playerInput.scrollWheel = Vector2.zero;
        }
    }

    #region Fast Swap Methods

    private bool HasFastSwapWeapons()
    {
        return fastSwapIndexes.Count > 0;
    }

    private void OnFastSwapConfigurationChange()
    {
        fastSwapIndexes.Clear();

        for (int weaponIndex = 0; weaponIndex < FastSwapWeaponArray.Length; weaponIndex++)
        {
            EquipableItem fastSwapWeapon = FastSwapWeaponArray[weaponIndex];

            if (fastSwapWeapon)
                fastSwapIndexes.Add(weaponIndex);
        }
    }

    #endregion
}
