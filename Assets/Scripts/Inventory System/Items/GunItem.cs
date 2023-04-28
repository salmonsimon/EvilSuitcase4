using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunItem : EquipableItem, IPointerDownHandler
{
    [SerializeField] private AmmoType ammoType;
    public AmmoType AmmoType { get { return ammoType; } }

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

                if (OnAmmoAmountChange != null)
                    OnAmmoAmountChange();
            }
        }
    }

    [SerializeField] private int maxAmmo;

    private TextMeshProUGUI ammoText;

    [SerializeField] protected RectTransform ammoTextPanel;

    public delegate void OnAmmoAmountChangeDelegate();
    public event OnAmmoAmountChangeDelegate OnAmmoAmountChange;

    protected override void Awake()
    {
        base.Awake();

        buttonsPanel.gameObject.SetActive(false);
        ammoText = ammoTextPanel.GetComponentInChildren<TextMeshProUGUI>();
        UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        ammoText.text = currentAmmo.ToString();
    }

    public override void RotateInfoPanels()
    {
        switch (direction)
        {
            case Item.Direction.Down:
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1)*cellWidth) - 20, 0);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth) + 20, ((height - 1) * cellHeight) - 20);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Left:
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth), ((height - 1) * cellHeight) + 30);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(-30, ((height - 1) * cellHeight) + 20);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Up:
                ammoTextPanel.anchoredPosition = new Vector2(-30, ((height - 1) * cellHeight) + 50);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(-70, -30);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Right:
                ammoTextPanel.anchoredPosition = new Vector2(-50, 20);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth) - 20, -70);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;
        }
    }
}
