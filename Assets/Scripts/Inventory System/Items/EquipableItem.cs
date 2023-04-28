using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class EquipableItem : Item
{
    protected int weaponShortcut = -1;

    public override void Discard()
    {
        DiscardCurrentWeaponShortcut();
        Destroy(gameObject);
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

    private void DiscardCurrentWeaponShortcut()
    {
        if (weaponShortcut >= 0)
        {
            EquipableItem[] newArray = GameManager.instance.GetInventoryManager().FastSwapWeaponArray;
            newArray[weaponShortcut] = null;
            GameManager.instance.GetInventoryManager().FastSwapWeaponArray = newArray;
        }
    }
}
