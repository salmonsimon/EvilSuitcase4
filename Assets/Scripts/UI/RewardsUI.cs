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

    [SerializeField] private Inventory rewardsInventory;

    [SerializeField] private TextMeshProUGUI rewardsCountdownText;
    [SerializeField] private TextMeshProUGUI waveCounterVariableText;

    [SerializeField] private ScrollRect mainScrollRect; 

    private GameObject player;
    private InputsUI input;

    private float timer = 0;

    private void Start()
    {
        player = GameManager.instance.GetPlayer();
        input = player.GetComponent<InputsUI>();
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
        {
            GameManager.instance.GetInventoryDragDropSystem().ForceStopDragging();
            GameManager.instance.GetWaveManager().FinishWave();
        }

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

        SetupRewardInventory(rewardItems);

        rewardsPanel.SetActive(true);

        timer = rewardsCountdown;
        waveCounterVariableText.text = (GameManager.instance.GetWaveManager().CurrentWave + 1).ToString();
    }

    private void SetupRewardInventory(List<Item> rewardItems)
    {
        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        rewardsInventory.InventorySetup(rewardsInventory.GridWidth, rewardsInventory.GridHeight);
        inventoryManager.FillRewardsInventory(rewardsInventory, rewardItems);
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
