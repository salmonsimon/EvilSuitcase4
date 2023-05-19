using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeleeItem : EquipableItem
{
    #region Variables

    [SerializeField] private float currentDurability;
    public float CurrentDurability
    {
        get { return currentDurability; }
        set
        {
            if (currentDurability != value)
            {
                currentDurability = value;
                UpdateDurabilityText();
            }
        }
    }

    private TextMeshProUGUI durabilityText;

    #endregion

    #region Object References

    [SerializeField] protected RectTransform durabilityTextPanel;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        durabilityText = durabilityTextPanel.GetComponentInChildren<TextMeshProUGUI>();
        UpdateDurabilityText();
    }

    public override Item RewardItemSetup(RewardItem rewardItem)
    {
        CurrentDurability = Random.Range(rewardItem.DurabilityMinMax.x, rewardItem.DurabilityMinMax.y);

        return this;
    }

    protected void UpdateDurabilityText()
    {
        int currentDurabilityInt = Mathf.RoundToInt(currentDurability * 100);
        string currentDurabilityText = ((currentDurabilityInt < 10) ? "0" : "") + currentDurabilityInt + "%";

        durabilityText.text = currentDurabilityText;
    }

    public override void RotateInfoPanels()
    {
        RectTransform currentButtonPanel = GetCurrentButtonPanel().GetComponent<RectTransform>();

        switch (direction)
        {
            case Item.Direction.Down:
                durabilityTextPanel.anchoredPosition = new Vector2(((width - 1) * cellSize) - 30, 0);
                durabilityTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                currentButtonPanel.anchoredPosition = new Vector2((width * cellSize) + 20, (height * cellSize) - 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Left:
                durabilityTextPanel.anchoredPosition = new Vector2(((width - 1) * cellSize), ((height - 1) * cellSize) + 20);
                durabilityTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                currentButtonPanel.anchoredPosition = new Vector2(20, (height * cellSize) + 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Up:
                durabilityTextPanel.anchoredPosition = new Vector2(-20, ((height - 1) * cellSize) + 50);
                durabilityTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                currentButtonPanel.anchoredPosition = new Vector2(-20, 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Right:
                durabilityTextPanel.anchoredPosition = new Vector2(-50, 30);
                durabilityTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                currentButtonPanel.anchoredPosition = new Vector2((width * cellSize) - 20, -20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;
        }
    }
}
