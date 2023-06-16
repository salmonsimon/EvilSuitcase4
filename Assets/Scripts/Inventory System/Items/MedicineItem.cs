using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineItem : Item
{
    [SerializeField] private Vector2Int healingRange;

    public override void Discard()
    {
        RemoveFromMainInventory();

        base.Discard();
    }

    public virtual void Use()
    {
        int healthToRecover = Random.Range(healingRange.x, healingRange.y + 1);

        GameManager.instance.GetPlayer().GetComponent<HealthManager>().RecoverHealth(healthToRecover);

        GameManager.instance.GetSFXManager().PlaySound(Config.POTION_SFX);

        Discard();
    }

    public override void RotateInfoPanels()
    {
        RectTransform currentButtonPanel = GetCurrentButtonPanel().GetComponent<RectTransform>();

        switch (direction)
        {
            case Item.Direction.Down:
                
                currentButtonPanel.anchoredPosition = new Vector2((width * cellSize) + 20, (height * cellSize) - 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Left:

                currentButtonPanel.anchoredPosition = new Vector2(20, (height * cellSize) + 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Up:

                currentButtonPanel.anchoredPosition = new Vector2(-20, 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Right:

                currentButtonPanel.anchoredPosition = new Vector2((width * cellSize) - 20, -20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;
        }
    }
}
