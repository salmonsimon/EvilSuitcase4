using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utils;

[RequireComponent(typeof(GraphicRaycaster), typeof(Canvas))]
public class Item : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum Direction
    {
        Down,
        Left,
        Up,
        Right,
    }

    #region Object References

    [Header("Main Object References")]
    [SerializeField] protected ItemScriptableObject itemSO;
    [SerializeField] protected GameObject mainInventoryButtonPanel;
    [SerializeField] protected GameObject rewardsMainInventoryButtonPanel;
    [SerializeField] protected GameObject rewardsInventoryButtonPanel;

    protected GameObject blockedPanel;
    protected GameObject visualPanel;

    #endregion

    #region Parameters

    protected int width;
    public int Width { get { return width; } }

    protected int height;
    public int Height { get { return height; } }

    protected float cellSize = 50f;
    public float CellSize { get { return cellSize; } private set { cellSize = value; } }

    #endregion

    #region Variables

    protected Direction direction;

    protected Vector2Int origin;

    protected Inventory holdingInventory;
    public Inventory HoldingInventory { get { return holdingInventory; } set { holdingInventory = value; } }

    protected bool isBlocked;
    public bool IsBlocked { get { return isBlocked; } set { isBlocked = value; } }

    #endregion


    protected virtual void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        width = itemSO.Width;
        height = itemSO.Height;

        mainInventoryButtonPanel.SetActive(false);
        rewardsInventoryButtonPanel.SetActive(false);
        rewardsMainInventoryButtonPanel.SetActive(false);

        blockedPanel = transform.Find("Blocked Panel").gameObject;
        blockedPanel.SetActive(false);

        visualPanel = transform.GetChild(0).gameObject;
    }

    protected void OnEnable()
    {
        if (IsBlocked)
            blockedPanel.SetActive(true);
        else
            blockedPanel.SetActive(false);
    }

    public virtual Item RewardItemSetup(RewardItem rewardItem)
    {
        return this;
    }

    public void ItemSetup(Transform parent, Vector2 anchoredPosition, Vector2Int origin, Direction direction, float inventoryCellSize)
    {
        transform.SetParent(parent);
        transform.rotation = Quaternion.Euler(0, GetRotationAngle(direction), 0);

        GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        this.origin = origin;
        this.direction = direction;

        float newScale = inventoryCellSize / CellSize;

        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public GameObject CreateVisualBackgroundGrid(Transform visualParentTransform, ItemScriptableObject itemTetrisSO)
    {
        Transform visualTransform = Instantiate(itemTetrisSO.GridVisual, visualParentTransform);

        Transform template = visualTransform.Find("Template");
        template.gameObject.SetActive(false);

        for (int x = 0; x < itemTetrisSO.Width; x++)
        {
            for (int y = 0; y < itemTetrisSO.Height; y++)
            {
                Transform backgroundSingleTransform = Instantiate(template, visualTransform);
                backgroundSingleTransform.gameObject.SetActive(true);
            }
        }

        visualTransform.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * cellSize;

        visualTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(itemTetrisSO.Width, itemTetrisSO.Height) * cellSize;

        visualTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        visualTransform.SetAsFirstSibling();

        return visualTransform.gameObject;
    }

    #region Getters and Setters

    public Vector2Int GetGridPosition()
    {
        return origin;
    }

    public void SetOrigin(Vector2Int origin)
    {
        this.origin = origin;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return GetGridPositionList(origin, direction, itemSO.Width, itemSO.Height);
    }

    public Direction GetDirection()
    {
        return direction;
    }

    public void SetDirection(Direction direction)
    {
        this.direction = direction;
    }

    public int GetCurrentHorizontalDimension()
    {
        switch (direction)
        {
            default:
            case Direction.Down: return Width;
            case Direction.Left: return Height;
            case Direction.Up: return Width;
            case Direction.Right: return Height;
        }
    }

    public int GetCurrentVerticalDimension()
    {
        switch (direction)
        {
            default:
            case Direction.Down: return Height;
            case Direction.Left: return Width;
            case Direction.Up: return Height;
            case Direction.Right: return Width;
        }
    }

    public ItemScriptableObject GetItemSO()
    {
        return itemSO;
    }

    public ItemType GetItemType()
    {
        return itemSO.ItemType;
    }

    public static Direction GetNextDirection(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return Direction.Left;
            case Direction.Left: return Direction.Up;
            case Direction.Up: return Direction.Right;
            case Direction.Right: return Direction.Down;
        }
    }

    public static int GetRotationAngle(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return 0;
            case Direction.Left: return 90;
            case Direction.Up: return 180;
            case Direction.Right: return 270;
        }
    }

    public static Vector2Int GetRotationOffset(Direction direction, int width, int height)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return new Vector2Int(0, 0);
            case Direction.Left: return new Vector2Int(0, width);
            case Direction.Up: return new Vector2Int(width, height);
            case Direction.Right: return new Vector2Int(height, 0);
        }
    }

    public static List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction direction, int width, int height)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (direction)
        {
            default:
            case Direction.Down:
            case Direction.Up:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Direction.Left:
            case Direction.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }

        return gridPositionList;
    }

    protected virtual GameObject GetCurrentButtonPanel()
    {
        if (GameManager.instance.IsOnRewardsUI)
        {
            return HoldingInventory.MainInventory ? rewardsMainInventoryButtonPanel : rewardsInventoryButtonPanel;
        }
        else
            return mainInventoryButtonPanel;
    }

    #endregion

    #region Item Main Functionalities

    public virtual void AddToMainInventory()
    {
        GameManager.instance.GetInventoryManager().SavedItems.Add(this);
    }

    public virtual void RemoveFromMainInventory()
    {
        if (IsBlocked)
            GameManager.instance.GetInventoryManager().BlockedItems.Remove(this);
        else
            GameManager.instance.GetInventoryManager().SavedItems.Remove(this);
    }

    public virtual void RotateInfoPanels()
    {

    }

    public void DiscardButton()
    {
        holdingInventory.DiscardCandidate = this;
        HoldingInventory.OpenDiscardConfirmationPanel();
    }

    public virtual void Discard()
    {
        Destroy(gameObject);
    }

    public virtual void BlockItem()
    {
        if (IsBlocked)
            return;

        blockedPanel.SetActive(true);

        if (TryGetComponent(out EquipableItem equipableItem))
        {
            equipableItem.DiscardCurrentWeaponShortcut(true);

            EquipableItem currentEquipedItem = GameManager.instance.GetInventoryManager().EquippedItem;

            if (currentEquipedItem && currentEquipedItem.Equals(equipableItem))
                equipableItem.Unequip();

            GameManager.instance.GetInventoryManager().SavedItems.Remove(this);
        }
        else
            RemoveFromMainInventory();

        isBlocked = true;
    }

    public virtual void UnblockItem()
    {
        if (!IsBlocked)
            return;

        blockedPanel.SetActive(false);

        if (IsSubclassOfRawGeneric(GetType(), typeof(EquipableItem)))
        {
            EquipableItem equipableItem = GetComponent<EquipableItem>();
            int weaponShortcut = equipableItem.WeaponShortcut;

            if (weaponShortcut >= 0)
            {
                GameManager.instance.GetPauseMenuUI().SetFastSwapCandidate(equipableItem);
                GameManager.instance.GetPauseMenuUI().SetFastSwapWeapon(weaponShortcut);
            }
        }

        isBlocked = false;
        AddToMainInventory();
    }

    public void AddButton()
    {
        if (!GameManager.instance.GetInventoryManager().AddItemManuallyToMainInventory
            (GameManager.instance.GetRewardsUI().MainInventory, this))
        {
            GameManager.instance.GetSFXManager().PlaySound(Config.WRONG_SFX);
            Debug.LogError("Couldn't add item to main inventory");
        }
        else
            GameManager.instance.GetSFXManager().PlaySound(Config.DROP_SFX);
    }

    #endregion

    #region Mouse/Keyboard Input Scheme

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        GameObject buttonPanelToOpen = GetCurrentButtonPanel();

        if (eventData.button == PointerEventData.InputButton.Right && !IsBlocked)
        {
            HoldingInventory.SetNewOpenButton(buttonPanelToOpen.gameObject);
            buttonPanelToOpen.gameObject.SetActive(true);
            GameManager.instance.GetSFXManager().PlaySound(Config.HOVER_SFX);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject currentButtonPanel = GetCurrentButtonPanel();
        GameObject openButtonPanel = HoldingInventory.OpenItemButtonPanel;

        if (openButtonPanel != currentButtonPanel.gameObject)
        {
            HoldingInventory.SetNewOpenButton(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HoldingInventory.MainInventory && GameManager.instance.GetPauseMenuUI().IsGamePaused)
        {
            if (holdingInventory.TryGetComponent(out MainInventoryItemInfoUI mainInventoryItemInfoUI))
            {
                mainInventoryItemInfoUI.UpdateItemInfoText(itemSO.ItemDescription);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (HoldingInventory.MainInventory && GameManager.instance.GetPauseMenuUI().IsGamePaused)
        {
            if (holdingInventory.TryGetComponent(out MainInventoryItemInfoUI mainInventoryItemInfoUI))
            {
                mainInventoryItemInfoUI.UpdateItemInfoText("");
            }
        }
    }

    #endregion
}
