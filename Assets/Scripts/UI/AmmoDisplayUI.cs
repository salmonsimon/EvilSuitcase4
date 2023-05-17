using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplayUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI weaponText;

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
        weaponImage.sprite = bulletSprite;

        GunItem equippedItem = (GunItem)GameManager.instance.GetInventoryManager().EquippedItem;
        currentAmmoType = equippedItem.AmmoType;

        int currentStockedAmmo = GameManager.instance.GetInventoryManager().StockedAmmoDictionary[equippedItem.AmmoType];

        UpdateCounters(currentClipAmmo, currentStockedAmmo);
    }

    public void Setup(Sprite meleeWeaponSprite)
    {
        weaponImage.sprite = meleeWeaponSprite;

        MeleeItem equippedItem = (MeleeItem)GameManager.instance.GetInventoryManager().EquippedItem;
        float currentDurability = equippedItem.CurrentDurability;

        string weaponText = Mathf.RoundToInt((currentDurability * 100)) + "% / 100%";
        this.weaponText.SetText(weaponText);
    }

    public void UpdateCounters(int currentClipAmmo, int currentStockedAmmo)
    {
        string currentClipAmmoText = ((currentClipAmmo < 10) ? "0" : "") + currentClipAmmo;
        string currentStockedAmmoText = ((currentStockedAmmo < 10) ? "0" : "") + currentStockedAmmo;

        string ammoText = currentClipAmmoText + '/' + currentStockedAmmoText;
        this.weaponText.SetText(ammoText);

        int fastSwapIndex = GameManager.instance.GetInventoryManager().FastSwapIndex;
        GameManager.instance.GetInventoryUI().LoadFastSwapGameplayPanel(fastSwapIndex);
    }

    public void UpdateCounters(float currentDurability)
    {
        int currentDurabilityInt = Mathf.RoundToInt(currentDurability * 100);

        string currentDurabilityText = ((currentDurabilityInt < 10) ? "0" : "") + currentDurabilityInt;

        string durabilityText = currentDurabilityText + "% / 100%";
        this.weaponText.SetText(durabilityText);

        int fastSwapIndex = GameManager.instance.GetInventoryManager().FastSwapIndex;
        GameManager.instance.GetInventoryUI().LoadFastSwapGameplayPanel(fastSwapIndex);
    }

    public void UpdateStockedAmmoCounter()
    {
        int currentStockedAmmo = GameManager.instance.GetInventoryManager().StockedAmmoDictionary[currentAmmoType];

        string currentStockedAmmoText = ((currentStockedAmmo < 10) ? "0" : "") + currentStockedAmmo;

        string ammoText = this.weaponText.text.Split("/")[0] + '/' + currentStockedAmmoText;
        this.weaponText.SetText(ammoText);

        int fastSwapIndex = GameManager.instance.GetInventoryManager().FastSwapIndex;
        GameManager.instance.GetInventoryUI().LoadFastSwapGameplayPanel(fastSwapIndex);
    }
}
