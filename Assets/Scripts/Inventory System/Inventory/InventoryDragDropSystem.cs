using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryDragDropSystem : MonoBehaviour
{
    public static InventoryDragDropSystem Instance { get; private set; }

    [SerializeField] private List<Inventory> inventoryList;

    private Inventory draggingInventory;
    private Item draggingItem;
    private Vector2Int mouseDragGridPositionOffset;
    private Vector2 mouseDragAnchoredPositionOffset;
    private Item.Direction direction;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput playerInput;
    private InputsUI input;
#endif

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Inventory inventoryTetris in inventoryList)
            inventoryTetris.OnItemPlaced += (object sender, Item placedObject) => {};

        playerInput = GameManager.instance.GetPlayer().GetComponent<PlayerInput>();
        input = GameManager.instance.GetPlayer().GetComponent<InputsUI>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        if (input.rotate && draggingItem != null)
        {
            direction = Item.GetNextDirection(direction);
            input.rotate = false;
        }

        if (draggingItem != null)
        {
            // Calculate target position to move the dragged item
            RectTransformUtility.ScreenPointToLocalPointInRectangle(draggingInventory.GetItemContainer(), input.point, null, out Vector2 targetPosition);
            targetPosition += new Vector2(-mouseDragAnchoredPositionOffset.x, -mouseDragAnchoredPositionOffset.y);

            // Apply rotation offset to target position
            Vector2Int rotationOffset = Item.GetRotationOffset(direction, draggingItem.GetItemSO().width, draggingItem.GetItemSO().height);
            targetPosition += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventory.GetGrid().GetCellSize();

            // Snap position
            targetPosition /= 10f;// draggingInventoryTetris.GetGrid().GetCellSize();
            targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
            targetPosition *= 10f;

            // Move and rotate dragged object
            draggingItem.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(draggingItem.GetComponent<RectTransform>().anchoredPosition, targetPosition, Time.deltaTime * 20f);
            draggingItem.transform.rotation = Quaternion.Lerp(draggingItem.transform.rotation, Quaternion.Euler(0, 0, -Item.GetRotationAngle(direction)), Time.deltaTime * 15f);
        }
    }

    public void StartedDragging(Inventory inventory, Item item)
    {
        // Started Dragging
        draggingInventory = inventory;
        draggingItem = item;

        Cursor.visible = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.GetItemContainer(), input.point, null, out Vector2 anchoredPosition);
        Vector2Int mouseGridPosition = inventory.GetGridPosition(anchoredPosition);

        // Calculate Grid Position offset from the placedObject origin to the mouseGridPosition
        mouseDragGridPositionOffset = mouseGridPosition - item.GetGridPosition();

        // Calculate the anchored poisiton offset, where exactly on the image the player clicked
        mouseDragAnchoredPositionOffset = anchoredPosition - item.GetComponent<RectTransform>().anchoredPosition;

        // Save initial direction when started draggign
        direction = item.GetDirection();

        // Apply rotation offset to drag anchored position offset
        Vector2Int rotationOffset = Item.GetRotationOffset(direction, item.GetItemSO().width, item.GetItemSO().height);
        mouseDragAnchoredPositionOffset += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventory.GetGrid().GetCellSize();
    }

    public void StoppedDragging(Inventory fromInventory, Item item)
    {
        draggingInventory = null;
        draggingItem = null;

        Cursor.visible = true;

        // Remove item from its current inventory
        fromInventory.RemoveItemAt(item.GetGridPosition());

        Inventory toInventory = null;

        // Find out which InventoryTetris is under the mouse position
        foreach (Inventory inventory in inventoryList)
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

        // Check if it's on top of a InventoryTetris
        if (toInventory != null)
        {
            Vector3 screenPoint = input.point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInventory.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);
            Vector2Int itemOrigin = toInventory.GetGridPosition(anchoredPosition);
            itemOrigin = itemOrigin - mouseDragGridPositionOffset;

            bool tryPlaceItem = toInventory.TryPlaceItem(item.GetItemSO() as ItemSO, itemOrigin, direction);

            if (tryPlaceItem)
            {
                // Item placed!
            }
            else
            {
                // Cannot drop item here!
                //TooltipCanvas.ShowTooltip_Static("Cannot Drop Item Here!");
                //FunctionTimer.Create(() => { TooltipCanvas.HideTooltip_Static(); }, 2f, "HideTooltip", true, true);

                // Drop on original position
                fromInventory.TryPlaceItem(item.GetItemSO() as ItemSO, item.GetGridPosition(), item.GetDirection());
            }
        }
        else
        {
            // Not on top of any Inventory Tetris!

            // Cannot drop item here!
            //TooltipCanvas.ShowTooltip_Static("Cannot Drop Item Here!");
            //FunctionTimer.Create(() => { TooltipCanvas.HideTooltip_Static(); }, 2f, "HideTooltip", true, true);

            // Drop on original position
            fromInventory.TryPlaceItem(item.GetItemSO() as ItemSO, item.GetGridPosition(), item.GetDirection());
        }
    }
}
