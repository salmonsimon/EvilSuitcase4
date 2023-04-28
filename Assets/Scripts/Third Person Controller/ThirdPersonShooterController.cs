using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using static Utils;
using System;

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

    [Header("Weapon")]
    [SerializeField] private Transform weaponContainer;
    private Weapon equippedWeapon;

    #region Obejct References

    private ThirdPersonController thirdPersonController;
    public ThirdPersonController ThirdPersonController { get { return thirdPersonController; } }

    private StarterAssetsInputs starterAssetsInputs;
    private PlayerGunAnimations playerGunAnimations;

    private Animator animator;

    #endregion

    #region Logic Variables

    private Vector3 aimDirection;
    private bool aiming = false;

    private bool isReloading = false;
    private AnimationClip reloadAnimationClip = null;
    private Coroutine reloadCoroutine = null;

    private bool isAbleToReload = true;
    private Coroutine shotgunShootCoroutine = null;

    #endregion

    #region Parameters

    private float movementSpeed = 5f;
    private float aimMovementSpeed = 3f;

    #endregion

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        playerGunAnimations = GetComponent<PlayerGunAnimations>();

        animator = GetComponent<Animator>();
        //EquipWeapon(equippedWeapon, equippedWeaponItem);
    }

    private void Update()
    {
        if (starterAssetsInputs.aim && !isReloading)
        {
            aiming = true;

            aimVirtualCamera.gameObject.SetActive(true);

            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            thirdPersonController.SetAbleToSprint(false);
            thirdPersonController.MoveSpeed = aimMovementSpeed;
        }
        else 
        {
            aiming = false;
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
                starterAssetsInputs.shoot = false;
        }
    }

    private void FixedUpdate()
    {
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

    public void FindAndEquipWeapon(EquipableItem equipableItem)
    {
        string containerName = "";

        if (equipableItem.GetItemSO().itemType == ItemSO.ItemType.Melee)
            containerName = Config.MELEE_WEAPON_CONTAINER_NAME;
        else if (equipableItem.GetItemSO().itemType == ItemSO.ItemType.Gun)
            containerName = Config.GUN_CONTAINER_NAME;

        Transform weaponTransform = weaponContainer.Find(containerName + "/" + equipableItem.GetItemSO().itemName);

        if (weaponTransform && weaponTransform.TryGetComponent(out Weapon weaponToEquip))
            EquipWeapon(weaponToEquip, equipableItem);
    }

    public void EquipWeapon(Weapon newWeapon, EquipableItem newWeaponItem)
    {
        ActivateNewWeapon(newWeapon);

        if (IsSubclassOfRawGeneric(equippedWeapon.GetType(), typeof(Gun)))
            GunWeaponSetup((GunItem)newWeaponItem);
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
        GameManager.instance.GetAmmoDisplayUI().Setup(equippedGun.GunConfiguration.AmmoConfig.BulletSprite, equippedGun.CurrentClipAmmo);

        equippedGun.SetStarterAssetsInputs(starterAssetsInputs);
    }

    private void ActivateWeapon(Weapon weapon, bool activate)
    {
        string weaponName = equippedWeapon.name;
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

                foreach (Transform child in weaponRig)
                {
                    if (child.TryGetComponent(out MultiRotationConstraint multiRotationConstraint))
                    {
                        if (child.name == "Spine Rotation")
                            multiRotationConstraint.weight = activate ? 1 : 0;
                        else
                            multiRotationConstraint.weight = activate ? .3f : 0;
                    }

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

        reloadCoroutine = StartCoroutine(PlayClip(Animator.StringToHash(reloadAnimationClip.name), 0f));
    }

    public void FinishedReloading()
    {
        isReloading = false;
        equippedWeapon.GetComponent<Gun>().Reload();
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

        /*
        if (isAlive)
        {
            animator.Play(clipHash);
        }
        */
    }
}
