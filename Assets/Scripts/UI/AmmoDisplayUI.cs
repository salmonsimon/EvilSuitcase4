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

    public void Setup(Sprite bulletSprite, int currentClipAmmo, int currentStockedAmmo)
    {
        bulletImage.sprite = bulletSprite;
        UpdateCounters(currentClipAmmo, currentStockedAmmo);
    }

    public void UpdateCounters(int currentClipAmmo, int currentStockedAmmo)
    {
        string currentClipAmmoText = ((currentClipAmmo < 10) ? "0" : "") + currentClipAmmo;
        string currentStockedAmmoText = ((currentStockedAmmo < 10) ? "0" : "") + currentStockedAmmo;

        string ammoText = currentClipAmmoText + '/' + currentStockedAmmoText;
        this.ammoText.SetText(ammoText);
    }
}
