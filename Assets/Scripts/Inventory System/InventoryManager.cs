using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using StarterAssets;
using System.Linq;
using static Inventory;

public class InventoryManager : MonoBehaviour
{
    #region Inventory Configuration

    [Header("Inventory Configuration")]
    [SerializeField] private int inventoryWidth;
    public int InventoryWidth { get { return inventoryWidth; } }

    [SerializeField] private int inventoryHeight;
    public int InventoryHeight { get { return inventoryHeight; } }

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
    public EquipableItem EquippedItem { get { return equippedItem; } set { equippedItem = value; OnEquippedItemChange(); } }

    [SerializeField] private int fastSwapIndex = 0;
    public int FastSwapIndex { get { return fastSwapIndex; } }

    [SerializeField] private List<int> fastSwapIndexes;

    [SerializeField] private int currentEquippedWeaponShortcutIndex = -1;
    public int CurrentEquippedWeaponShortcutIndex { get { return currentEquippedWeaponShortcutIndex; } set { currentEquippedWeaponShortcutIndex = value; } }

    #endregion

    #region Inventory Information Variables

    [Header("Inventory Information Variables")]

    [SerializeField] private List<Item> blockedItems = new List<Item>();
    public List<Item> BlockedItems { get { return blockedItems; } }

    [SerializeField] private List<Item> savedItems = new List<Item>();
    public List<Item> SavedItems { get { return savedItems; } set { savedItems = value; } }

    #endregion

    #region Object References

    private StarterAssetsInputs playerInput;
    private ThirdPersonShooterController playerThirdPersonShooterController;

    [SerializeField] private GameObject itemContainer;
    public GameObject ItemContainer { get { return itemContainer; } }

    [SerializeField] private EquipableItem suitcaseItem;
    public EquipableItem SuitcaseItem { get { return suitcaseItem; } }

    #endregion

    #region Events and Delegates

    public delegate void OnStockedAmmoChangeDelegate();
    public event OnStockedAmmoChangeDelegate OnStockedAmmoChange;

    public void StockedAmmoChange()
    {
        OnStockedAmmoChange();
    }

    #endregion

    public void ResetProgress()
    {
        SuitcaseItem.DiscardCurrentWeaponShortcut();

        for (int i = BlockedItems.Count - 1; i >= 0; i--)
            BlockedItems[i].Discard();

        for (int j = SavedItems.Count - 1; j >= 0; j--)
            SavedItems[j].Discard();

        blockedItems.Clear();
        savedItems.Clear();

        fastSwapIndex = 0;
        CurrentEquippedWeaponShortcutIndex = -1;
    }

    private void Start()
    {
        playerInput = GameManager.instance.GetPlayer().GetComponent<StarterAssetsInputs>();
        playerThirdPersonShooterController = GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>();
    }

