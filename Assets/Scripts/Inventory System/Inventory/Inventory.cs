using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Configuration")]
    [SerializeField] private bool mainInventory = true;
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

    public event EventHandler<Item> OnItemPlaced;

    private Grid<GridObject> grid;

    private Item discardCandidate;
    public Item DiscardCandidate { get { return discardCandidate; } set { discardCandidate = value; } }

    private void OnDisable()
    {
        discardConfirmationPanel.SetActive(false);
        DiscardCandidate = null;
    }

    private void Awake()
    {
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        backgroundPreview.gameObject.SetActive(false);

        CreateBackground();
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

    public Grid<GridObject> GetGrid()
    {
        return grid;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return grid.IsValidGridPosition(gridPosition);
    }


    public bool TryPlaceItem(ItemSO itemTetrisSO, Vector2Int placedObjectOrigin, Item.Direction direction)
    {
        // Test Can Build
        List<Vector2Int> gridPositionList = Item.GetGridPositionList(placedObjectOrigin, direction, itemTetrisSO.width, itemTetrisSO.height);
        bool canPlace = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);
            if (!isValidPosition)
            {
                // Not valid
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
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canPlace = false;
                    break;
                }
            }
        }

        if (canPlace)
        {
            Vector2Int rotationOffset = Item.GetRotationOffset(direction, itemTetrisSO.width, itemTetrisSO.height);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

            Item placedObject = Item.CreateCanvas(itemContainer, placedObjectWorldPosition, placedObjectOrigin, direction, itemTetrisSO);
            placedObject.transform.rotation = Quaternion.Euler(0, 0, -Item.GetRotationAngle(direction));

            placedObject.GetComponent<ItemDragDrop>().Setup(this);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetItem(placedObject);
            }

            OnItemPlaced?.Invoke(this, placedObject);
            placedObject.HoldingInventory = this;
            placedObject.RotateInfoPanels();
            placedObject.GetComponent<Canvas>().overrideSorting = true;
            placedObject.GetComponent<Canvas>().sortingOrder = 1000 - (20 * gridPositionList[0].y) - gridPositionList[0].x;

            if (mainInventory)
                placedObject.AddToMainInventory();

            // Object Placed!
            return true;
        }
        else
        {
            // Object CANNOT be placed!
            return false;
        }
    }

    public void RemoveItemAt(Vector2Int removeGridPosition)
    {
        Item placedItem = grid.GetGridObject(removeGridPosition.x, removeGridPosition.y).GetItem();

        if (placedItem != null)
        {
            // Demolish
            placedItem.DestroySelf();

            List<Vector2Int> gridPositionList = placedItem.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearItem();
            }
        }
    }

    public RectTransform GetItemContainer()
    {
        return itemContainer;
    }

    private void CreateBackground()
    {
        Transform template = backgroundSlotTemplate;
        template.gameObject.SetActive(false);

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

    [Serializable]
    public struct AddItem
    {
        public string itemSOName;
        public ItemSO.ItemType itemType;
        public Vector2Int gridPosition;
        public Item.Direction direction;
    }

    [Serializable]
    public struct ListAddItem
    {
        public List<AddItem> addItemList;
    }

    public string Save()
    {
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
            addItemList.Add(new AddItem
            {
                direction = item.GetDirection(),
                gridPosition = item.GetGridPosition(),
                itemSOName = (item.GetItemSO() as ItemSO).name,
                itemType = item.GetItemType()
            });

        }

        return JsonUtility.ToJson(new ListAddItem { addItemList = addItemList });
    }

    public void Load(string loadString)
    {
        ListAddItem listAddItem = JsonUtility.FromJson<ListAddItem>(loadString);

        foreach (AddItem addItem in listAddItem.addItemList)
        {
            TryPlaceItem(GetItemSOFromName(addItem.itemSOName, addItem.itemType), addItem.gridPosition, addItem.direction);
        }
    }

    public ItemSO GetItemSOFromName(string itemSOName, ItemSO.ItemType itemType)
    {
        ItemSO itemSO = Resources.Load("Scriptable Objects/Items/" + itemType.ToString() + "/" + itemSOName) as ItemSO;

        return itemSO;
    }

    public void OpenDiscardConfirmationPanel()
    {
        TextMeshProUGUI TMPRO = discardConfirmationPanel.GetComponentInChildren<TextMeshProUGUI>();
        TMPRO.text = TMPRO.text.Split(":")[0] + ": " + DiscardCandidate.GetItemSO().itemName;

        discardConfirmationPanel.SetActive(true);
    }

    public void DiscardConfirmationButton()
    {
        if (DiscardCandidate)
            DiscardCandidate.Discard();
    }

    public void CancelDiscardButton()
    {
        if (DiscardCandidate)
            DiscardCandidate = null;
    }
}
