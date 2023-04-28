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
    [SerializeField] private List<Item> testingItemPrefabs = null;

    private Item selectedTestingItemPrefab;
    private Item.Direction dir;
    private Inventory inventoryTetris;
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

        selectedTestingItemPrefab = null;

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();

        playerInput = GameManager.instance.GetPlayer().GetComponent<PlayerInput>();
        input = GameManager.instance.GetPlayer().GetComponent<InputsUI>();
    }

    private void Update()
    {
        if (input.click && selectedTestingItemPrefab != null)
        {
            input.click = false;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, input.point, null, out Vector2 anchoredPosition);

            Vector2Int placedObjectOrigin = inventoryTetris.GetGridPosition(anchoredPosition);

            Item newItem = Instantiate(selectedTestingItemPrefab);

            bool tryPlaceItem = inventoryTetris.TryPlaceItem(newItem, placedObjectOrigin, dir);

            if (tryPlaceItem)
            {
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Cannot build here
                //TooltipCanvas.ShowTooltip_Static("Cannot Build Here!");
                //FunctionTimer.Create(() => { TooltipCanvas.HideTooltip_Static(); }, 2f, "HideTooltip", true, true);

                Destroy(newItem.gameObject);
            }
        }

        if (input.rotate && selectedTestingItemPrefab != null)
        {
            dir = Item.GetNextDirection(dir);
            input.rotate = false;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) { selectedTestingItemPrefab = testingItemPrefabs[0]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) { selectedTestingItemPrefab = testingItemPrefabs[1]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) { selectedTestingItemPrefab = testingItemPrefabs[2]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4)) { selectedTestingItemPrefab = testingItemPrefabs[3]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5)) { selectedTestingItemPrefab = testingItemPrefabs[4]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6)) { selectedTestingItemPrefab = testingItemPrefabs[5]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7)) { selectedTestingItemPrefab = testingItemPrefabs[6]; RefreshSelectedObjectType(); }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha8)) { selectedTestingItemPrefab = testingItemPrefabs[7]; RefreshSelectedObjectType(); }
        

        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
    }

    private void DeselectObjectType()
    {
        selectedTestingItemPrefab = null; 
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

        if (selectedTestingItemPrefab != null)
        {
            Vector2Int rotationOffset = Item.GetRotationOffset(dir, selectedTestingItemPrefab.GetItemSO().width, selectedTestingItemPrefab.GetItemSO().height);
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
        if (selectedTestingItemPrefab != null)
        {
            return Quaternion.Euler(0, 0, -Item.GetRotationAngle(dir));
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public Item GetSelectedTestingItemPrefab()
    {
        return selectedTestingItemPrefab;
    }
}
