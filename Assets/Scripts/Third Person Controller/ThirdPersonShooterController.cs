using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalLookSensitivity = 2f;
    [SerializeField] private float aimSensitivity = 1f;

    [SerializeField] private Transform lookAt;

    [Header("Rigging")]
    [SerializeField] private List<Rig> idleRigs;
    [SerializeField] private List<Rig> aimRigs;

    [Header("Weapon")]
    [SerializeField] private Transform weaponContainer;
    [SerializeField] private Weapon equippedWeapon;

    #region Obejct References

    private ThirdPersonController thirdPersonController;
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
        EquipWeapon(equippedWeapon);
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
            starterAssetsInputs.reload &&
            equippedWeapon != null && 
            equippedWeapon.GetType() == typeof(Gun))
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

            if (equippedWeapon.GetType() == typeof(Gun) && 
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

    private void ActivateWeapon(Weapon weapon, bool activate)
    {
        string weaponName = equippedWeapon.name;
        string weaponType = equippedWeapon.GetType().Name;
        string containerName = "";

        switch (weaponType)
        {
            case Config.GUN_CLASS_NAME:
                containerName = Config.GUN_CONTAINER_NAME;
                break;

            case Config.MELEE_WEAPON_CLASS_NAME:
                containerName = Config.MELEE_WEAPON_CONTAINER_NAME;
                break;
        }

        Transform weaponTransform = weaponContainer.Find(containerName + "/" + weaponName);
        if (weaponTransform)
            weaponTransform.gameObject.SetActive(activate);

        foreach (Rig rig in idleRigs)
        {
            Transform weaponRig = rig.transform.Find(containerName + "/" + weaponName);
            if (weaponRig)
                weaponRig.gameObject.SetActive(activate);
        }

        foreach (Rig rig in aimRigs)
        {
            Transform weaponRig = rig.transform.Find(containerName + "/" + weaponName);
            if (weaponRig)
                weaponRig.gameObject.SetActive(activate);
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        if (equippedWeapon != null)
            ActivateWeapon(equippedWeapon, false);

        equippedWeapon = newWeapon;

        ActivateWeapon(equippedWeapon, true);

        if (equippedWeapon.GetType() == typeof(Gun))
        {
            Gun equippedGun = equippedWeapon.GetComponent<Gun>();

            reloadAnimationClip = equippedGun.ReloadAnimationClip;

            playerGunAnimations.Setup(equippedGun);
            GameManager.instance.GetAmmoDisplayUI().Setup(equippedGun.GunConfiguration.AmmoConfig.BulletSprite, equippedGun.CurrentClipAmmo, equippedGun.CurrentStockedAmmo);
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
