using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using static Utils;
using System;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    public CinemachineVirtualCamera AimVirtualCamera { get { return aimVirtualCamera; } }

    [SerializeField] private float normalLookSensitivity = 2f;
    [SerializeField] private float aimSensitivity = 1f;

    [SerializeField] private Transform lookAt;

    [Header("Rigging")]
    [SerializeField] private List<Rig> idleRigs;
    [SerializeField] private List<Rig> aimRigs;
    [SerializeField] private List<Rig> attackRigs;

    [Header("Weapon")]
    [SerializeField] private Transform weaponContainer;

    [SerializeField] private Weapon equippedWeapon;
    public Weapon EquippedWeapon { get { return equippedWeapon; } }

    #region Object References

    private ThirdPersonController thirdPersonController;
    public ThirdPersonController ThirdPersonController { get { return thirdPersonController; } }

    private StarterAssetsInputs starterAssetsInputs;
    private PlayerGunAnimations playerGunAnimations;

    private Animator animator;

    private CrossHair crosshair;

    private PlayerHealthAnimations playerHealthAnimations;

    private HealthManager playerHealthManager;

    #endregion

    #region Logic Variables

    private Vector3 aimDirection;
    private bool aiming = false;

    private bool isReloading = false;
    public bool IsReloading { get { return isReloading; } }

    private AnimationClip reloadAnimationClip = null;
    private Coroutine reloadCoroutine = null;

    private bool isAbleToReload = true;
    public bool IsAbleToReload { get { return isAbleToReload; } }

    private Coroutine shotgunShootCoroutine = null;

    private bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } }

    #endregion

    #region Parameters

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float aimMovementSpeed = 2f;

    #endregion

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        playerGunAnimations = GetComponent<PlayerGunAnimations>();

        animator = GetComponent<Animator>();
        crosshair = GameManager.instance.GetCrossHair();
        playerHealthAnimations = GetComponent<PlayerHealthAnimations>(); 
        playerHealthManager = GetComponent<HealthManager>();
    }

    private void OnEnable()
    {
        if (playerHealthManager)
        {
            playerHealthManager.OnDeath += Death;
            playerHealthManager.OnRevival += Revival;
        }
    }

    private void OnDisable()
    {
        if (playerHealthManager)
        {
            playerHealthManager.OnDeath -= Death;
            playerHealthManager.OnRevival -= Revival;
        }
    }

    private void Start()
    {
        if (playerHealthManager)
        {
            playerHealthManager.OnDeath += Death;
            playerHealthManager.OnRevival += Revival;
        }
    }

    private void Death()
    {
        aiming = false;

        thirdPersonController.enabled = false;
    }

    private void Revival()
    {
        thirdPersonController.enabled = true;
    }

    private void Update()
    {
        if (!playerHealthManager.IsAlive) 
        {
            aimVirtualCamera.gameObject.SetActive(false);
            crosshair.ShowCrossHairUI(false);

            return;
        }

        if (playerHealthAnimations.IsOnHurtAnimation)
        {
            aiming = false;

            crosshair.ShowCrossHairUI(false);

            aimVirtualCamera.gameObject.SetActive(false);

            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetAbleToSprint(false);

            thirdPersonController.MoveSpeed = aimMovementSpeed;

            starterAssetsInputs.shoot = false;

            return;
        }
            

        if (IsAttacking)
        {
            aiming = false;

            crosshair.ShowCrossHairUI(false);

            aimVirtualCamera.gameObject.SetActive(false);

            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            thirdPersonController.SetAbleToSprint(false);

            thirdPersonController.MoveSpeed = 0f;

            return;
        }

        if ((starterAssetsInputs.aim && !isReloading))
        {
            aiming = true;
            crosshair.ShowCrossHairUI(true);

            aimVirtualCamera.gameObject.SetActive(true);

            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            thirdPersonController.SetAbleToSprint(false);
            thirdPersonController.MoveSpeed = aimMovementSpeed;
        }
        else 
        {
            aiming = false;
            crosshair.ShowCrossHairUI(false);

            starterAssetsInputs.shoot = false;

            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalLookSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetAbleToSprint(true);
            thirdPersonController.MoveSpeed = movementSpeed;
        }

        if (!isReloading && 
            isAbleToReload &&
            starterAssetsInputs.reload &&
            equippedWeapon != null && 
            IsSubclassOfRawGeneric(equippedWeapon.GetType(), typeof(Gun))
           )
        {
            if (equippedWeapon.GetComponent<Gun>().CanReload())
            {
                PlayReloadAnimation();
                return;
            }
        }

        if (!isReloading && starterAssetsInputs.shoot && equippedWeapon != null)
        {
            equippedWeapon.Attack();

            if (IsSubclassOfRawGeneric(equippedWeapon.GetType(), typeof(Gun)) &&
                equippedWeapon.GetComponent<Gun>().CurrentClipAmmo == 0)
            {
                starterAssetsInputs.shoot = false;
                crosshair.OutOfBullets();
            }
        }

        if (IsReloading)
            thirdPersonController.SetAbleToSprint(false);
    }

    private void FixedUpdate()
    {
        if (IsAttacking)
        {
            foreach (Rig rig in attackRigs)
                rig.weight = Mathf.Lerp(rig.weight, 1f, Time.deltaTime * 20f);

            foreach (Rig rig in idleRigs)
                rig.weight = Mathf.Lerp(rig.weight, 0f, Time.deltaTime * 20f);
        }
        else
        {
            foreach (Rig rig in attackRigs)
                rig.weight = Mathf.Lerp(rig.weight, 0f, Time.deltaTime * 20f);

            foreach (Rig rig in idleRigs)
                rig.weight = Mathf.Lerp(rig.weight, 1f, Time.deltaTime * 20f);
        }

        if (aiming)
        {
            if (Vector3.Dot(transform.forward, aimDirection) > .9f)
            {
                foreach (Rig rig in aimRigs)
                    rig.weight = Mathf.Lerp(rig.weight, 1f, Time.deltaTime * 20f);
            }
        }
        else
        {
            foreach (Rig rig in aimRigs)
                rig.weight = Mathf.Lerp(rig.weight, 0f, Time.deltaTime * 20f);
        }
    }

    private void LateUpdate()
    {
        if (aiming)
        {
            Vector3 worldAimTarget = lookAt.position;
            worldAimTarget.y = transform.position.y;

            aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon)
        {
            ActivateWeapon(equippedWeapon, false);
            equippedWeapon = null;
        }

        GameManager.instance.GetWeaponDisplayUI().UnequipWeapon();
    }

    public void FindAndEquipWeapon(EquipableItem equipableItem)
    {
        string containerName = "";

        if (equipableItem.GetItemSO().ItemType == ItemType.Melee)
            containerName = Config.MELEE_WEAPON_CONTAINER_NAME;
        else if (equipableItem.GetItemSO().ItemType == ItemType.Gun)
            containerName = Config.GUN_CONTAINER_NAME;

        Transform weaponTransform = weaponContainer.Find(containerName + "/" + equipableItem.GetItemSO().ItemName);

        if (weaponTransform && weaponTransform.TryGetComponent(out Weapon weaponToEquip))
            EquipWeapon(weaponToEquip, equipableItem);
    }

    public void EquipWeapon(Weapon newWeapon, EquipableItem newWeaponItem)
    {
        ActivateNewWeapon(newWeapon);

        if (newWeapon.TryGetComponent(out Gun gun))
        {
            GunItem newGunItem = (GunItem)newWeaponItem;

            GunWeaponSetup(newGunItem);
            crosshair.SetupCrossHair(gun.GunConfiguration.CrossHairConfig);

            if (newGunItem.CurrentAmmo == 0)
                crosshair.OutOfBullets();
        }
        else if (newWeapon.TryGetComponent(out MeleeWeapon meleeWeapon))
        {
            MeleeWeaponSetup((MeleeItem)newWeaponItem);
            crosshair.SetupCrossHair(meleeWeapon.WeaponConfiguration.CrossHairConfig);
        }
            
    }

    private void ActivateNewWeapon(Weapon newWeapon)
    {
        if (equippedWeapon != null)
            ActivateWeapon(equippedWeapon, false);

        equippedWeapon = newWeapon;

        ActivateWeapon(equippedWeapon, true);
    }

    private void GunWeaponSetup(GunItem newGunItem)
    {
        Gun equippedGun = equippedWeapon.GetComponent<Gun>();
        equippedGun.CurrentClipAmmo = newGunItem.CurrentAmmo;

        reloadAnimationClip = equippedGun.ReloadAnimationClip;

        playerGunAnimations.Setup(equippedGun);
        GameManager.instance.GetWeaponDisplayUI().Setup(equippedGun.GunConfiguration.AmmoConfig.BulletSprite, equippedGun.CurrentClipAmmo);

        equippedGun.SetStarterAssetsInputs(starterAssetsInputs);
    }

    private void MeleeWeaponSetup(MeleeItem newMeleeItem)
    {
        MeleeWeapon equippedMeleeWeapon = equippedWeapon.GetComponent<MeleeWeapon>();
        equippedMeleeWeapon.CurrentDurability = newMeleeItem.CurrentDurability;

        GameManager.instance.GetWeaponDisplayUI().Setup(equippedMeleeWeapon.WeaponConfiguration.WeaponSprite);
    }

    private void ActivateWeapon(Weapon weapon, bool activate)
    {
        string weaponName = weapon.name;
        string containerName = "";

        if (IsSubclassOfRawGeneric(equippedWeapon.GetType(), typeof(Gun)))
            containerName = Config.GUN_CONTAINER_NAME;
        else
            containerName = Config.MELEE_WEAPON_CONTAINER_NAME;

        Transform weaponTransform = weaponContainer.Find(containerName + "/" + weaponName);

        if (weaponTransform)
            weaponTransform.gameObject.SetActive(activate);

        foreach (Rig rig in idleRigs)
        {
            Transform weaponRig = rig.transform.Find(containerName + "/" + weaponName);
            if (weaponRig)
            {
                if (weaponRig.TryGetComponent(out MultiPositionConstraint multiPositionConstraint))
                    multiPositionConstraint.weight = activate ? 1 : 0;

                if (weaponRig.TryGetComponent(out MultiParentConstraint multiParentConstraint))
                    multiParentConstraint.weight = activate ? 1 : 0;

                if (weaponRig.TryGetComponent(out MultiRotationConstraint multiRotationConstraint))
                    multiRotationConstraint.weight = activate ? 1 : 0;

                foreach (Transform child in weaponRig)
                {
                    if (child.TryGetComponent(out TwoBoneIKConstraint twoBoneIKConstraint))
                        twoBoneIKConstraint.weight = activate ? 1 : 0;
                }
            }
        }

        foreach (Rig rig in aimRigs)
        {
            if (rig.name == "Aim - Body Inclination IK")
            {
                Transform defaultRig = rig.transform.Find("Default");

                foreach (Transform child in defaultRig)
                    if (child.TryGetComponent(out MultiAimConstraint multiAimConstraint))
                        multiAimConstraint.weight = activate ? 1 : 0;

                continue;
            }

            Transform weaponRig = rig.transform.Find(containerName + "/" + weaponName);
            if (weaponRig)
            {
                if (weaponRig.TryGetComponent(out MultiPositionConstraint multiPositionConstraint))
                    multiPositionConstraint.weight = activate ? 1 : 0;

                if (weaponRig.TryGetComponent(out MultiAimConstraint multiAimConstraint))
                    multiAimConstraint.weight = activate ? 1 : 0;

                if (weaponRig.TryGetComponent(out MultiParentConstraint multiParentConstraint))
                    multiParentConstraint.weight = activate ? 1 : 0;

                if (weaponRig.TryGetComponent(out MultiRotationConstraint multiRotationConstraint1))
                    multiRotationConstraint1.weight = activate ? 1 : 0;

                foreach (Transform child in weaponRig)
                {
                    if (child.TryGetComponent(out MultiRotationConstraint multiRotationConstraint2))
                    {
                        if (child.name == "Spine Rotation")
                            multiRotationConstraint2.weight = activate ? 1 : 0;
                        else
                            multiRotationConstraint2.weight = activate ? .3f : 0;
                    }

                    if (child.TryGetComponent(out TwoBoneIKConstraint twoBoneIKConstraint))
                        twoBoneIKConstraint.weight = activate ? 1 : 0;
                }
            }
        }

        foreach (Rig rig in attackRigs)
        {
            Transform weaponRig = rig.transform.Find(containerName + "/" + weaponName);
            if (weaponRig)
            {
                if (weaponRig.TryGetComponent(out MultiPositionConstraint multiPositionConstraint))
                    multiPositionConstraint.weight = activate ? 1 : 0;

                if (weaponRig.TryGetComponent(out MultiAimConstraint multiAimConstraint))
                    multiAimConstraint.weight = activate ? 1 : 0;

                if (weaponRig.TryGetComponent(out MultiParentConstraint multiParentConstraint))
                    multiParentConstraint.weight = activate ? 1 : 0;

                if (weaponRig.TryGetComponent(out MultiRotationConstraint multiRotationConstraint1))
                    multiRotationConstraint1.weight = activate ? 1 : 0;

                foreach (Transform child in weaponRig)
                {
                    if (child.TryGetComponent(out TwoBoneIKConstraint twoBoneIKConstraint))
                        twoBoneIKConstraint.weight = activate ? 1 : 0;
                }
            }
        }
    }

    public void PlayReloadAnimation()
    {
        isReloading = true;
        starterAssetsInputs.shoot = false;

        crosshair.ReloadWeapon();

        reloadCoroutine = StartCoroutine(PlayClip(Animator.StringToHash(reloadAnimationClip.name), 0f));
    }

    public void FinishedReloading()
    {
        isReloading = false;

        equippedWeapon.GetComponent<Gun>().Reload();
    }

    public void FinishMeleeAttack()
    {
        isAttacking = false;

        if (equippedWeapon.TryGetComponent(out MeleeWeapon meleeWeapon) && meleeWeapon.CurrentDurability <= 0)
            meleeWeapon.Break();
    }

    public void PlayMeleeAttackAnimation(AnimationClip attackAnimationClip)
    {
        isAttacking = true;
        starterAssetsInputs.shoot = false;

        reloadCoroutine = StartCoroutine(PlayClip(Animator.StringToHash(attackAnimationClip.name), 0f));
    }

    public void PlayShotgunShootAnimation(float delay)
    {
        starterAssetsInputs.shoot = false;
        isAbleToReload = false;

        shotgunShootCoroutine = StartCoroutine(PlayClip(Animator.StringToHash("Shotgun Shoot"), delay));
    }

    public void FinishedShootingAnimation() 
    {
        isAbleToReload = true;
    }

    protected IEnumerator PlayClip(int clipHash, float startTime)
    {
        yield return new WaitForSeconds(startTime);

        animator.Play(clipHash);
    }

    public bool CanTriggerHurtAnimation()
    {
        return !IsAttacking && !IsReloading;
    }
}
