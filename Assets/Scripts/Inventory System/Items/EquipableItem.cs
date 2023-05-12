using UnityEngine;

public class EquipableItem : Item
{
    #region Variables

    [SerializeField] protected GameObject reloadingMainInventoryButtonPanel;
    [SerializeField] protected int weaponShortcut = -1;
    public int WeaponShortcut { get { return weaponShortcut; } }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        reloadingMainInventoryButtonPanel.SetActive(false);
    }

    public override void Discard()
    {
        DiscardCurrentWeaponShortcut();
        RemoveFromMainInventory();

        Destroy(gameObject);
    }

    public void DiscardCurrentWeaponShortcut(bool saveWeaponShortcut = false)
    {
        if (weaponShortcut >= 0)
        {
            EquipableItem[] newArray = GameManager.instance.GetInventoryManager().FastSwapWeaponArray;
            newArray[weaponShortcut] = null;
            GameManager.instance.GetInventoryManager().FastSwapWeaponArray = newArray;
        }

        if (!saveWeaponShortcut)
            weaponShortcut = -1;
    }

    public void Equip()
    {
        GameManager.instance.GetInventoryManager().EquippedItem = this;
        GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>().FindAndEquipWeapon(this);
    }

    public void Unequip()
    {
        GameManager.instance.GetInventoryManager().EquippedItem = null;
        GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>().UnequipWeapon();
        GameManager.instance.GetInventoryManager().CurrentEquippedWeaponShortcutIndex = -1;
    }

    public void OpenFastSwapConfigPanel()
    {
        GameManager.instance.GetInventoryUI().SetFastSwapCandidate(this);
        GameManager.instance.GetInventoryUI().OpenAndLoadFastSwapConfigPanel();
    }

    public void SetWeaponShortcut(int newWeaponShortcut)
    {
        DiscardCurrentWeaponShortcut();

        weaponShortcut = newWeaponShortcut;
    }

    protected override GameObject GetCurrentButtonPanel()
    {
        if (GameManager.instance.IsOnRewardsUI)
            return HoldingInventory.MainInventory ? rewardsMainInventoryButtonPanel : rewardsInventoryButtonPanel;
        else
            return GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>().IsReloading ?
                reloadingMainInventoryButtonPanel : mainInventoryButtonPanel;
    }
}
