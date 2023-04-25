using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Windows;
#endif

public class InventoryManualPlacement : MonoBehaviour
{
    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private List<ItemSO> placedObjectTypeSOList = null;

    private ItemSO placedObjectTypeSO;
    private Item.Direction dir;
    private Inventory inventoryTetris;
    private RectTransform canvasRectTransform;
    private RectTransform itemContainer;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput playerInput;
    private InputsUI input;
#endif

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            return playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    private void Awake()
    {
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

        playerInput = GameManager.instance.GetPlayer().GetComponent<PlayerInput>();
        input = GameManager.instance.GetPlayer().GetComponent<InputsUI>();
    }

    private void Update()
    {
        // Try to place
        //if (Input.GetMouseButtonDown(0) && placedObjectTypeSO != null)
        if (input.click && placedObjectTypeSO != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, input.point, null, out Vector2 anchoredPosition);

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

        //if (Input.GetKeyDown(KeyCode.R))
        if (input.rotate && placedObjectTypeSO != null)
        {
            dir = Item.GetNextDirection(dir);
            input.rotate = false;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7)) { placedObjectTypeSO = placedObjectTypeSOList[6]; RefreshSelectedObjectType(); }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha8)) { placedObjectTypeSO = placedObjectTypeSOList[7]; RefreshSelectedObjectType(); }
        

        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, input.point, null, out Vector2 anchoredPosition);
        inventoryTetris.GetGrid().GetXY(anchoredPosition, out int x, out int y);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = Item.GetRotationOffset(dir, placedObjectTypeSO.width, placedObjectTypeSO.height);
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
            return Quaternion.Euler(0, 0, -Item.GetRotationAngle(dir));
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
