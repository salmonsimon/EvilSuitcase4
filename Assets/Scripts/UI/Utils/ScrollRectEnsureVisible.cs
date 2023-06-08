using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectEnsureVisible : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform scrollRectTransform;
    RectTransform contentPanel;
    RectTransform selectedRectTransform;
    GameObject lastSelected;

    Vector2 targetPos;

    [SerializeField] private bool hasOffset = false;
    [SerializeField] private float offset = 0f;

    List<Selectable> allDescendants;

    void Start()
    {
        scrollRectTransform = GetComponent<RectTransform>();

        if (contentPanel == null)
            contentPanel = GetComponent<ScrollRect>().content;

        targetPos = contentPanel.anchoredPosition;

        allDescendants = contentPanel.transform.GetComponentsInChildren<Selectable>().ToList();
    }

    void Update()
    {
        if (!_mouseHover)
            Autoscroll();
    }


    public void Autoscroll()
    {
        if (contentPanel == null)
            contentPanel = GetComponent<ScrollRect>().content;

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null)
        {
            return;
        }
        if (selected == lastSelected)
        {
            return;
        }
        if (!IsDescendantOf(selected.GetComponent<Selectable>()))
        {
            return;
        }

        selectedRectTransform = (RectTransform)selected.transform;

        targetPos.x = contentPanel.anchoredPosition.x;
        targetPos.y = -(selectedRectTransform.localPosition.y) - (selectedRectTransform.rect.height / 2);

        if (selected.transform.parent != contentPanel.transform)
        {
            float yOffset = 0;

            Transform parent = selected.transform.parent;

            while (parent != contentPanel.transform)
            {
                yOffset -= parent.localPosition.y;

                parent = parent.parent;
            }

            targetPos.y += yOffset;
        }

        if (hasOffset)
            targetPos.y += offset;

        targetPos.y = Mathf.Clamp(targetPos.y, 0, contentPanel.sizeDelta.y - scrollRectTransform.sizeDelta.y);

        contentPanel.anchoredPosition = targetPos;
        lastSelected = selected;
    }


    bool _mouseHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mouseHover = false;
    }

    private bool IsDescendantOf(Selectable possibleDescendant)
    {
        return allDescendants.Contains(possibleDescendant);
    }
}