using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplayUI : MonoBehaviour
{
    [SerializeField] private Image bulletImage;
    [SerializeField] private TextMeshProUGUI ammoText;

    private AmmoType currentAmmoType;

    private void OnEnable()
    {
        GameManager.instance.GetInventoryManager().OnStockedAmmoChange += UpdateStockedAmmoCounter;
    }

    private void OnDisable()
    {
        GameManager.instance.GetInventoryManager().OnStockedAmmoChange -= UpdateStockedAmmoCounter;
    }

    public void Setup(Sprite bulletSprite, int currentClipAmmo)
    {
        bulletImage.sprite = bulletSprite;

        GunItem equippedItem = (GunItem)GameManager.instance.GetInventoryManager().EquippedItem;
        currentAmmoType = equippedItem.AmmoType;

        int currentStockedAmmo = GameManager.instance.GetInventoryManager().StockedAmmoDictionary[equippedItem.AmmoType];

        UpdateCounters(currentClipAmmo, currentStockedAmmo);
    }

    public void UpdateCounters(int currentClipAmmo, int currentStockedAmmo)
    {
        string currentClipAmmoText = ((currentClipAmmo < 10) ? "0" : "") + currentClipAmmo;
        string currentStockedAmmoText = ((currentStockedAmmo < 10) ? "0" : "") + currentStockedAmmo;

        string ammoText = currentClipAmmoText + '/' + currentStockedAmmoText;
        this.ammoText.SetText(ammoText);

        int fastSwapIndex = GameManager.instance.GetInventoryManager().FastSwapIndex;
        GameManager.instance.GetInventoryUI().LoadFastSwapGameplayPanel(fastSwapIndex);
    }

    public void UpdateStockedAmmoCounter()
    {
        int currentStockedAmmo = GameManager.instance.GetInventoryManager().StockedAmmoDictionary[currentAmmoType];

        string currentStockedAmmoText = ((currentStockedAmmo < 10) ? "0" : "") + currentStockedAmmo;

        string ammoText = this.ammoText.text.Split("/")[0] + '/' + currentStockedAmmoText;
        this.ammoText.SetText(ammoText);

        int fastSwapIndex = GameManager.instance.GetInventoryManager().FastSwapIndex;
        GameManager.instance.GetInventoryUI().LoadFastSwapGameplayPanel(fastSwapIndex);
    }
}
