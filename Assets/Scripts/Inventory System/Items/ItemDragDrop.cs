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

        placedObject.CreateVisualBackgroundGrid(transform.GetChild(0), placedObject.GetItemSO() as ItemScriptableObject, inventoryTetris.GetGrid().GetCellSize());

        GameManager.instance.GetInventoryDragDropSystem().StartedDragging(inventoryTetris, placedObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameManager.instance.GetInventoryDragDropSystem().StoppedDragging(inventoryTetris, placedObject);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
