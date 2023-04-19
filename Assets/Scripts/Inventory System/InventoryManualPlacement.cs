using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManualPlacement : MonoBehaviour
{
    public static InventoryManualPlacement Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private List<ItemSO> placedObjectTypeSOList = null;

    private ItemSO placedObjectTypeSO;
    private ItemSO.Direction dir;
    private Inventory inventoryTetris;
    private RectTransform canvasRectTransform;
    private RectTransform itemContainer;

    private void Awake()
    {
        Instance = this;

        inventoryTetris = GetComponent<Inventory>();

        placedObjectTypeSO = null;

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Try to place
        if (Input.GetMouseButtonDown(0) && placedObjectTypeSO != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 anchoredPosition);

            Vector2Int placedObjectOrigin = inventoryTetris.GetGridPosition(anchoredPosition);

            bool tryPlaceItem = inventoryTetris.TryPlaceItem(placedObjectTypeSO as ItemSO, placedObjectOrigin, dir);

            if (tryPlaceItem)
            {
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Cannot build here
                //TooltipCanvas.ShowTooltip_Static("Cannot Build Here!");
                //FunctionTimer.Create(() => { TooltipCanvas.HideTooltip_Static(); }, 2f, "HideTooltip", true, true);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = ItemSO.GetNextDirection(dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { placedObjectTypeSO = placedObjectTypeSOList[6]; RefreshSelectedObjectType(); }

        //if (Input.GetKeyDown(KeyCode.Alpha8)) { placedObjectTypeSO = placedObjectTypeSOList[7]; RefreshSelectedObjectType(); }
        

        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
    }

    private void DeselectObjectType()
    {
        placedObjectTypeSO = null; 
        RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetCanvasSnappedPosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 anchoredPosition);
        inventoryTetris.GetGrid().GetXY(anchoredPosition, out int x, out int y);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector2 placedObjectCanvas = inventoryTetris.GetGrid().GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * inventoryTetris.GetGrid().GetCellSize();
            return placedObjectCanvas;
        }
        else
        {
            return anchoredPosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, 0, -placedObjectTypeSO.GetRotationAngle(dir));
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public ItemSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }
}
