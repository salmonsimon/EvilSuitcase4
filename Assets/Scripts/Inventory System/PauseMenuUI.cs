using AirFishLab.ScrollingList;
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

public class PauseMenuUI : MonoBehaviour
{
    #region Object References

    [Header("References")]
    [SerializeField] private GameObject iconContainer;
    [SerializeField] private GameObject keyBindingPanel;

    [SerializeField] private GameObject inventoryPanel;

    [SerializeField] private List<GameObject> pauseMenuPanelList;
    [SerializeField] private CircularScrollingList iconCircularScrillList;
    [SerializeField] private int activePanelIndex = 0;

    [Header("Fast Swap Panels")]
    [SerializeField] private GameObject fastSwapConfigPanel;
    [SerializeField] private GameObject fastSwapGameplayPanel;

    [Header("Audio Sliders")]
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

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

        iconCircularScrillList.SetInteractable(false);
    }

    private void Update()
    {
        if (isGamePaused)
        {
            if (input.next)
            {
                NextMenu();
                input.next = false;
            }
            else if (input.previous)
            {
                PreviousMenu();
                input.previous = false;
            }
            else if (input.autoSort)
            {
                GameManager.instance.GetInventoryManager().AutoSortMainInventory(inventoryPanel.GetComponent<Inventory>(), GameManager.instance.GetInventoryManager().SavedItems);
                input.autoSort = false;
            }
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

            if (fastSwapItem.TryGetComponent(out GunItem gunItem))
            {
                string ammoText = gunItem.CurrentAmmo + "/" + ammoDictionary[gunItem.AmmoType];

                TMPRO.text = ammoText;
                TMPRO.gameObject.SetActive(true);
            }
            else if (fastSwapItem.TryGetComponent(out MeleeItem meleeItem))
            {
                int currentDurabilityInt = Mathf.RoundToInt(meleeItem.CurrentDurability * 100);

                string currentDurabilityText = ((currentDurabilityInt < 10) ? "0" : "") + currentDurabilityInt;

                string durabilityText = currentDurabilityText + "/100%";

                TMPRO.text = durabilityText;
                TMPRO.gameObject.SetActive(true);
            }

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

                if (item.TryGetComponent(out GunItem gunItem))
                {
                    string ammoText = gunItem.CurrentAmmo + "/" + ammoDictionary[gunItem.AmmoType];

                    TMPRO.text = ammoText;
                    TMPRO.gameObject.SetActive(true);
                }
                else if (item.TryGetComponent(out MeleeItem meleeItem))
                {
                    int currentDurabilityInt = Mathf.RoundToInt(meleeItem.CurrentDurability * 100);

                    string currentDurabilityText = ((currentDurabilityInt < 10) ? "0" : "") + currentDurabilityInt;

                    string durabilityText = currentDurabilityText + "/100%";

                    TMPRO.text = durabilityText;
                    TMPRO.gameObject.SetActive(true);
                }

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

            if (fastSwapItem.TryGetComponent(out GunItem gunItem))
            {
                string ammoText = gunItem.CurrentAmmo + "/" + ammoDictionary[gunItem.AmmoType];

                TMPRO.text = ammoText;
                TMPRO.gameObject.SetActive(true);
            }
            else if (fastSwapItem.TryGetComponent(out MeleeItem meleeItem))
            {
                int currentDurabilityInt = Mathf.RoundToInt(meleeItem.CurrentDurability * 100);

                string currentDurabilityText = ((currentDurabilityInt < 10) ? "0" : "") + currentDurabilityInt;

                string durabilityText = currentDurabilityText + "/100%";

                TMPRO.text = durabilityText;
                TMPRO.gameObject.SetActive(true);
            }

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

                if (item.TryGetComponent(out GunItem gunItem))
                {
                    string ammoText = gunItem.CurrentAmmo + "/" + ammoDictionary[gunItem.AmmoType];

                    TMPRO.text = ammoText;
                    TMPRO.gameObject.SetActive(true);
                }
                else if (item.TryGetComponent(out MeleeItem meleeItem))
                {
                    int currentDurabilityInt = Mathf.RoundToInt(meleeItem.CurrentDurability * 100);

                    string currentDurabilityText = ((currentDurabilityInt < 10) ? "0" : "") + currentDurabilityInt;

                    string durabilityText = currentDurabilityText + "/100%";

                    TMPRO.text = durabilityText;
                    TMPRO.gameObject.SetActive(true);
                }

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

        iconContainer.SetActive(true);

        SetGamePaused(true);

        OpenActiveMenuPanel();
    }

    public void PauseGameAndOpenInventory()
    {
        GameManager.instance.GetSFXManager().PlaySound(Config.PAUSE_SFX);

        GameObject player = GameManager.instance.GetPlayer();
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);

        iconContainer.SetActive(true);

        SetGamePaused(true);

        OpenPauseInventory();
    }

    private void OpenActiveMenuPanel()
    {
        if (activePanelIndex == 0)
            InventorySetup();

        pauseMenuPanelList[activePanelIndex].SetActive(true);

        StartCoroutine(WaitAndSetActiveIconAlphaCoroutine());
    }

    public void WaitAndSetActiveIconAlpha()
    {
        foreach (Transform child in iconCircularScrillList.transform)
            child.GetComponent<CanvasGroup>().alpha = .3f;

        Transform activeIcon = iconCircularScrillList.transform.GetChild(iconCircularScrillList.ListBank.GetContentCount() - 1);
        activeIcon.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    private IEnumerator WaitAndSetActiveIconAlphaCoroutine()
    {
        yield return null;

        WaitAndSetActiveIconAlpha();
    }

    public void OpenPauseInventory()
    {
        InventorySetup();

        inventoryPanel.SetActive(true);
    }

    private void InventorySetup()
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();
        int mainInventoryWidth = inventoryManager.InventoryWidth;
        int mainInventoryHeight = inventoryManager.InventoryHeight;

        inventoryPanel.GetComponent<Inventory>().InventorySetup(mainInventoryWidth, mainInventoryHeight);
    }

    public void ResumeGame()
    {
        SetGamePaused(false);

        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(true);

        iconContainer.SetActive(false);

        keyBindingPanel.SetActive(false);

        foreach(GameObject panel in pauseMenuPanelList)
            panel.SetActive(false);
    }

    public void SetGamePaused(bool value)
    {

        isGamePaused = value;

        if (value)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void MainMenuButton()
    {
        GameManager.instance.ToMainMenu();
    }

    private void NextMenu()
    {
        pauseMenuPanelList[activePanelIndex].SetActive(false);

        Transform activeIcon = iconCircularScrillList.transform.GetChild(iconCircularScrillList.ListBank.GetContentCount() - 1);
        activeIcon.GetComponent<CanvasGroup>().alpha = .3f;

        activePanelIndex = (int)Mathf.Repeat(activePanelIndex + 1, iconCircularScrillList.ListBank.GetContentCount());

        iconCircularScrillList.SelectContentID(activePanelIndex);

        pauseMenuPanelList[activePanelIndex].SetActive(true);
    }

    private void PreviousMenu()
    {
        pauseMenuPanelList[activePanelIndex].SetActive(false);

        Transform activeIcon = iconCircularScrillList.transform.GetChild(iconCircularScrillList.ListBank.GetContentCount() - 1);
        activeIcon.GetComponent<CanvasGroup>().alpha = .3f;

        activePanelIndex = (int)Mathf.Repeat(activePanelIndex - 1, iconCircularScrillList.ListBank.GetContentCount());

        iconCircularScrillList.SelectContentID(activePanelIndex);

        pauseMenuPanelList[activePanelIndex].SetActive(true);
    }

    public void OnNewIconSelected(ListBox previousSelected, ListBox newSelected)
    {
        previousSelected.GetComponent<CanvasGroup>().alpha = .3f;
        newSelected.GetComponent<CanvasGroup>().alpha = 1f;
    }

    #endregion

    #region Audio

    public void SetAudioSlidersVolumesPauseMenu()
    {
        musicVolumeSlider.value = Settings.Instance.musicVolume;
        sfxVolumeSlider.value = Settings.Instance.SFXVolume;
    }

    public void UpdateMusicVolume(float value)
    {
        GameManager.instance.GetMusicManager().UpdateVolume(value);
    }

    public void UpdateSFXVolume(float value)
    {
        GameManager.instance.GetSFXManager().UpdateVolume(value);
    }

    #endregion
}
