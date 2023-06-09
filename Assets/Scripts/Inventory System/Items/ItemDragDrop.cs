using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

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

        GetComponent<Canvas>().overrideSorting = true;
        GetComponent<Canvas>().sortingOrder = 10000;
        placedObject.HoldingInventory.SetNewOpenButton(null);

        placedObject.CreateVisualBackgroundGrid(transform.GetChild(0), placedObject.GetItemSO() as ItemScriptableObject);

        GameManager.instance.GetInventoryDragDropSystem().StartedDragging(inventory, placedObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GetComponent<Canvas>().overrideSorting = false;

        GameManager.instance.GetInventoryDragDropSystem().StoppedDragging(inventory, placedObject);
        GameManager.instance.GetSFXManager().PlaySound(Config.DROP_SFX);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.instance.GetSFXManager().PlaySound(Config.PICKUP_SFX);
    }
}
