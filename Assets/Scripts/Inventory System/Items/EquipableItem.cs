using UnityEngine;

public class EquipableItem : Item
{
    #region Variables

    //TO DO: CHANGE THIS AND PUT IT ONLY IN GUNS
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

        EquipableItem currentEquipedItem = GameManager.instance.GetInventoryManager().EquippedItem;

        if (currentEquipedItem && currentEquipedItem.Equals(this))
            Unequip();

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
        GameManager.instance.GetSFXManager().PlaySound(Config.EQUIP_SFX);
    }

    public void Unequip()
    {

        if (GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>().EquippedWeapon)
            GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>().UnequipWeapon();

        GameManager.instance.GetInventoryManager().EquippedItem = null;
        GameManager.instance.GetInventoryManager().CurrentEquippedWeaponShortcutIndex = -1;
    }

    public void OpenFastSwapConfigPanel()
    {
        GameManager.instance.GetPauseMenuUI().SetFastSwapCandidate(this);
        GameManager.instance.GetPauseMenuUI().OpenAndLoadFastSwapConfigPanel();
    }

    public void SetWeaponShortcut(int newWeaponShortcut)
    {
        DiscardCurrentWeaponShortcut();

        weaponShortcut = newWeaponShortcut;
    }


    // TO DO: MAKE THE OVERRIDE IN GUNS TOO, TO ADD THE RELOADING PANEL
    protected override GameObject GetCurrentButtonPanel()
    {
        if (GameManager.instance.IsOnRewardsUI)
            return HoldingInventory.MainInventory ? rewardsMainInventoryButtonPanel : rewardsInventoryButtonPanel;
        else
            return GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>().IsReloading ?
                reloadingMainInventoryButtonPanel : mainInventoryButtonPanel;
    }
}
