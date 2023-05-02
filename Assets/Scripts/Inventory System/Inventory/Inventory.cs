using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IPointerDownHandler
{
    #region Inventory Configuration - Parameters

    [Header("Inventory Configuration")]
    [SerializeField] private bool mainInventory = true;
    public bool MainInventory { get { return mainInventory; } }

    [SerializeField] private GameObject discardConfirmationPanel;

    [Header("Grid Configuration")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 50f;
    [SerializeField] private RectTransform itemContainer;

    [Header("Background")]
    [SerializeField] private Transform backgroundPreview;
    [SerializeField] private Transform background;
    [SerializeField] private Transform backgroundSlotTemplate;

    #endregion

    #region Object References

    private Grid<GridObject> grid;

    #endregion

    #region Variables

    private Item discardCandidate;
    public Item DiscardCandidate { get { return discardCandidate; } set { discardCandidate = value; } }

    [SerializeField] private GameObject openItemButtonPanel;
    public GameObject OpenItemButtonPanel { get { return openItemButtonPanel; } }

    #endregion

    #region Events

    public event EventHandler<Item> OnItemPlaced;

    #endregion

    private void OnEnable()
    {
        // here charge inventory if main inventory
        // (charge from saved inventory in inventory manager)
    }

    private void OnDisable()
    {
        discardConfirmationPanel.SetActive(false);
        DiscardCandidate = null;

        SetNewOpenButton(null);
    }

    private void Awake()
    {
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        backgroundPreview.gameObject.SetActive(false);

        CreateInventoryBackground();
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        public Item item;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            item = null;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + item;
        }

        public void SetItem(Item item)
        {
            this.item = item;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearItem()
        {
            item = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public Item GetItem()
        {
            return item;
        }

        public bool CanBuild()
        {
            return item == null;
        }

        public bool HasItem()
        {
            return item != null;
        }

    }

    #region Getters and Setters

    public void InventorySetup(int gridWidth, int gridHeight)
    {
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        GetComponent<RectTransform>().pivot = new Vector2((gridWidth * cellSize / 2), (gridHeight * cellSize / 2));

        CreateInventoryBackground();

        Load(GameManager.instance.GetInventoryManager().SavedInventory);
    }

    public Grid<GridObject> GetGrid()
    {
        return grid;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public RectTransform GetItemContainer()
    {
        return itemContainer;
    }

    #endregion

    #region Inventory Main Functionalities

    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return grid.IsValidGridPosition(gridPosition);
    }

    public bool TryPlaceItem(Item item, Vector2Int placedObjectOrigin, Item.Direction direction, bool loadingInventory = false)
    {
        int width = item.GetItemSO().Width;
        int height = item.GetItemSO().Height;

        List<Vector2Int> gridPositionList = Item.GetGridPositionList(placedObjectOrigin, direction, width, height);

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
            Vector2Int rotationOffset = Item.GetRotationOffset(direction, width, height);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

            item.ItemSetup(itemContainer, placedObjectWorldPosition, placedObjectOrigin, direction);

            item.transform.rotation = Quaternion.Euler(0, 0, -Item.GetRotationAngle(direction));

            item.GetComponent<ItemDragDrop>().Setup(this);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetItem(item);
            }

            OnItemPlaced?.Invoke(this, item);
            item.HoldingInventory = this;
            item.RotateInfoPanels();
            item.GetComponent<Canvas>().overrideSorting = true;
            item.GetComponent<Canvas>().sortingOrder = 1000 - (20 * gridPositionList[0].y) - gridPositionList[0].x;

            if (mainInventory && !loadingInventory)
            {
                item.AddToMainInventory();
                GameManager.instance.GetInventoryManager().SavedInventory = Save();
            }

            return true;
        }

        return false;
    }

    public void RemoveItemAt(Vector2Int removeGridPosition)
    {
        Item placedItem = grid.GetGridObject(removeGridPosition.x, removeGridPosition.y).GetItem();

        if (placedItem != null)
        {
            List<Vector2Int> gridPositionList = placedItem.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearItem();
            }
        }
    }

    #endregion

    #region Visual Methods

    private void CreateInventoryBackground()
    {
        Transform template = backgroundSlotTemplate;
        template.gameObject.SetActive(false);

        foreach (Transform child in background)
        {
            if (!child.Equals(backgroundSlotTemplate))
                Destroy(child.gameObject);
        }

        for (int x = 0; x < GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < GetGrid().GetHeight(); y++)
            {
                Transform backgroundSingleTransform = Instantiate(template, background);
                backgroundSingleTransform.gameObject.SetActive(true);
            }
        }

        background.GetComponent<GridLayoutGroup>().cellSize = new Vector2(GetGrid().GetCellSize(), GetGrid().GetCellSize());

        background.GetComponent<RectTransform>().sizeDelta = new Vector2(GetGrid().GetWidth(), GetGrid().GetHeight()) * GetGrid().GetCellSize();
    }

    #endregion

    #region Discard Item

    public void OpenDiscardConfirmationPanel()
    {
        TextMeshProUGUI TMPRO = discardConfirmationPanel.GetComponentInChildren<TextMeshProUGUI>();
        TMPRO.text = TMPRO.text.Split(":")[0] + ": " + DiscardCandidate.GetItemSO().ItemName;

        discardConfirmationPanel.SetActive(true);
    }

    public void DiscardConfirmationButton()
    {
        if (DiscardCandidate)
        {
            DiscardCandidate.Discard();
            Save();
        }
    }

    public void CancelDiscardButton()
    {
        if (DiscardCandidate)
            DiscardCandidate = null;
    }

    #endregion

    #region Open Item Buttons Management
    public void SetNewOpenButton(GameObject newOpenButton)
    {
        if (OpenItemButtonPanel)
            openItemButtonPanel.SetActive(false);

        openItemButtonPanel = newOpenButton;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OpenItemButtonPanel)
        {
            openItemButtonPanel.SetActive(false);
            openItemButtonPanel = null;
        }
    }

    #endregion

    #region Saving / Loading (WIP)

    [Serializable]
    public struct AddItem
    {
        public string itemName;
        public ItemType itemType;
        public Vector2Int gridPosition;
        public Item.Direction direction;
        public int currentAmmo;
        public int currentDurability;
    }

    [Serializable]
    public struct ListAddItem
    {
        public List<AddItem> addItemList;
    }

    public string Save()
    {
        Debug.Log("Saving inventory");

        List<Item> itemList = new List<Item>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                if (grid.GetGridObject(x, y).HasItem())
                {
                    itemList.Remove(grid.GetGridObject(x, y).GetItem());
                    itemList.Add(grid.GetGridObject(x, y).GetItem());
                }
            }
        }

        List<AddItem> addItemList = new List<AddItem>();
        foreach (Item item in itemList)
        {
            int currentAmmo = 0;
            int currentDurability = 0;

            if (item.TryGetComponent(out GunItem gunItem))
                currentAmmo = gunItem.CurrentAmmo;
            else if (item.TryGetComponent(out AmmoItem ammoItem))
                currentAmmo = ammoItem.CurrentAmmo;
            //else if (item.TryGetComponent(out MeleeItem meleeItem))
                // currentDurability = meleeItem.CurrentDurability;

            addItemList.Add(new AddItem
            {
                direction = item.GetDirection(),
                gridPosition = item.GetGridPosition(),
                itemName = (item.GetItemSO() as ItemScriptableObject).name,
                itemType = item.GetItemType(),
                currentAmmo = currentAmmo,
                currentDurability = currentDurability
            });

        }

        return JsonUtility.ToJson(new ListAddItem { addItemList = addItemList });
    }

    public Item GetItemPrefabFromName(string itemSOName, ItemType itemType)
    {
        GameObject instantiatedPrefab = Instantiate(Resources.Load("Scriptable Objects/Item Prefabs/" + itemType.ToString() + "/" + itemSOName)) as GameObject;

        instantiatedPrefab.TryGetComponent(out Item item);

        return item;
    }

    public void Load(string loadString)
    {
        if (string.IsNullOrEmpty(loadString))
            return;

        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        ListAddItem listAddItem = JsonUtility.FromJson<ListAddItem>(loadString);

        foreach (AddItem addItem in listAddItem.addItemList)
        {
            Item itemToPlace = GetItemPrefabFromName(addItem.itemName, addItem.itemType);

            if (itemToPlace.TryGetComponent(out GunItem gunItem))
                itemToPlace.GetComponent<GunItem>().CurrentAmmo = addItem.currentAmmo;
            else if (itemToPlace.TryGetComponent(out AmmoItem ammoItem))
                itemToPlace.GetComponent<AmmoItem>().CurrentAmmo = addItem.currentAmmo;
            //else if (itemToPlace.TryGetComponent(out MeleeItem meleeItem))
            // itemToPlace.GetComponent<MeleeItem>().currentDurability = addItem.currentDurability;

            TryPlaceItem(itemToPlace, addItem.gridPosition, addItem.direction, true);
        }
    }

    #endregion
}
