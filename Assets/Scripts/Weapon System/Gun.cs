using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : Weapon
{
    #region Configuration

    [Header("Gun Configuration")]
    [SerializeField] protected GunScriptableObject gunConfiguration;
    public GunScriptableObject GunConfiguration { get { return gunConfiguration; } }

    #endregion

    #region Weapon Prefabs

    [Header("Weapon Prefabs")]
    [SerializeField] private GameObject magazine;
    public GameObject Magazine { get { return magazine; } }

    #endregion

    #region Animation

    [Header("Animation")]
    [SerializeField] protected AnimationClip reloadAnimationClip;
    public AnimationClip ReloadAnimationClip { get { return reloadAnimationClip; } }

    protected GunAnimations gunAnimations;

    #endregion

    #region Logic Variables

    protected float LastshootTime;

    protected int currentClipAmmo;
    public int CurrentClipAmmo { get { return currentClipAmmo; } }

    protected int currentStockedAmmo;
    public int CurrentStockedAmmo { get { return currentStockedAmmo; } }

    #endregion

    #region GameObject References

    protected ParticleSystem ShootParticleSystem;

    protected Transform crossHairTarget;

    protected ThirdPersonShooterController playerThirdPersonShooterController;

    protected AmmoDisplayUI ammoDisplayUI;
    
    #endregion

    protected virtual void Awake()
    {
        ShootParticleSystem = GetComponentInChildren<ParticleSystem>();
        crossHairTarget = GameObject.FindGameObjectWithTag(Config.CROSSHAIR_TAG).transform;
        gunAnimations = GetComponentInChildren<GunAnimations>();

        LastshootTime = 0;

        currentClipAmmo = gunConfiguration.AmmoConfig.ClipSize;
        currentStockedAmmo = gunConfiguration.AmmoConfig.ClipSize * 5;

        playerThirdPersonShooterController = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG).GetComponent<ThirdPersonShooterController>();

        ammoDisplayUI = GameManager.instance.GetAmmoDisplayUI();
    }

    public override void Attack()
    {
        if (currentClipAmmo > 0)
        {
            Shoot();
        }
        else
        {
            if (CanReload())
                playerThirdPersonShooterController.PlayReloadAnimation();
            else
                gunConfiguration.AudioConfig.PlayEmptyClip();
        }
    }

    public virtual void Shoot()
    {
        
    }

    public void Reload()
    {
        int maxReloadAmount = Mathf.Min(gunConfiguration.AmmoConfig.ClipSize, currentStockedAmmo);
        int bulletsToFillClip = gunConfiguration.AmmoConfig.ClipSize - currentClipAmmo;
        int reloadAmount = Mathf.Min(maxReloadAmount, bulletsToFillClip);

        currentClipAmmo += reloadAmount;
        currentStockedAmmo -= reloadAmount;

        UpdateAmmoDisplayCounters();
    }

    public bool CanReload()
    {
        return (currentClipAmmo < gunConfiguration.AmmoConfig.ClipSize) && (currentStockedAmmo > 0);
    }

    protected virtual void SubstractClipAmmo()
    {
        currentClipAmmo--;
        UpdateAmmoDisplayCounters();
    }

    protected virtual void UpdateAmmoDisplayCounters()
    {
        ammoDisplayUI.UpdateCounters(CurrentClipAmmo, CurrentStockedAmmo);
    }
}
