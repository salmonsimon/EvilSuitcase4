using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utils;

[RequireComponent(typeof(GraphicRaycaster),  typeof(Canvas))]
public class Item : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
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
    [SerializeField] protected RectTransform buttonsPanel;

    protected GameObject blockedPanel;

    #endregion

    #region Parameters

    protected float width;
    protected float height;
    protected float cellWidth;
    protected float cellHeight;

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

        cellWidth = rectTransform.sizeDelta.x;
        cellHeight = rectTransform.sizeDelta.y;

        width = itemSO.Width;
        height = itemSO.Height;

        blockedPanel = transform.Find("Blocked Panel").gameObject;
        blockedPanel.SetActive(false);
    }

    protected void OnEnable()
    {
        if (IsBlocked)
            blockedPanel.SetActive(true);
        else
            blockedPanel.SetActive(false);
    }

    public void ItemSetup(Transform parent, Vector2 anchoredPosition, Vector2Int origin, Direction direction)
    {
        transform.SetParent(parent);
        transform.rotation = Quaternion.Euler(0, GetRotationAngle(direction), 0);

        GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        this.origin = origin;
        this.direction = direction;
    }

    public void CreateVisualBackgroundGrid(Transform visualParentTransform, ItemScriptableObject itemTetrisSO, float cellSize)
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

    #endregion

    #region Item Main Functionalities

    public virtual void AddToMainInventory()
    {
        GameManager.instance.GetInventoryManager().SavedItems.Add(this);
    }

    public virtual void RemoveFromMainInventory()
    {
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
        isBlocked = true;

        blockedPanel.SetActive(true);

        if (IsSubclassOfRawGeneric(GetType(), typeof(EquipableItem)))
        {
            EquipableItem equipableItem = GetComponent<EquipableItem>();
            equipableItem.DiscardCurrentWeaponShortcut(true);

            EquipableItem currentEquipedItem = GameManager.instance.GetInventoryManager().EquippedItem;

            if (currentEquipedItem && currentEquipedItem.Equals(equipableItem))
                equipableItem.Unequip();
        }
        else if (IsSubclassOfRawGeneric(GetType(), typeof(AmmoItem)))
        {
            AmmoItem ammoItem = GetComponent<AmmoItem>();
            ammoItem.RemoveFromMainInventory();
        }
    }

    public virtual void UnblockItem()
    {
        isBlocked = false;

        blockedPanel.SetActive(false);

        if (IsSubclassOfRawGeneric(GetType(), typeof(EquipableItem)))
        {
            EquipableItem equipableItem = GetComponent<EquipableItem>();
            int weaponShortcut = equipableItem.WeaponShortcut;

            if (weaponShortcut >= 0)
            {
                GameManager.instance.GetInventoryUI().SetFastSwapCandidate(equipableItem);
                GameManager.instance.GetInventoryUI().SetFastSwapWeapon(weaponShortcut);
            }
            
        }
        else if (IsSubclassOfRawGeneric(GetType(), typeof(AmmoItem)))
        {
            AmmoItem ammoItem = GetComponent<AmmoItem>();
            ammoItem.AddToMainInventory();
        }
    }

    #endregion

    #region Mouse/Keyboard Input Scheme

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !IsBlocked)
        {
            HoldingInventory.SetNewOpenButton(buttonsPanel.gameObject);
            buttonsPanel.gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject openButtonPanel = HoldingInventory.OpenItemButtonPanel;

        if (openButtonPanel != buttonsPanel.gameObject)
        {
            HoldingInventory.SetNewOpenButton(null);
        }
    }

    #endregion
}
