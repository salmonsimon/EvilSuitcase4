using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Utils;

public class InventoryUI : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject pauseInventoryPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject fastSwapConfigPanel;
    [SerializeField] private GameObject fastSwapGameplayPanel;
    [SerializeField] private GameObject rewardsInventoryPanel;

    [Header("Variables")]
    [SerializeField] private GunItem fastSwapCandidate;

    private bool isGamePaused = false;
    public bool IsGamePaused { get { return isGamePaused; } }

    public void OpenAndLoadFastSwapConfigPanel()
    {
        Item[] fastSwapItems = GameManager.instance.GetInventoryManager().FastSwapWeaponArray;
        Dictionary<AmmoType, int> ammoDictionary = GameManager.instance.GetInventoryManager().AmmoDictionary;

        for (int itemIndex = 0; itemIndex < fastSwapItems.Length; itemIndex++)
        {
            Item fastSwapItem = fastSwapItems[itemIndex];

            Transform itemPanel = fastSwapConfigPanel.transform.GetChild(itemIndex);

            Transform weaponSpriteContainer = itemPanel.Find("Weapon Sprite");

            foreach (Transform child in weaponSpriteContainer)
                Destroy(child.gameObject);

            TextMeshProUGUI TMPRO = itemPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            TMPRO.gameObject.SetActive(false);

            if (fastSwapItem == null) continue;

            Instantiate(fastSwapItem.GetItemSO().visual, weaponSpriteContainer);
            weaponSpriteContainer.gameObject.SetActive(true);

            if (IsSubclassOfRawGeneric(fastSwapItem.GetType(), typeof(GunItem)))
            {
                GunItem gunItem = (GunItem) fastSwapItem;

                string ammoText = gunItem.CurrentAmmo + "/" + ammoDictionary[gunItem.AmmoType];

                TMPRO.text = ammoText;
                TMPRO.gameObject.SetActive(true);
            }
            /*
            else if (IsSubclassOfRawGeneric(fastSwapItem.GetType(), typeof(WeaponItem)))
            {
                // Same but add durability
            }
            */
        }

        fastSwapConfigPanel.SetActive(true);
    }

    public void OpenAndLoadFastSwapGameplayPanel()
    {
        Item[] fastSwapItems = GameManager.instance.GetInventoryManager().FastSwapWeaponArray;
        Dictionary<AmmoType, int> ammoDictionary = GameManager.instance.GetInventoryManager().AmmoDictionary;

        for (int itemIndex = 0; itemIndex < fastSwapItems.Length; itemIndex++)
        {
            Item fastSwapItem = fastSwapItems[itemIndex];

            Transform itemPanel = fastSwapGameplayPanel.transform.GetChild(itemIndex);

            Transform weaponSpriteContainer = itemPanel.Find("Weapon Sprite");

            foreach (Transform child in weaponSpriteContainer)
                Destroy(child.gameObject);

            TextMeshProUGUI TMPRO = itemPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            TMPRO.gameObject.SetActive(false);

            if (fastSwapItem == null) continue;

            Instantiate(fastSwapItem.GetItemSO().visual, weaponSpriteContainer);
            weaponSpriteContainer.gameObject.SetActive(true);

            if (IsSubclassOfRawGeneric(fastSwapItem.GetType(), typeof(GunItem)))
            {
                GunItem gunItem = (GunItem)fastSwapItem;

                string ammoText = gunItem.CurrentAmmo + "/" + ammoDictionary[gunItem.AmmoType];

                TMPRO.text = ammoText;
                TMPRO.gameObject.SetActive(true);
            }
            /*
            else if (IsSubclassOfRawGeneric(fastSwapItem.GetType(), typeof(WeaponItem)))
            {
                // Same but add durability
            }
            */
        }

        fastSwapGameplayPanel.SetActive(true);
    }

    public void SetFastSwapCandidate(GunItem fastSwapCandidate)
    {
        this.fastSwapCandidate = fastSwapCandidate;
    }

    public void SetFastSwapWeapon(int fastSwapIndex)
    {
        if (fastSwapCandidate)
        {
            fastSwapCandidate.SetWeaponShortcut(fastSwapIndex);
            GameManager.instance.GetInventoryManager().FastSwapWeaponArray[fastSwapIndex] = fastSwapCandidate;
            fastSwapCandidate = null;
        }
    }

    public void PauseGame()
    {
        GameManager.instance.GetSFXManager().PlaySound(Config.PAUSE_SFX);

        GameObject player = GameManager.instance.GetPlayer();
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);

        SetGamePaused(true);

        pauseInventoryPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        SetGamePaused(false);

        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(true);

        pauseInventoryPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void SetGamePaused(bool value)
    {
        isGamePaused = value;

        if (value)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void ToMainMenu()
    {
        GameManager.instance.ToMainMenu();
    }
}
