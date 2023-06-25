using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmmoItem : Item
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
    public int MaxAmmo { get { return maxAmmo; } }

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

    public override Item RewardItemSetup(RewardItem rewardItem)
    {
        CurrentAmmo = Random.Range(rewardItem.AmmoMinMax.x, rewardItem.AmmoMinMax.y + 1);

        return this;
    }

    public override void RemoveFromMainInventory()
    {
        base.RemoveFromMainInventory();

        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        inventoryManager.AmmoItemListDictionary[ammoType].Remove(this);

        inventoryManager.StockedAmmoDictionary[ammoType] -= currentAmmo;

        if (inventoryManager.StockedAmmoDictionary[ammoType] < 0)
            inventoryManager.StockedAmmoDictionary[ammoType] = 0;

        inventoryManager.StockedAmmoChange();
    }

    public override void AddToMainInventory()
    {
        base.AddToMainInventory();

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

    public void FillCurrentStockedAmmoWithNewAmmoItem()
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        if (inventoryManager.AmmoItemListDictionary[ammoType].Any())
        {
            List<AmmoItem> inventoryAmmoItemList = inventoryManager.AmmoItemListDictionary[ammoType];
            AmmoItem lastAmmoItemInInventoryList = inventoryAmmoItemList[inventoryAmmoItemList.Count - 1];

            if (lastAmmoItemInInventoryList.CurrentAmmo == lastAmmoItemInInventoryList.MaxAmmo)
                return;
            else
            {
                int maxFillAmount = Mathf.Min(lastAmmoItemInInventoryList.MaxAmmo, CurrentAmmo);
                int bulletsToFillAmmoItem = lastAmmoItemInInventoryList.MaxAmmo - lastAmmoItemInInventoryList.CurrentAmmo;
                int fillAmount = Mathf.Min(maxFillAmount, bulletsToFillAmmoItem);

                lastAmmoItemInInventoryList.CurrentAmmo += fillAmount;

                inventoryAmmoItemList[inventoryAmmoItemList.Count - 1] = lastAmmoItemInInventoryList;

                CurrentAmmo -= fillAmount;

                GameManager.instance.GetSFXManager().PlaySound(Config.DROP_SFX);
            }
        }
    }

    public override void Discard()
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        RemoveFromMainInventory();

        Destroy(gameObject);
    }

    public override void RotateInfoPanels()
    {
        RectTransform currentButtonPanel = GetCurrentButtonPanel().GetComponent<RectTransform>();

        switch (direction)
        {
            case Item.Direction.Down:
                ammoTextPanel.anchoredPosition = new Vector2(((width - 1) * cellSize) - 20, 0);
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
