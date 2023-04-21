using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmmoItem : Item
{
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
    [SerializeField] protected RectTransform buttonsPanel;
    [SerializeField] private Button discardButton;


    public delegate void OnAmmoAmountChangeDelegate();
    public event OnAmmoAmountChangeDelegate OnAmmoAmountChange;

    protected override void Awake()
    {
        base.Awake();

        discardButton.gameObject.SetActive(false);
        ammoText = ammoTextPanel.GetComponentInChildren<TextMeshProUGUI>();
        UpdateAmmoText();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            discardButton.gameObject.SetActive(true);
            discardButton.Select();
        }
    }

    private void UpdateAmmoText()
    {
        ammoText.text = currentAmmo.ToString();
    }

    public override void RotateInfoPanels()
    {
        switch (direction)
        {
            case ItemSO.Direction.Down:
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth) - 20, 0);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth) + 20, ((height - 1) * cellHeight) - 20);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case ItemSO.Direction.Left:
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth), ((height - 1) * cellHeight) + 30);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(20 - ((height - 1) * cellHeight), ((height - 1) * cellHeight) + 20);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case ItemSO.Direction.Up:
                ammoTextPanel.anchoredPosition = new Vector2(20 - ((height - 1) * cellHeight), ((width - 1) * cellWidth));
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(-(((height - 1) * cellHeight) + 20), 20 - ((height - 1) * cellHeight));
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case ItemSO.Direction.Right:
                ammoTextPanel.anchoredPosition = new Vector2(-((height - 1) * cellHeight), ((height - 1) * cellHeight) - 30);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth) - 20, -(((height - 1) * cellHeight) + 20));
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;
        }
    }
}