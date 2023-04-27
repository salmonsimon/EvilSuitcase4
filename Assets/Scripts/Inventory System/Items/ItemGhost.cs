using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGhost : MonoBehaviour
{
    private RectTransform rectTransform;
    private Transform visual;
    private ItemSO placedObjectTypeSO;

    private InventoryManualPlacement inventoryManualPlacement;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        inventoryManualPlacement = GetComponentInParent<InventoryManualPlacement>(true);
        inventoryManualPlacement.OnSelectedChanged += Instance_OnSelectedChanged;

        RefreshVisual();
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
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
        ItemSO placedObjectTypeSO = null;

        if (placedObject != null)
            placedObjectTypeSO = placedObject.GetItemSO();

        if (placedObjectTypeSO != null)
        {
            visual = Instantiate(placedObjectTypeSO.visual, transform);
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
        }
    }
}
