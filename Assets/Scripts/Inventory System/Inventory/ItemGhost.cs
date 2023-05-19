using UnityEngine;

public class ItemGhost : MonoBehaviour
{
    #region Object References

    private RectTransform rectTransform;
    private InventoryManualPlacement inventoryManualPlacement;

    #endregion

    #region Variables

    private Transform visual;

    #endregion

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        inventoryManualPlacement = GetComponentInParent<InventoryManualPlacement>(true);
        inventoryManualPlacement.OnSelectedChanged += OnSelectedChanged;

        RefreshVisual();
    }

    private void OnSelectedChanged(object sender, System.EventArgs e)
    {
        RefreshVisual();
    }

    private void LateUpdate()
    {
        Vector2 targetPosition = inventoryManualPlacement.GetCanvasSnappedPosition();

        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.fixedDeltaTime * 15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, inventoryManualPlacement.GetPlacedObjectRotation(), Time.fixedDeltaTime * 15f);
    }

    private void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        Item placedObject = inventoryManualPlacement.GetSelectedTestingItemPrefab();
        ItemScriptableObject placedObjectTypeSO = null;

        if (placedObject != null)
            placedObjectTypeSO = placedObject.GetItemSO();

        if (placedObjectTypeSO != null)
        {
            visual = Instantiate(placedObjectTypeSO.ItemVisual, transform);

            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;

            float cellSize = inventoryManualPlacement.Inventory.CellSize;
            float visualWidth = cellSize * placedObjectTypeSO.Width;
            float visualHeight = cellSize * placedObjectTypeSO.Height;

            visual.GetComponent<RectTransform>().sizeDelta = new Vector2(visualWidth, visualHeight);

            visual.gameObject.SetActive(false);
            visual.gameObject.SetActive(true);
        }
    }
}
