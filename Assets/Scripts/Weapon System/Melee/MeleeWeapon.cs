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

    protected WeaponDisplayUI ammoDisplayUI;

    #endregion

    protected override void Start()
    {
        base.Start();

        ammoDisplayUI = GameManager.instance.GetWeaponDisplayUI();

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
        }
    }


    // TO DO: THIS SHOULD BE USED ONLY WHEN HITTING SOME ENEMY, DO IT WHEN COLLIDING WITH AN ENEMY
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

    public void Break()
    {
        StartCoroutine(BreakCoroutine());
    }

    private IEnumerator BreakCoroutine()
    {
        yield return null;

        ammoDisplayUI.UnequipWeapon();

        playerThirdPersonShooterController.UnequipWeapon();

        GameManager.instance.GetSFXManager().PlaySound(weaponConfiguration.AudioConfig.BreakClip);

        yield return null;

        GameManager.instance.GetInventoryManager().EquippedItem.Discard();
    }
}
