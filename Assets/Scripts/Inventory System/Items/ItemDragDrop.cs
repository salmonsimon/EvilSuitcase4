using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup canvasGroup;

    private Inventory inventory;
    private Item placedObject;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<Item>();
    }

    public void Setup(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;

        placedObject.CreateVisualBackgroundGrid(transform.GetChild(0), placedObject.GetItemSO() as ItemScriptableObject);

        GameManager.instance.GetInventoryDragDropSystem().StartedDragging(inventory, placedObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameManager.instance.GetInventoryDragDropSystem().StoppedDragging(inventory, placedObject);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
