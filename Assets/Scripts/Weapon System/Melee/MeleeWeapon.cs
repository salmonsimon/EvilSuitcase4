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

    private int currentAttackID = -1;
    public int CurrentAttackID { get { return currentAttackID; }}

    #endregion

    #region Object References

    protected ThirdPersonShooterController playerThirdPersonShooterController;

    protected WeaponDisplayUI weaponDisplayUI;

    #endregion

    protected override void Start()
    {
        base.Start();

        weaponDisplayUI = GameManager.instance.GetWeaponDisplayUI();

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
            sfx.PlayAudioClip(weaponConfiguration.AudioConfig.GetRandomAttackClip());

            currentAttackID = Mathf.RoundToInt(Time.time);

            remainingAttackAnimations.Add(currentAttackAnimation);

            currentAttackAnimation = remainingAttackAnimations[0];
            remainingAttackAnimations.RemoveAt(0);
        }
    }

    public virtual void SubstractDurability()
    {
        float durabilityLoss = weaponConfiguration.DurabilityConfig.GetDurabilityLoss();

        CurrentDurability -= durabilityLoss;

        GameManager.instance.GetInventoryManager().EquippedItem.GetComponent<MeleeItem>().CurrentDurability -= durabilityLoss;

        UpdateDurabilityDisplayCounters();
    }

    private void UpdateDurabilityDisplayCounters()
    {
        weaponDisplayUI.UpdateCounters(currentDurability);
    }

    public void Break()
    {
        StartCoroutine(BreakCoroutine());
    }

    private IEnumerator BreakCoroutine()
    {
        yield return null;

        weaponDisplayUI.UnequipWeapon();

        playerThirdPersonShooterController.UnequipWeapon();

        GameManager.instance.GetSFXManager().PlaySound(weaponConfiguration.AudioConfig.BreakClip);

        GameManager.instance.GetInventoryManager().EquippedItem.Discard();
    }
}
