using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Utils;

public class InventoryUI : MonoBehaviour
{
    #region Object References

    [Header("Panel References")]
    [SerializeField] private GameObject pauseInventoryPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject fastSwapConfigPanel;
    [SerializeField] private GameObject fastSwapGameplayPanel;

    private InputsUI input;
    
    #endregion

    #region Variables

    [Header("Variables")]
    private EquipableItem fastSwapCandidate;

    private bool isGamePaused = false;
    public bool IsGamePaused { get { return isGamePaused; } }

    #endregion

    #region Parameters

    private float fastSwapGameplayPanelShowDuration = Config.FAST_SWAP_GAMEPLAY_PANEL_SHOW_DURATION;

    #endregion

    private void Start()
    {
        input = GameManager.instance.GetPlayer().GetComponent<InputsUI>();
    }

    private void Update()
    {
        if (IsGamePaused && input.autoSort)
        {
            GameManager.instance.GetInventoryManager().AutoSortMainInventory(pauseInventoryPanel.GetComponent<Inventory>(), GameManager.instance.GetInventoryManager().SavedItems);
            input.autoSort = false;
        }
    }

    public void OpenAndLoadFastSwapConfigPanel()
    {
        Item[] fastSwapItems = GameManager.instance.GetInventoryManager().FastSwapWeaponArray;
        Dictionary<AmmoType, int> ammoDictionary = GameManager.instance.GetInventoryManager().StockedAmmoDictionary;

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

            Instantiate(fastSwapItem.GetItemSO().ItemFastSwapVisual, weaponSpriteContainer);
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
                // TO DO: Same but add durability
            }
            */

