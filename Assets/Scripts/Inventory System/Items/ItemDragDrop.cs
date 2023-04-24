using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Inventory inventoryTetris;
    private Item placedObject;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<Item>();
    }

    public void Setup(Inventory inventoryTetris)
    {
        this.inventoryTetris = inventoryTetris;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;

        placedObject.CreateVisualGrid(transform.GetChild(0), placedObject.GetItemSO() as ItemSO, inventoryTetris.GetGrid().GetCellSize());

        GameManager.instance.GetInventoryDragDropSystem().StartedDragging(inventoryTetris, placedObject);
        //InventoryDragDropSystem.Instance.StartedDragging(inventoryTetris, placedObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameManager.instance.GetInventoryDragDropSystem().StoppedDragging(inventoryTetris, placedObject);
        //InventoryDragDropSystem.Instance.StoppedDragging(inventoryTetris, placedObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
