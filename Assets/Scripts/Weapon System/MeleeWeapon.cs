using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    #region Configuration

    [Header("Weapon Configuration")]
    [SerializeField] protected MeleeWeaponScriptableObject weaponConfiguration;
    public MeleeWeaponScriptableObject WeaponConfiguration { get { return weaponConfiguration; } }

    #endregion

    #region Variables

    protected float LastAttackTime;

    [SerializeField] protected float currentDurability = 1f;
    public float CurrentDurability { get { return currentDurability; } set { currentDurability = value; } }

    protected AnimationClip currentAttackAnimation;
    protected List<AnimationClip> remainingAttackAnimations;

    #endregion

    #region Object References

    protected ThirdPersonShooterController playerThirdPersonShooterController;

    protected AmmoDisplayUI ammoDisplayUI;

    #endregion

    protected override void Start()
    {
        base.Start();

        ammoDisplayUI = GameManager.instance.GetAmmoDisplayUI();

        LastAttackTime = -weaponConfiguration.AttacksConfig.AttackRate;

        playerThirdPersonShooterController = GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>();

        int randomFirstAttackAnimationIndex = Random.Range(0, weaponConfiguration.AttacksConfig.AttackAnimationClips.Count);

        currentAttackAnimation = weaponConfiguration.AttacksConfig.AttackAnimationClips[randomFirstAttackAnimationIndex];

        remainingAttackAnimations = new List<AnimationClip>(weaponConfiguration.AttacksConfig.AttackAnimationClips);
        remainingAttackAnimations.RemoveAt(randomFirstAttackAnimationIndex);
    }

    public override void Attack()
    {
        if (!playerThirdPersonShooterController.IsAttacking && 
            Time.time > weaponConfiguration.AttacksConfig.AttackRate + LastAttackTime)
        {
            playerThirdPersonShooterController.PlayMeleeAttackAnimation(currentAttackAnimation);

            remainingAttackAnimations.Add(currentAttackAnimation);

            currentAttackAnimation = remainingAttackAnimations[0];
            remainingAttackAnimations.RemoveAt(0);

            float durabilityLoss = weaponConfiguration.DurabilityConfig.GetDurabilityLoss();
            SubstractDurability(durabilityLoss);

            if (CurrentDurability <= 0)
                Break();
        }
    }

    protected virtual void SubstractDurability(float durabilityLoss)
    {
        CurrentDurability -= durabilityLoss;

        GameManager.instance.GetInventoryManager().EquippedItem.GetComponent<MeleeItem>().CurrentDurability -= durabilityLoss;

        UpdateDurabilityDisplayCounters();
    }

    private void UpdateDurabilityDisplayCounters()
    {
        ammoDisplayUI.UpdateCounters(currentDurability);
    }

    private void Break()
    {
        sfx.PlayAudioClip(weaponConfiguration.AudioConfig.BreakClip);

        GameManager.instance.GetInventoryManager().EquippedItem.Discard();
    }
}
