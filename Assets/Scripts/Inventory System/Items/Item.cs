using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerClickHandler
{
    public enum Direction
    {
        Down,
        Left,
        Up,
        Right,
    }

    [SerializeField] protected ItemSO itemSO;
    protected Direction direction;
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

    public static Item CreateCanvas(Transform parent, Vector2 anchoredPosition, Vector2Int origin, Direction direction, ItemSO placedItemSO)
    {
        Transform placedObjectTransform = Instantiate(placedItemSO.prefab, parent);
        placedObjectTransform.rotation = Quaternion.Euler(0, GetRotationAngle(direction), 0);
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
        return GetGridPositionList(origin, direction, itemSO.width, itemSO.height);
    }

    public Direction GetDirection()
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

    public ItemSO.ItemType GetItemType()
    {
        return itemSO.itemType;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            Debug.Log("Pressed right button on " + gameObject.name + " gameobject");
    }

    public virtual void Discard()
    {
        Destroy(gameObject);
    }

    public virtual void AddToMainInventory()
    {

    }

    public virtual void RotateInfoPanels()
    {

    }

    public static Direction GetNextDirection(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return Direction.Left;
            case Direction.Left: return Direction.Up;
            case Direction.Up: return Direction.Right;
            case Direction.Right: return Direction.Down;
        }
    }

    public static int GetRotationAngle(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return 0;
            case Direction.Left: return 90;
            case Direction.Up: return 180;
            case Direction.Right: return 270;
        }
    }

    public static Vector2Int GetRotationOffset(Direction direction, int width, int height)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return new Vector2Int(0, 0);
            case Direction.Left: return new Vector2Int(0, width);
            case Direction.Up: return new Vector2Int(width, height);
            case Direction.Right: return new Vector2Int(height, 0);
        }
    }

    public static List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction direction, int width, int height)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (direction)
        {
            default:
            case Direction.Down:
            case Direction.Up:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Direction.Left:
            case Direction.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

    public void CreateVisualGrid(Transform visualParentTransform, ItemSO itemTetrisSO, float cellSize)
    {
        Transform visualTransform = Instantiate(itemTetrisSO.gridVisual, visualParentTransform);

        // Create background
        Transform template = visualTransform.Find("Template");
        template.gameObject.SetActive(false);

        for (int x = 0; x < itemTetrisSO.width; x++)
        {
            for (int y = 0; y < itemTetrisSO.height; y++)
            {
                Transform backgroundSingleTransform = Instantiate(template, visualTransform);
                backgroundSingleTransform.gameObject.SetActive(true);
            }
        }

        visualTransform.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * cellSize;

        visualTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(itemTetrisSO.width, itemTetrisSO.height) * cellSize;

        visualTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        visualTransform.SetAsFirstSibling();
    }


}
