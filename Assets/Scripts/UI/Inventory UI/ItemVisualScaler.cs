using UnityEngine;
using UnityEngine.UI;

public class ItemVisualScaler : MonoBehaviour
{
    [SerializeField] private Image sprite;

    [SerializeField] private float leftMultiplier;
    [SerializeField] private float rightMultiplier;
    [SerializeField] private float topMultiplier;
    [SerializeField] private float bottomMultiplier;

    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        float width = rectTransform.sizeDelta.x;
        float height = rectTransform.sizeDelta.y;

        RectTransform spriteRectTransform = sprite.GetComponent<RectTransform>();

        spriteRectTransform.offsetMin = new Vector2(width * leftMultiplier, height * bottomMultiplier);
        spriteRectTransform.offsetMax = new Vector2(-width * rightMultiplier, - height * topMultiplier);
    }
}
