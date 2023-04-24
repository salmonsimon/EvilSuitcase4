using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item Scriptable Object", order = 0)]
public class ItemSO : ScriptableObject
{
    public enum Direction
    {
        Down,
        Left,
        Up,
        Right,
    }

    public string itemName;
    public Transform prefab;
    public Transform visual;
    public Transform gridVisual;
    public int width;
    public int height;

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

    public static Vector2Int GetDirectionForwardVector(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return new Vector2Int(0, -1);
            case Direction.Left: return new Vector2Int(-1, 0);
            case Direction.Up: return new Vector2Int(0, +1);
            case Direction.Right: return new Vector2Int(+1, 0);
        }
    }

    public static Direction GetDirection(Vector2Int from, Vector2Int to)
    {
        if (from.x < to.x)
        {
            return Direction.Right;
        }
        else
        {
            if (from.x > to.x)
            {
                return Direction.Left;
            }
            else
            {
                if (from.y < to.y)
                {
                    return Direction.Up;
                }
                else
                {
                    return Direction.Down;
                }
            }
        }
    }

    public int GetRotationAngle(Direction direction)
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

    public Vector2Int GetRotationOffset(Direction direction)
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

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction direction)
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
        Transform visualTransform = Instantiate(gridVisual, visualParentTransform);

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
