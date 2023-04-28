using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmmoItem : Item
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
    public int MaxAmmo { get { return maxAmmo; } }


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

    public override void RemoveFromMainInventory()
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        inventoryManager.AmmoItemListDictionary[ammoType].Remove(this);

        inventoryManager.StockedAmmoDictionary[ammoType] -= currentAmmo;
        inventoryManager.StockedAmmoChange();
    }

    public override void AddToMainInventory()
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        if (!inventoryManager.StockedAmmoDictionary.ContainsKey(ammoType))
        {
            inventoryManager.StockedAmmoDictionary.Add(ammoType, currentAmmo);

            List<AmmoItem> newAmmoItemList = new List<AmmoItem>();
            newAmmoItemList.Add(this);

            inventoryManager.AmmoItemListDictionary.Add(ammoType, newAmmoItemList);
        }
        else
        {
            inventoryManager.StockedAmmoDictionary[ammoType] += currentAmmo;

            if (inventoryManager.AmmoItemListDictionary[ammoType].Any())
            {
                List<AmmoItem> inventoryAmmoItemList = inventoryManager.AmmoItemListDictionary[ammoType];
                AmmoItem lastAmmoItemInInventoryList = inventoryAmmoItemList[inventoryAmmoItemList.Count - 1];

                if (CurrentAmmo == MaxAmmo)
                {
                    inventoryAmmoItemList[inventoryAmmoItemList.Count - 1] = this;
                    inventoryAmmoItemList.Add(lastAmmoItemInInventoryList);

                    return;
                }

                int maxFillAmount = Mathf.Min(lastAmmoItemInInventoryList.MaxAmmo, CurrentAmmo);
                int bulletsToFillAmmoItem = lastAmmoItemInInventoryList.MaxAmmo - lastAmmoItemInInventoryList.CurrentAmmo;
                int fillAmount = Mathf.Min(maxFillAmount, bulletsToFillAmmoItem);

                lastAmmoItemInInventoryList.CurrentAmmo += fillAmount;

                inventoryAmmoItemList[inventoryAmmoItemList.Count - 1] = lastAmmoItemInInventoryList;

                CurrentAmmo -= fillAmount;

                if (CurrentAmmo > 0)
                    inventoryAmmoItemList.Add(this);
                else
                    this.Discard();
            }
            else
            {
                inventoryManager.AmmoItemListDictionary[ammoType].Add(this);
            }
        }

        inventoryManager.StockedAmmoChange();
    }

    public override void Discard()
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        inventoryManager.StockedAmmoDictionary[ammoType] -= currentAmmo;
        inventoryManager.StockedAmmoChange();

        Destroy(gameObject);
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
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth) - 20, 0);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth) + 20, ((height - 1) * cellHeight) - 20);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Left:
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1) * cellWidth), 30);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(-30, ((height - 1) * cellHeight) + 20);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Up:
                ammoTextPanel.anchoredPosition = new Vector2(-30, 50);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(-70, -30);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Item.Direction.Right:
                ammoTextPanel.anchoredPosition = new Vector2(-50, 20);
                ammoTextPanel.rotation = Quaternion.Euler(0, 0, 0);

                buttonsPanel.anchoredPosition = new Vector2(30, -70);
                buttonsPanel.rotation = Quaternion.Euler(0, 0, 0);

                break;
        }
    }
}