    private void Update()
    {
        if (playerThirdPersonShooterController.IsReloading ||
            !playerThirdPersonShooterController.IsAbleToReload ||
            playerThirdPersonShooterController.IsAttacking)
            return;

        if (playerInput.weaponShortcut > -1 && AbleToFastSwap())
        {
            EquipableItem equipableItem = FastSwapWeaponArray[playerInput.weaponShortcut];

            if (equipableItem && (currentEquippedWeaponShortcutIndex != playerInput.weaponShortcut))
            {
                equipableItem.Equip();
                currentEquippedWeaponShortcutIndex = playerInput.weaponShortcut;
                GameManager.instance.GetPauseMenuUI().ShowFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);
            }

            playerInput.weaponShortcut = -1;
        }
        else if (playerInput.scrollWheel != Vector2.zero && AbleToFastSwap())
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
                GameManager.instance.GetPauseMenuUI().ShowFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);
            }

            playerInput.scrollWheel = Vector2.zero;
        }
    }

    private bool AbleToFastSwap()
    {
        return !GameManager.instance.GetPauseMenuUI().IsGamePaused &&
                 !GameManager.instance.IsOnRewardsUI &&
                 !GameManager.instance.GetTransitionManager().RunningTransition &&
                 GameManager.instance.GetWaveManager().ChallengingWave &&
                 HasFastSwapWeapons();
    }

    #region Fast Swap Methods

    private bool HasFastSwapWeapons()
    {
        return fastSwapIndexes.Count > 0;
    }

    private void OnEquippedItemChange()
    {
        if (equippedItem.WeaponShortcut >= 0)
        {
            CurrentEquippedWeaponShortcutIndex = equippedItem.WeaponShortcut;

            int i = 0;
            for (int weaponIndex = 0; weaponIndex < FastSwapWeaponArray.Length; weaponIndex++)
            {
                EquipableItem fastSwapWeapon = FastSwapWeaponArray[weaponIndex];

                if (fastSwapWeapon && fastSwapWeapon.Equals(GameManager.instance.GetInventoryManager().EquippedItem))
                    fastSwapIndex = i;
                else if (fastSwapWeapon)
                    i++;
            }
        }
        else
        {
            CurrentEquippedWeaponShortcutIndex = -1;
            fastSwapIndex = 0;
        }
    }

    private void OnFastSwapConfigurationChange()
    {
        fastSwapIndexes.Clear();

        int i = 0;
        for (int weaponIndex = 0; weaponIndex < FastSwapWeaponArray.Length; weaponIndex++)
        {
            EquipableItem fastSwapWeapon = FastSwapWeaponArray[weaponIndex];

            if (fastSwapWeapon && fastSwapWeapon.Equals(GameManager.instance.GetInventoryManager().EquippedItem))
            {
                fastSwapIndex = i;
                fastSwapIndexes.Add(weaponIndex);
            }
            else if (fastSwapWeapon)
            {
                fastSwapIndexes.Add(weaponIndex);
                i++;
            }
        }

        GameManager.instance.GetPauseMenuUI().LoadFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);
    }

    #endregion

    #region Item Blocking Methods

    public bool BlockRandomItems(int itemsToBlock)
    {
        List<int> randomIndexList = Enumerable.Range(0, SavedItems.Count).ToList();
        randomIndexList.Shuffle();

        if (itemsToBlock > SavedItems.Count)
            itemsToBlock = SavedItems.Count;

        int i = 0;
        while (itemsToBlock > 0 && i < SavedItems.Count)
        {
            Item itemToBlock = SavedItems[randomIndexList[i]];

            blockedItems.Add(itemToBlock);

            itemsToBlock--;
            i++;
        }

        foreach (Item itemToBlock in blockedItems)
            itemToBlock.BlockItem();

        GameManager.instance.GetPauseMenuUI().LoadFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);

        return i > 0;
    }

    public void UnblockBlockedItems()
    {
        foreach (Item blockedItem in blockedItems)
            blockedItem.UnblockItem();

        this.blockedItems.Clear();

        GameManager.instance.GetPauseMenuUI().LoadFastSwapGameplayPanel(currentEquippedWeaponShortcutIndex);
    }

    #endregion

    #region Inventory Filling Methods

    public void AutoSortMainInventory(Inventory inventory)
    {
        inventory.gameObject.SetActive(false);

        FillInventory(inventory, GetMainInventoryItems());

        inventory.gameObject.SetActive(true);
    }

    public void FillRewardsInventory(Inventory inventory, List<Item> items)
    {
        inventory.gameObject.SetActive(false);

        FillInventory(inventory, items, false);

        foreach (Item item in items)
            inventory.TryPlaceItem(item, item.GetGridPosition(), item.GetDirection());

        inventory.gameObject.SetActive(true);
    }

    public void FillInventory(Inventory inventory, List<Item> items, bool maintainHeight = true)
    {
        int inventoryWidth = inventory.GridWidth;
        int inventoryHeight = inventory.GridHeight;

        Grid<GridObject> grid;

        if (maintainHeight)
            grid = new Grid<GridObject>(inventoryWidth, inventoryHeight, 50f, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        else
            grid = new Grid<GridObject>(inventoryWidth, 200, 50f, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        List<Vector2Int> originalPositions = new List<Vector2Int>();
        List<Item.Direction> originalDirections = new List<Item.Direction>();

        foreach (Item item in items)
        {
            originalPositions.Add(item.GetGridPosition());
            originalDirections.Add(item.GetDirection());
        }

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
            {
                inventory.InventorySetup(inventoryWidth, finalHeight);

                foreach (Item item in sortedItems)
                {
                    Vector2Int origin = item.GetGridPosition();

                    int correctedY = finalHeight - origin.y - item.GetCurrentVerticalDimension();
                    Vector2Int newOrigin = new Vector2Int(origin.x, correctedY);

                    item.SetOrigin(newOrigin);
                }
            }
            else
            {
                foreach (Item item in sortedItems)
                {
                    Vector2Int origin = item.GetGridPosition();

                    int correctedY = inventoryHeight - origin.y - item.GetCurrentVerticalDimension();
                    Vector2Int newOrigin = new Vector2Int(origin.x, correctedY);

                    item.SetOrigin(newOrigin);
                }
            }
        }
        else
        {
            foreach (Item item in unplacedItems)
            {
                if (!TryAddingItemManually(grid, item))
                {
                    couldFillManually = false;
                    break;
                }
            }

            if (!couldFillManually)
            {
                Debug.LogError("Couldn't fill all items in inventory");
                GameManager.instance.GetSFXManager().PlaySound(Config.WRONG_SFX);

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SetOrigin(originalPositions[i]);
                    items[i].SetDirection(originalDirections[i]);
                }
            }
            else
            {
                foreach (Item item in sortedItems)
                {
                    Vector2Int origin = item.GetGridPosition();

                    int correctedY = inventoryHeight - origin.y - item.GetCurrentVerticalDimension();
                    Vector2Int newOrigin = new Vector2Int(origin.x, correctedY);

                    item.SetOrigin(newOrigin);
                }
            }
        }
    }

    public bool AddItemManuallyToMainInventory(Inventory inventory, Item item)
    {
        bool couldAddItem = true;

        if (item.TryGetComponent(out AmmoItem ammoItem))
        {
            ammoItem.FillCurrentStockedAmmoWithNewAmmoItem();

            if (ammoItem.CurrentAmmo <= 0)
            {
                Destroy(item.gameObject);

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
            AutoSortMainInventory(inventory);

            if (TryAddingItemManually(inventory.GetGrid(), item))
            {
                inventory.gameObject.SetActive(false);

                inventory.TryPlaceItem(item, item.GetGridPosition(), item.GetDirection());

                inventory.gameObject.SetActive(true);
            }
            else
            {
                AutoSortMainInventory(inventory);

                if (TryAddingItemManually(inventory.GetGrid(), item))
                {
                    inventory.gameObject.SetActive(false);

                    inventory.TryPlaceItem(item, item.GetGridPosition(), item.GetDirection());

                    inventory.gameObject.SetActive(true);
                }
                else
                    couldAddItem = false;
            }
        }

        return couldAddItem;
    }

    public bool TryAddingItemManually(Grid<GridObject> grid, Item item)
    {
        bool couldAddItem = true;

        item.SetDirection(Item.Direction.Down);

        if (!TryAddingItemHorizontally(grid, item))
        {
            item.SetDirection(Item.Direction.Left);

            if (!TryAddingItemVertically(grid, item))
            {
                couldAddItem = false;
                item.SetDirection(Item.Direction.Down);
            }
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

        int minWidthInCurrentColumn = gridWidth;

        while (!finishedSearching)
        {
            if (y + item.GetCurrentVerticalDimension() > gridHeight)
            {
                x += minWidthInCurrentColumn;

                y = 0;
                minWidthInCurrentColumn = gridWidth;
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

                if (placedItem != null)
                {
                    int remainingItemWidth = placedItem.GetCurrentHorizontalDimension() + placedItem.GetGridPosition().x - x;

                    if (remainingItemWidth < minWidthInCurrentColumn)
                        minWidthInCurrentColumn = remainingItemWidth;

                    y += placedItem.GetCurrentVerticalDimension();
                }
                else
                    y++;
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

        int minHeightInCurrentRow = gridHeight;

        while (!finishedSearching)
        {
            if (x + item.GetCurrentHorizontalDimension() > gridWidth)
            {
                y += minHeightInCurrentRow;

                x = 0;
                minHeightInCurrentRow = gridHeight;
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

                if (placedItem != null)
                {
                    int remainingItemHeight = placedItem.GetCurrentVerticalDimension() + placedItem.GetGridPosition().y - y;

                    if (remainingItemHeight < minHeightInCurrentRow)
                        minHeightInCurrentRow = remainingItemHeight;

                    x += placedItem.GetCurrentHorizontalDimension();
                }
                else
                    x++;
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

    public List<Item> GetMainInventoryItems()
    {
        List<Item> mainInventoryItems = new List<Item>();
        mainInventoryItems.AddRange(SavedItems);
        mainInventoryItems.AddRange(BlockedItems);

        return mainInventoryItems;
    }

    #endregion
}