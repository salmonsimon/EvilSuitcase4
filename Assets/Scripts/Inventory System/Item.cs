using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemSO itemSO;
    private Vector2Int origin;
    private ItemSO.Direction direction;

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
        return itemSO.nameString;
    }

    public ItemSO GetItemSO()
    {
        return itemSO;
    }
}
