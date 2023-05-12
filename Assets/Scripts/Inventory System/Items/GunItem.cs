using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunItem : EquipableItem, IPointerDownHandler
{
    #region Parameters

    [Header("Ammo Related Information")]
    [SerializeField] private AmmoType ammoType;
    public AmmoType AmmoType { get { return ammoType; } }

    #endregion

    #region Variables

    [SerializeField] private int currentAmmo;
    public int CurrentAmmo
    {
        get { return currentAmmo; }
        set
        {
            if (currentAmmo != value)
            {
                currentAmmo = value;
                UpdateAmmoText();
            }
        }
    }

    [SerializeField] private int maxAmmo;

    private TextMeshProUGUI ammoText;

    #endregion

    #region Object References

    [SerializeField] protected RectTransform ammoTextPanel;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        ammoText = ammoTextPanel.GetComponentInChildren<TextMeshProUGUI>();
        UpdateAmmoText();
    }

    #region Getters and Setters

    private void UpdateAmmoText()
    {
        ammoText.text = currentAmmo.ToString();
    }

    #endregion

    #region Item Main Functionalities

    public override void RotateInfoPanels()
    {
        RectTransform currentButtonPanel = GetCurrentButtonPanel().GetComponent<RectTransform>();

        switch (direction)
        {
            case Item.Direction.Down:
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1)*cellSize) - 20, 0);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                currentButtonPanel.anchoredPosition = new Vector2((width * cellSize) + 20, (height * cellSize) - 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Left:
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1) * cellSize), ((height - 1) * cellSize) + 30);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                currentButtonPanel.anchoredPosition = new Vector2(20, (height * cellSize) + 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Up:
                ammoTextPanel.anchoredPosition = new Vector2(-30, ((height - 1) * cellSize) + 50);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                currentButtonPanel.anchoredPosition = new Vector2(-20, 20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Right:
                ammoTextPanel.anchoredPosition = new Vector2(-50, 20);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                currentButtonPanel.anchoredPosition = new Vector2((width * cellSize) - 20, -20);
                currentButtonPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;
        }
    }

    #endregion
}
