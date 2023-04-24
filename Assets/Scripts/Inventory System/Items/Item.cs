using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected ItemSO itemSO;
    protected ItemSO.Direction direction;
    protected Vector2Int origin;

    protected float width;
    protected float height;
    protected float cellWidth;
    protected float cellHeight;


    protected virtual void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        cellWidth = rectTransform.sizeDelta.x;
        cellHeight = rectTransform.sizeDelta.y;

        width = itemSO.width;
        height = itemSO.height;
    }

    public static Item Create(Vector3 worldPosition, Vector2Int origin, ItemSO.Direction direction, ItemSO placedItemSO)
    {
        Transform placedItemTransform = Instantiate(placedItemSO.prefab, worldPosition, Quaternion.Euler(0, placedItemSO.GetRotationAngle(direction), 0));

        Item placedItem = placedItemTransform.GetComponent<Item>();
        placedItem.itemSO = placedItemSO;
        placedItem.origin = origin;
        placedItem.direction = direction;

        return placedItem;
    }

    public static Item CreateCanvas(Transform parent, Vector2 anchoredPosition, Vector2Int origin, ItemSO.Direction direction, ItemSO placedItemSO)
    {
        Transform placedObjectTransform = Instantiate(placedItemSO.prefab, parent);
        placedObjectTransform.rotation = Quaternion.Euler(0, placedItemSO.GetRotationAngle(direction), 0);
        placedObjectTransform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        Item placedItem = placedObjectTransform.GetComponent<Item>();
        placedItem.itemSO = placedItemSO;
        placedItem.origin = origin;
        placedItem.direction = direction;

        return placedItem;
    }

    public Vector2Int GetGridPosition()
    {
        return origin;
    }

    public void SetOrigin(Vector2Int origin)
    {
        this.origin = origin;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return itemSO.GetGridPositionList(origin, direction);
    }

    public ItemSO.Direction GetDirection()
    {
        return direction;
    }

    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }

    public override string ToString()
    {
        return itemSO.itemName;
    }

    public ItemSO GetItemSO()
    {
        return itemSO;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            Debug.Log("Pressed right button on " + gameObject.name + " gameobject");
    }

    public virtual void Discard()
    {
        Debug.Log("Discarded Item: " + gameObject.name);
        Destroy(gameObject);
    }

    public virtual void RotateInfoPanels()
    {

    }
}
