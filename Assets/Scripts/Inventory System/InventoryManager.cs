using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using StarterAssets;
using System.Linq;
using System.Runtime.CompilerServices;
using static Inventory;
using Mono.Cecil;
using static UnityEditor.Progress;

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
    public int CurrentEquippedWeaponShortcutIndex { get { return currentEquippedWeaponShortcutIndex; } set { currentEquippedWeaponShortcutIndex = value; } }

    #endregion

    #region Inventory Information Variables

    [Header("Inventory Information Variables")]

    private List<Item> blockedItems = new List<Item>();
    public List<Item> BlockedItems { get { return blockedItems; } }

    [SerializeField] private List<Item> savedItems = new List<Item>();
    public List<Item> SavedItems { get { return savedItems; } set { savedItems = value; } }

    #endregion

    #region Object References

    private StarterAssetsInputs playerInput;
    private ThirdPersonShooterController playerThirdPersonShooterController;

    [SerializeField] private GameObject itemContainer;
    public GameObject ItemContainer { get { return itemContainer; } }
 
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

    #region Item Blocking Methods

    public void BlockRandomItems(int itemsToBlock)
    {
        List<int> randomIndexList = Enumerable.Range(0, SavedItems.Count).ToList();
        randomIndexList.Shuffle();

        if (itemsToBlock > SavedItems.Count)
            itemsToBlock = SavedItems.Count;

        int i = 0;

        while (itemsToBlock > 0 && i < SavedItems.Count)
        {
            Item itemToBlock = SavedItems[randomIndexList[i]];

            itemToBlock.BlockItem();
            blockedItems.Add(itemToBlock);

            itemsToBlock--;
            i++;
        }

        GameManager.instance.GetInventoryUI().LoadFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);
    }

    public void UnblockBlockedItems()
    {
        foreach (Item blockedItem in blockedItems)
            blockedItem.UnblockItem();

        this.blockedItems.Clear();

        GameManager.instance.GetInventoryUI().LoadFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);
    }

    #endregion

    #region Inventory Filling Methods

    public void AutoSortMainInventory(Inventory inventory, List<Item> items)
    {
        inventory.gameObject.SetActive(false);

        FillInventory(inventory, items);

        inventory.gameObject.SetActive(true);
    }

    public void FillInventory(Inventory inventory, List<Item> items, bool maintainHeight = true)
    {
        int inventoryWidth = inventory.GridWidth;
        int inventoryHeight = inventory.GridHeight;

        Grid<GridObject> grid = new Grid<GridObject>(inventoryWidth, inventoryHeight, 50f, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        List<Item> sortedItems = SortItemsByHeight(items);
        List<Item> unplacedItems = new List<Item>();

        bool couldFillFirstTry = true;
        bool couldFillManually = true;

        int x = 0;
        int y = 0;
        int largestHeightInCurrentRow = 0;

        int finalHeight = 0;

        foreach (Item item in sortedItems)
        {
            item.SetDirection(Item.Direction.Down);

            if (!couldFillFirstTry)
            {
                unplacedItems.Add(item);
                continue;
            }

            if (x + item.GetCurrentHorizontalDimension() > inventoryWidth)
            {
                y += largestHeightInCurrentRow;

                x = 0;
                largestHeightInCurrentRow = 0;
            }

            finalHeight = y + item.GetCurrentVerticalDimension();
            int remainingHeight = inventoryHeight - finalHeight;

            if (remainingHeight >= 0 || !maintainHeight)
            {
                List<Vector2Int> gridPositionList = Item.GetGridPositionList(new Vector2Int(x, y), item.GetDirection(), item.Width, item.Height);

                bool canPlace = true;

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    bool isValidPosition = grid.IsValidGridPosition(gridPosition);

                    if (!isValidPosition)
                    {
                        canPlace = false;
                        break;
                    }

                    if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    item.SetOrigin(new Vector2Int(x, y));

                    foreach (Vector2Int gridPosition in gridPositionList)
                        grid.GetGridObject(gridPosition.x, gridPosition.y).SetItem(item);

                    x += item.GetCurrentHorizontalDimension();

                    if (item.GetCurrentVerticalDimension() > largestHeightInCurrentRow)
                        largestHeightInCurrentRow = item.Height;
                }
                else
                {
                    unplacedItems.Add(item);
                    couldFillFirstTry = false;
                }
            }
            else
            {
                unplacedItems.Add(item);
                couldFillFirstTry = false;
            }
        }

        if (couldFillFirstTry)
        {
            if (!maintainHeight && finalHeight > inventoryHeight)
                inventory.InventorySetup(inventoryWidth, finalHeight);

            GameManager.instance.GetInventoryManager().SavedItems = sortedItems;
        }
        else
        {
            foreach (Item item in unplacedItems)
            {
                if (item.TryGetComponent(out AmmoItem ammoItem))
                {
                    ammoItem.FillCurrentStockedAmmoWithNewAmmoItem();

                    if (ammoItem.CurrentAmmo < 0)
                    {
                        sortedItems.Remove(item);
                        ammoItem.Discard();
                        continue;
                    }
                }

                if (!TryAddingItemManually(grid, item))
                {
                    couldFillManually = false; 
                    break;
                }
            }

            if (!couldFillManually)
                Debug.LogError("Couldn't fill all items in inventory");
            else
                GameManager.instance.GetInventoryManager().SavedItems = sortedItems;
        }
    }

    public bool AddItemManuallyToMainInventory(Inventory inventory, Item item)
    {
        bool couldAddItem = true;

        if (item.TryGetComponent(out AmmoItem ammoItem))
        {
            ammoItem.FillCurrentStockedAmmoWithNewAmmoItem();

            if (ammoItem.CurrentAmmo < 0)
            {
                ammoItem.Discard();

                return couldAddItem;
            }
        }

        if (TryAddingItemManually(inventory.GetGrid(), item))
        {
            inventory.gameObject.SetActive(false);

            inventory.TryPlaceItem(item, item.GetGridPosition(), item.GetDirection());

            inventory.gameObject.SetActive(true);
        }
        else
        {
            AutoSortMainInventory(inventory, GameManager.instance.GetInventoryManager().SavedItems);

            if (TryAddingItemManually(inventory.GetGrid(), item))
            {
                inventory.gameObject.SetActive(false);

                inventory.TryPlaceItem(item, item.GetGridPosition(), item.GetDirection());

                inventory.gameObject.SetActive(true);
            }
            else
                couldAddItem = false;
        }

        return couldAddItem;
    }

    public bool TryAddingItemManually(Grid<GridObject> grid, Item item)
    {
        bool couldAddItem = true;

        if (!TryAddingItemHorizontally(grid, item))
        {
            item.SetDirection(Item.Direction.Left);

            if (!TryAddingItemVertically(grid, item))
                couldAddItem = false;
        }
        
        return couldAddItem;
    }

    private bool TryAddingItemVertically(Grid<GridObject> grid, Item item)
    {
        bool couldAdd = true;

        bool finishedSearching = false;

        int gridWidth = grid.GetWidth();
        int gridHeight = grid.GetHeight();

        int x = 0;
        int y = 0;

        int minWidthInCurrentColumn = int.MaxValue;

        while (!finishedSearching)
        {
            item.SetDirection(Item.Direction.Left);

            if (y + item.GetCurrentVerticalDimension() > gridHeight)
            {
                x += minWidthInCurrentColumn;

                y = 0;
                minWidthInCurrentColumn = int.MaxValue;
            }

            if (x >= gridWidth)
            {
                couldAdd = false;
                finishedSearching = true;

                break;
            }

            List<Vector2Int> gridPositionList = Item.GetGridPositionList(new Vector2Int(x, y), item.GetDirection(), item.Width, item.Height);

            bool canPlace = true;

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                bool isValidPosition = grid.IsValidGridPosition(gridPosition);

                if (!isValidPosition)
                {
                    canPlace = false;
                    break;
                }

                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                item.SetOrigin(new Vector2Int(x, y));

                foreach (Vector2Int gridPosition in gridPositionList)
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetItem(item);

                break;
            }
            else
            {
                Item placedItem = grid.GetGridObject(x, y).GetItem();

                int remainingItemWidth = placedItem.GetCurrentHorizontalDimension() + placedItem.GetGridPosition().x - x;

                if (remainingItemWidth < minWidthInCurrentColumn)
                    minWidthInCurrentColumn = remainingItemWidth;

                y += placedItem.GetCurrentVerticalDimension();
            }
        }

        return couldAdd;
    }

    private bool TryAddingItemHorizontally(Grid<GridObject> grid, Item item)
    {

        bool couldAdd = true;

        bool finishedSearching = false;

        int gridWidth = grid.GetWidth();
        int gridHeight = grid.GetHeight();

        int x = 0;
        int y = 0;

        int minHeightInCurrentRow = int.MaxValue;

        while (!finishedSearching)
        {
            if (x + item.GetCurrentHorizontalDimension() > gridWidth)
            {
                y += minHeightInCurrentRow;

                x = 0;
                minHeightInCurrentRow = int.MaxValue;
            }

            if (y >= gridHeight)
            {
                couldAdd = false;
                finishedSearching = true;
                
                break;
            }

            List<Vector2Int> gridPositionList = Item.GetGridPositionList(new Vector2Int(x, y), item.GetDirection(), item.Width, item.Height);

            bool canPlace = true;

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                bool isValidPosition = grid.IsValidGridPosition(gridPosition);

                if (!isValidPosition)
                {
                    canPlace = false;
                    break;
                }

                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                item.SetOrigin(new Vector2Int(x, y));

                foreach (Vector2Int gridPosition in gridPositionList)
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetItem(item);

                break;
            }
            else
            {
                Item placedItem = grid.GetGridObject(x, y).GetItem();


                int remainingItemHeight = placedItem.GetCurrentVerticalDimension() + placedItem.GetGridPosition().y - y;

                if (remainingItemHeight < minHeightInCurrentRow)
                    minHeightInCurrentRow = remainingItemHeight;

                x += placedItem.GetCurrentHorizontalDimension();
            }
        }

        return couldAdd;
    }

    private List<Item> SortItemsByHeight(List<Item> items)
    {
        List<Item> orderedList = items.OrderBy(i => i.Height).ToList();
        orderedList.Reverse();

        return orderedList;
    } 

    #endregion
}
