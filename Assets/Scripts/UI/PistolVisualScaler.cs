using UnityEngine;
using UnityEngine.UI;

public class PistolVisualScaler : MonoBehaviour
{
    [SerializeField] private Image sprite;

    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        float width = rectTransform.sizeDelta.x;
        float height = rectTransform.sizeDelta.y;

        RectTransform spriteRectTransform = sprite.GetComponent<RectTransform>();

        spriteRectTransform.offsetMin = new Vector2(-(width / 4f), spriteRectTransform.offsetMin.y);
        spriteRectTransform.offsetMax = new Vector2((height / 4f), spriteRectTransform.offsetMax.y);
    }
}
