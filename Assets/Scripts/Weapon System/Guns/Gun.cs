using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Gun : Weapon
{
    #region Configuration

    [Header("Gun Configuration")]
    [SerializeField] protected GunScriptableObject gunConfiguration;
    public GunScriptableObject GunConfiguration { get { return gunConfiguration; } }

    [SerializeField] protected AmmoType ammoType;
    public AmmoType AmmoType { get { return ammoType; } }

    #endregion

    #region Weapon Prefabs

    [Header("Weapon Prefabs")]
    [SerializeField] private GameObject animationObject;
    public GameObject AnimationObject { get { return animationObject; } }

    #endregion

    #region Animation

    [Header("Animation")]
    [SerializeField] protected AnimationClip reloadAnimationClip;
    public AnimationClip ReloadAnimationClip { get { return reloadAnimationClip; } }

    protected GunAnimations weaponAnimations;

    #endregion

    #region Logic Variables

    [SerializeField] protected float continuousShootingTime = 0f;
    [SerializeField] protected Vector2 currentRecoilRotation = Vector2.zero;
    [SerializeField] protected Vector2 targetRecoilRotation = Vector2.zero;

    protected float LastshootTime;

    [SerializeField] protected int currentClipAmmo;
    public int CurrentClipAmmo { get { return currentClipAmmo; } set { currentClipAmmo = value; } }

    public int CurrentStockedAmmo { get { return GameManager.instance.GetInventoryManager().StockedAmmoDictionary[ammoType]; }}

    #endregion

    #region GameObject References

    protected ParticleSystem ShootParticleSystem;

    protected Transform crossHairTarget;

    protected ThirdPersonShooterController playerThirdPersonShooterController;

    protected AmmoDisplayUI ammoDisplayUI;

    protected StarterAssetsInputs starterAssetInputs;
    
    #endregion

    protected virtual void Awake()
    {
        ShootParticleSystem = GetComponentInChildren<ParticleSystem>();
        crossHairTarget = GameObject.FindGameObjectWithTag(Config.CROSSHAIR_TAG).transform;
        weaponAnimations = GetComponentInChildren<GunAnimations>();

        LastshootTime = -gunConfiguration.ShootConfig.FireRate;

        playerThirdPersonShooterController = GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>();

        ammoDisplayUI = GameManager.instance.GetAmmoDisplayUI();
    }

    private void Update()
    {
        if (starterAssetInputs.shoot)
            continuousShootingTime += Time.deltaTime;
        else
            continuousShootingTime = 0;

        UpdateCameraRecoilRotation();
    }

    public override void Attack()
    {
        if (currentClipAmmo > 0)
        {
            Shoot();
        }
        else
        {
            if (CanReload() && playerThirdPersonShooterController.IsAbleToReload)
                playerThirdPersonShooterController.PlayReloadAnimation();
            else
                sfx.PlayAudioClip(gunConfiguration.AudioConfig.EmptyClip);
        }
    }

    public virtual void Shoot()
    {
        
    }

    public void Reload()
    {
        int maxReloadAmount = Mathf.Min(gunConfiguration.AmmoConfig.ClipSize, CurrentStockedAmmo);
        int bulletsToFillClip = gunConfiguration.AmmoConfig.ClipSize - currentClipAmmo;
        int reloadAmount = Mathf.Min(maxReloadAmount, bulletsToFillClip);

        currentClipAmmo += reloadAmount;
        GameManager.instance.GetInventoryManager().EquippedItem.GetComponent<GunItem>().CurrentAmmo += reloadAmount;

        InventoryManager inventoryManager = GameManager.instance.GetInventoryManager();

        List<AmmoItem> inventoryAmmoItemList = inventoryManager.AmmoItemListDictionary[ammoType];

        while (reloadAmount > 0)
        {
            AmmoItem lastAmmoItemInInventoryList = inventoryAmmoItemList[inventoryAmmoItemList.Count - 1];

            if (reloadAmount >= lastAmmoItemInInventoryList.CurrentAmmo)
            {
                reloadAmount -= lastAmmoItemInInventoryList.CurrentAmmo;

                inventoryAmmoItemList.Remove(lastAmmoItemInInventoryList);
                lastAmmoItemInInventoryList.Discard();
            }
            else
            {
                inventoryManager.StockedAmmoDictionary[ammoType] -= reloadAmount;

                lastAmmoItemInInventoryList.CurrentAmmo -= reloadAmount;

                reloadAmount = 0;
            }
        }

        UpdateAmmoDisplayCounters();
    }

    public bool CanReload()
    {
        return (currentClipAmmo < gunConfiguration.AmmoConfig.ClipSize) && (CurrentStockedAmmo > 0);
    }

    protected virtual void SubstractClipAmmo()
    {
        currentClipAmmo--;
        GameManager.instance.GetInventoryManager().EquippedItem.GetComponent<GunItem>().CurrentAmmo--;

        UpdateAmmoDisplayCounters();
    }

    protected virtual void UpdateAmmoDisplayCounters()
    {
        ammoDisplayUI.UpdateCounters(CurrentClipAmmo, CurrentStockedAmmo);
    }

    protected Vector3 CalculateSpread(float crosshairDistance)
    {
        return new Vector3(
                    Random.Range(-GunConfiguration.ShootConfig.Spread.x, GunConfiguration.ShootConfig.Spread.x) * crosshairDistance,
                    Random.Range(-GunConfiguration.ShootConfig.Spread.y, GunConfiguration.ShootConfig.Spread.y) * crosshairDistance,
                    Random.Range(-GunConfiguration.ShootConfig.Spread.z, GunConfiguration.ShootConfig.Spread.z) * crosshairDistance
                    );
    }

    protected void Recoil()
    {
        Vector2 recoil = Vector3.Lerp(
            Vector2.zero,
            new Vector2(
                Random.Range(-GunConfiguration.RecoilConfig.RecoilPattern.x, GunConfiguration.RecoilConfig.RecoilPattern.x),
                Random.Range(0, GunConfiguration.RecoilConfig.RecoilPattern.y)
                ),
            Mathf.Clamp01(continuousShootingTime / GunConfiguration.RecoilConfig.MaxRecoilTime)
        );

        targetRecoilRotation += recoil;
    }

    protected void UpdateCameraRecoilRotation()
    {
        targetRecoilRotation = Vector2.Lerp(targetRecoilRotation, Vector3.zero, GunConfiguration.RecoilConfig.RecoilRecoverySpeed * Time.deltaTime);
        currentRecoilRotation = Vector2.Lerp(currentRecoilRotation, targetRecoilRotation, GunConfiguration.RecoilConfig.RecoilSnappiness * Time.deltaTime);

        playerThirdPersonShooterController.ThirdPersonController.recoil = currentRecoilRotation;
    }

    public void SetStarterAssetsInputs(StarterAssetsInputs starterAssetsInputs)
    {
        this.starterAssetInputs = starterAssetsInputs;
    }
}
