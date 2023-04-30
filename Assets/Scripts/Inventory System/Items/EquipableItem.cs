public class EquipableItem : Item
{
    #region Variables

    protected int weaponShortcut = -1;

    #endregion

    public override void Discard()
    {
        DiscardCurrentWeaponShortcut();
        Destroy(gameObject);
    }

    private void DiscardCurrentWeaponShortcut()
    {
        if (weaponShortcut >= 0)
        {
            EquipableItem[] newArray = GameManager.instance.GetInventoryManager().FastSwapWeaponArray;
            newArray[weaponShortcut] = null;
            GameManager.instance.GetInventoryManager().FastSwapWeaponArray = newArray;
        }
    }

    public void Equip()
    {
        GameManager.instance.GetInventoryManager().EquippedItem = this;
        GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>().FindAndEquipWeapon(this);
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
}
