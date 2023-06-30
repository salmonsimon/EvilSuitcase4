using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RewardsUI : MonoBehaviour
{
    [SerializeField] private GameObject rewardsPanel;

    [SerializeField] private Inventory mainInventory;
    public Inventory MainInventory { get { return mainInventory; } }

    [SerializeField] private Inventory consumableRewardsInventory;
    [SerializeField] private Inventory meleeWeaponRewardsInventory;
    [SerializeField] private Inventory gunsRewardsInventory;

    [SerializeField] private TextMeshProUGUI rewardsCountdownText;
    [SerializeField] private TextMeshProUGUI waveCounterVariableText;

    [SerializeField] private ScrollRect mainScrollRect; 

    private GameObject player;
    private InputsUI input;

    private float timer = 0;

    private bool initialized = false;

    private void Start()
    {
        player = GameManager.instance.GetPlayer();
        input = player.GetComponent<InputsUI>();

        initialized = true;
    }

    private void Update()
    {
        if (input.autoSort && GameManager.instance.IsOnRewardsUI)
        {
            GameManager.instance.GetInventoryManager().AutoSortMainInventory(mainInventory);
            GameManager.instance.GetSFXManager().PlaySound(Config.AUTO_SORT_SFX);
            input.autoSort = false;
        }

        if (GameManager.instance.IsOnRewardsUI)
            timer -= Time.deltaTime;

        if (timer < 0 && GameManager.instance.IsOnRewardsUI)
            GameManager.instance.GetWaveManager().FinishWave();

        string countdownString = Utils.FloatToTimeSecondsFormat(timer);

        if (!rewardsCountdownText.text.Equals(countdownString))
            rewardsCountdownText.text = countdownString;
    }


    public void OpenRewardsUI(List<Item> rewardItems, float rewardsCountdown)
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);
        Cursor.visible = true;

        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();
        int mainInventoryWidth = inventoryManager.InventoryWidth;
        int mainInventoryHeight = inventoryManager.InventoryHeight;

        mainInventory.InventorySetup(mainInventoryWidth, mainInventoryHeight);

        SetupRewardInventories(rewardItems);

        rewardsPanel.SetActive(true);

        timer = rewardsCountdown;
        waveCounterVariableText.text = (GameManager.instance.GetWaveManager().CurrentWave + 1).ToString();
    }

    private void SetupRewardInventories(List<Item> rewardItems)
    {
        List<Item> consumableItems = new List<Item>();
        List<Item> meleeItems = new List<Item>();
        List<Item> gunItems = new List<Item>();

        foreach (Item item in rewardItems)
        {
            if (Utils.IsSubclassOfRawGeneric(item.GetType(), typeof(GunItem)))
                gunItems.Add(item);
            else if (Utils.IsSubclassOfRawGeneric(item.GetType(), typeof(MeleeItem)))
                meleeItems.Add(item);
            else
                consumableItems.Add(item);
        }

        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        consumableRewardsInventory.InventorySetup(consumableRewardsInventory.GridWidth, consumableRewardsInventory.GridHeight);
        inventoryManager.FillRewardsInventory(consumableRewardsInventory, consumableItems);

        meleeWeaponRewardsInventory.InventorySetup(meleeWeaponRewardsInventory.GridWidth, meleeWeaponRewardsInventory.GridHeight);
        inventoryManager.FillRewardsInventory(meleeWeaponRewardsInventory, meleeItems);

        gunsRewardsInventory.InventorySetup(gunsRewardsInventory.GridWidth, gunsRewardsInventory.GridHeight);
        inventoryManager.FillRewardsInventory(gunsRewardsInventory, gunItems);
    }

    public void CloseRewardsUI()
    {
        GameManager.instance.IsOnRewardsUI = false;
        rewardsPanel.SetActive(false);

        mainScrollRect.verticalNormalizedPosition = 1;
    }

    public void NextWaveButton()
    {
        timer = 0;
    }
}
