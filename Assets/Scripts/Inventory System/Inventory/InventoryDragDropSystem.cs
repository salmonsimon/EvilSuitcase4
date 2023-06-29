using System.Collections.Generic;
using UnityEngine;

public class InventoryDragDropSystem : MonoBehaviour
{
    #region Object References

    [SerializeField] private List<Inventory> pauseInventoryList;
    [SerializeField] private List<Inventory> rewardsInventoryList;

#if ENABLE_INPUT_SYSTEM
    private InputsUI input;
#endif

    #endregion

    #region Variables

    private Inventory draggingInventory;

    private Item draggingItem;
    public Item DraggingItem { get { return draggingItem; } }

    private Vector2Int mouseDragGridPositionOffset;
    private Vector2 mouseDragAnchoredPositionOffset;
    private Item.Direction direction;

    #endregion

    private void Start()
    {
        foreach (Inventory inventory in pauseInventoryList)
            inventory.OnItemPlaced += (object sender, Item placedObject) => {};

        foreach (Inventory inventory in rewardsInventoryList)
            inventory.OnItemPlaced += (object sender, Item placedObject) => { };

        input = GameManager.instance.GetPlayer().GetComponent<InputsUI>();
    }

    private void Update()
    {
        if (input.rotate && draggingItem != null)
        {
            direction = Item.GetNextDirection(direction);
            GameManager.instance.GetSFXManager().PlaySound(Config.ROTATE_SFX);
            input.rotate = false;
        }

        if (draggingItem != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(draggingInventory.GetItemContainer(), input.point, null, out Vector2 targetPosition);
            targetPosition += new Vector2(-mouseDragAnchoredPositionOffset.x, -mouseDragAnchoredPositionOffset.y);

            Vector2Int rotationOffset = Item.GetRotationOffset(direction, draggingItem.GetItemSO().Width, draggingItem.GetItemSO().Height);
            targetPosition += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventory.GetGrid().GetCellSize();

            targetPosition /= 10f; // draggingInventory.CellSize; //10f;
            targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
            targetPosition *= 10f; //draggingInventory.CellSize; //10f;

            draggingItem.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(draggingItem.GetComponent<RectTransform>().anchoredPosition, targetPosition, Time.fixedDeltaTime * 15f);
            draggingItem.transform.rotation = Quaternion.Lerp(draggingItem.transform.rotation, Quaternion.Euler(0, 0, -Item.GetRotationAngle(direction)), Time.fixedDeltaTime * 15f);
        }
    }

    public void StartedDragging(Inventory inventory, Item item)
    {
        draggingInventory = inventory;
        draggingItem = item;

        Cursor.visible = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.GetItemContainer(), input.point, null, out Vector2 anchoredPosition);
        Vector2Int mouseGridPosition = inventory.GetGridPosition(anchoredPosition);

        mouseDragGridPositionOffset = mouseGridPosition - item.GetGridPosition();

        mouseDragAnchoredPositionOffset = anchoredPosition - item.GetComponent<RectTransform>().anchoredPosition;

        direction = item.GetDirection();

        Vector2Int rotationOffset = Item.GetRotationOffset(direction, item.GetItemSO().Width, item.GetItemSO().Height);
        mouseDragAnchoredPositionOffset += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventory.GetGrid().GetCellSize();
    }

    public void StoppedDragging(Inventory fromInventory, Item item)
    {
        draggingInventory = null;
        draggingItem = null;

        Cursor.visible = true;

        fromInventory.RemoveItemAt(item.GetGridPosition());

        if (fromInventory.MainInventory)
            item.RemoveFromMainInventory();

        Inventory toInventory = null;

        List<Inventory> inventoriesToCheck = GameManager.instance.IsOnRewardsUI ? rewardsInventoryList : pauseInventoryList;

        foreach (Inventory inventory in inventoriesToCheck)
        {
            Vector3 screenPoint = input.point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);

            Vector2Int itemOrigin = inventory.GetGridPosition(anchoredPosition);

            itemOrigin = itemOrigin - mouseDragGridPositionOffset;

            if (inventory.IsValidGridPosition(itemOrigin))
            {
                toInventory = inventory;
                break;
            }
        }

        if (toInventory != null)
        {
            Vector3 screenPoint = input.point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInventory.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);
            Vector2Int itemOrigin = toInventory.GetGridPosition(anchoredPosition);
            itemOrigin = itemOrigin - mouseDragGridPositionOffset;

            bool tryPlaceItem = toInventory.TryPlaceItem(item, itemOrigin, direction);

            if (!tryPlaceItem)
                fromInventory.TryPlaceItem(item, item.GetGridPosition(), item.GetDirection());
        }
        else
            fromInventory.TryPlaceItem(item, item.GetGridPosition(), item.GetDirection());
    }
}
