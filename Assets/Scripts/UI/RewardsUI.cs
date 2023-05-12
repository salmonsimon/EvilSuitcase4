using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RewardsUI : MonoBehaviour
{
    [SerializeField] private GameObject rewardsPanel;

    [SerializeField] private Inventory mainInventory;
    public Inventory MainInventory { get { return mainInventory; } }

    [SerializeField] private Inventory consumableRewardsInventory;
    [SerializeField] private Inventory meleeWeaponRewardsInventory;
    [SerializeField] private Inventory gunsRewardsInventory;

    private GameObject player;
    private InputsUI input;

    private void Start()
    {
        player = GameManager.instance.GetPlayer();
        input = player.GetComponent<InputsUI>();
    }

    // TO DO: DELETE UPDATE FUNCTION AFTER TESTING PROPERLY
    private void Update()
    {
        if (!GameManager.instance.IsOnRewardsUI && Input.GetKeyDown(KeyCode.M))
            OpenRewardsUI();
        else if (GameManager.instance.IsOnRewardsUI && Input.GetKeyDown(KeyCode.M))
            CloseRewardsUI();

        if (GameManager.instance.IsOnRewardsUI && input.autoSort)
        {
            GameManager.instance.GetInventoryManager().AutoSortMainInventory(mainInventory, GameManager.instance.GetInventoryManager().SavedItems);
            input.autoSort = false;
        }
    }

    private void OpenRewardsUI()
    {
        GameManager.instance.IsOnRewardsUI = true;
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(false);
        Cursor.visible = true;

        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();
        int mainInventoryWidth = inventoryManager.InventoryWidth;
        int mainInventoryHeight = inventoryManager.InventoryHeight;

        mainInventory.InventorySetup(mainInventoryWidth, mainInventoryHeight);

        consumableRewardsInventory.InventorySetup(consumableRewardsInventory.GridWidth, consumableRewardsInventory.GridHeight);
        meleeWeaponRewardsInventory.InventorySetup(meleeWeaponRewardsInventory.GridWidth, meleeWeaponRewardsInventory.GridHeight);
        gunsRewardsInventory.InventorySetup(gunsRewardsInventory.GridWidth, gunsRewardsInventory.GridHeight);

        rewardsPanel.SetActive(true);
    }

    private void CloseRewardsUI()
    {
        GameManager.instance.IsOnRewardsUI = false;
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        player.GetComponent<StarterAssetsInputs>().SetCursorLockState(true);

        rewardsPanel.SetActive(false);
    }
}