            GameObject blockedPanel = itemPanel.Find("Blocked Panel").gameObject;
            blockedPanel.SetActive(false);
        }

        List<Item> blockedItems = GameManager.instance.GetInventoryManager().BlockedItems;

        foreach (Item item in blockedItems)
        {
            if (item.TryGetComponent(out EquipableItem equipableItem) && equipableItem.WeaponShortcut >= 0)
            {
                int weaponShortcut = equipableItem.WeaponShortcut;

                Transform itemPanel = fastSwapConfigPanel.transform.GetChild(weaponShortcut);

                Transform weaponSpriteContainer = itemPanel.Find("Weapon Sprite");

                foreach (Transform child in weaponSpriteContainer)
                    Destroy(child.gameObject);

                TextMeshProUGUI TMPRO = itemPanel.GetComponentInChildren<TextMeshProUGUI>(true);
                TMPRO.gameObject.SetActive(false);

                Instantiate(item.GetItemSO().ItemFastSwapVisual, weaponSpriteContainer);
                weaponSpriteContainer.gameObject.SetActive(true);

                if (IsSubclassOfRawGeneric(item.GetType(), typeof(GunItem)))
                {
                    GunItem gunItem = (GunItem)item;

                    string ammoText = gunItem.CurrentAmmo + "/" + ammoDictionary[gunItem.AmmoType];

                    TMPRO.text = ammoText;
                    TMPRO.gameObject.SetActive(true);
                }
                /*
                else if (IsSubclassOfRawGeneric(item.GetType(), typeof(WeaponItem)))
                {
                    // TO DO: Same but add durability
                }
                */

                GameObject blockedPanel = itemPanel.Find("Blocked Panel").gameObject;
                blockedPanel.SetActive(true);
            }
        }

        fastSwapConfigPanel.SetActive(true);
    }

    public void LoadFastSwapGameplayPanel(int equippedWeaponIndex)
    {
        Item[] fastSwapItems = GameManager.instance.GetInventoryManager().FastSwapWeaponArray;
        Dictionary<AmmoType, int> ammoDictionary = GameManager.instance.GetInventoryManager().StockedAmmoDictionary;

        for (int itemIndex = 0; itemIndex < fastSwapItems.Length; itemIndex++)
        {
            Item fastSwapItem = fastSwapItems[itemIndex];

            Transform itemPanel = fastSwapGameplayPanel.transform.GetChild(itemIndex);

            Transform weaponSpriteContainer = itemPanel.Find("Weapon Sprite");

            foreach (Transform child in weaponSpriteContainer)
                Destroy(child.gameObject);

            TextMeshProUGUI TMPRO = itemPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            TMPRO.gameObject.SetActive(false);

            if (itemIndex == equippedWeaponIndex)
            {
                Image panel = itemPanel.GetComponentInChildren<Image>();
                Color equippedWeaponColor = Color.white;
                equippedWeaponColor.a = .8f;

                panel.color = equippedWeaponColor;
            }
            else
            {
                Image panel = itemPanel.GetComponentInChildren<Image>();
                Color nonEquippedWeaponColor = Color.black;
                nonEquippedWeaponColor.a = .3f;

                panel.color = nonEquippedWeaponColor;
            }

            if (fastSwapItem == null) continue;

            Instantiate(fastSwapItem.GetItemSO().ItemFastSwapVisual, weaponSpriteContainer);

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
                // TO DO: Same but add durability
            }
            */

            GameObject blockedPanel = itemPanel.Find("Blocked Panel").gameObject;
            blockedPanel.SetActive(false);
        }

        List<Item> blockedItems = GameManager.instance.GetInventoryManager().BlockedItems;

        foreach (Item item in blockedItems)
        {
            if (item.TryGetComponent(out EquipableItem equipableItem) && equipableItem.WeaponShortcut >= 0)
            {
                int weaponShortcut = equipableItem.WeaponShortcut;

                Transform itemPanel = fastSwapGameplayPanel.transform.GetChild(weaponShortcut);

                Transform weaponSpriteContainer = itemPanel.Find("Weapon Sprite");

                foreach (Transform child in weaponSpriteContainer)
                    Destroy(child.gameObject);

                TextMeshProUGUI TMPRO = itemPanel.GetComponentInChildren<TextMeshProUGUI>(true);
                TMPRO.gameObject.SetActive(false);

                Image panel = itemPanel.GetComponentInChildren<Image>();
                Color nonEquippedWeaponColor = Color.black;
                nonEquippedWeaponColor.a = .3f;

                panel.color = nonEquippedWeaponColor;

                Instantiate(item.GetItemSO().ItemFastSwapVisual, weaponSpriteContainer);
                weaponSpriteContainer.gameObject.SetActive(true);

                if (IsSubclassOfRawGeneric(item.GetType(), typeof(GunItem)))
                {
                    GunItem gunItem = (GunItem)item;

                    string ammoText = gunItem.CurrentAmmo + "/" + ammoDictionary[gunItem.AmmoType];

                    TMPRO.text = ammoText;
                    TMPRO.gameObject.SetActive(true);
                }
                /*
                else if (IsSubclassOfRawGeneric(item.GetType(), typeof(WeaponItem)))
                {
                    // TO DO: Same but add durability
                }
                */

                GameObject blockedPanel = itemPanel.Find("Blocked Panel").gameObject;
                blockedPanel.SetActive(true);
            }
        }
    }

    #region UI Animations

    public void ShowFastSwapGameplayPanel(int equippedWeaponIndex)
    {
        StopAllCoroutines();
        StartCoroutine(ShowFastSwapGameplayPanelCoroutine(equippedWeaponIndex, fastSwapGameplayPanelShowDuration));
    }

    private IEnumerator ShowFastSwapGameplayPanelCoroutine(int equippedWeaponIndex, float duration)
    {
        LoadFastSwapGameplayPanel(equippedWeaponIndex);

        Animator animator = fastSwapGameplayPanel.GetComponent<Animator>();

        animator.enabled = false;
        animator.enabled = true;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("CrossfadeStart"))
        {
            animator.SetTrigger(Config.CROSSFADE_START_TRIGGER);
            yield return new WaitForSeconds(Config.START_TRANSITION_DURATION);
        }

        yield return new WaitForSeconds(duration);

        animator.SetTrigger(Config.CROSSFADE_END_TRIGGER);

        yield return new WaitForSeconds(Config.END_TRANSITION_DURATION);
    }

    #endregion

    #region Getters and Setters

    public void SetFastSwapCandidate(EquipableItem fastSwapCandidate)
    {
        this.fastSwapCandidate = fastSwapCandidate;
    }

    public void SetFastSwapWeapon(int fastSwapIndex)
    {
        EquipableItem[] currentFastSwapWeaponArray = GameManager.instance.GetInventoryManager().FastSwapWeaponArray;
        List<Item> blockedItems = GameManager.instance.GetInventoryManager().BlockedItems;

        if (currentFastSwapWeaponArray[fastSwapIndex] != null)
            currentFastSwapWeaponArray[fastSwapIndex].SetWeaponShortcut(-1);

        foreach (Item blockedItem in blockedItems)
        {
            if (blockedItem.TryGetComponent(out EquipableItem equipableItem) && equipableItem.WeaponShortcut == fastSwapIndex)
                equipableItem.SetWeaponShortcut(-1);
        }

        if (fastSwapCandidate)
        {
            fastSwapCandidate.SetWeaponShortcut(fastSwapIndex);

            EquipableItem[] newArray = currentFastSwapWeaponArray;
            newArray[fastSwapIndex] = fastSwapCandidate;
            GameManager.instance.GetInventoryManager().FastSwapWeaponArray = newArray;

            fastSwapCandidate = null;
        }
    }

    #endregion

    #region Pause Menu Related Methods

    public void PauseGame()
    {
        GameManager.instance.GetSFXManager().PlaySound(Config.PAUSE_SFX);

        GameObject player = GameManager.instance.GetPlayer();
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);

        SetGamePaused(true);

        OpenPauseInventory();

        settingsPanel.SetActive(false);
    }

    public void OpenPauseInventory()
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();
        int mainInventoryWidth = inventoryManager.InventoryWidth;
        int mainInventoryHeight = inventoryManager.InventoryHeight;

        pauseInventoryPanel.GetComponent<Inventory>().InventorySetup(mainInventoryWidth, mainInventoryHeight);

        pauseInventoryPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        SetGamePaused(false);

        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(true);

        pauseInventoryPanel.SetActive(false);
        settingsPanel.SetActive(false);
        fastSwapConfigPanel.SetActive(false);
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

    #endregion
}
