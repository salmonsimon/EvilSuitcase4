using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GunBackup : Weapon
{
    #region Configuration

    [Header("Gun Configuration")]
    [SerializeField] private GunScriptableObject gunConfiguration;
    public GunScriptableObject GunConfiguration { get { return gunConfiguration; } }

    [SerializeField] private ImpactType impactType;

    #endregion

    #region Weapon Prefabs

    [Header("Weapon Prefabs")]
    [SerializeField] private GameObject magazine;
    public GameObject Magazine { get { return magazine; } }

    #endregion

    #region Animation

    [Header("Animation")]
    [SerializeField] private AnimationClip reloadAnimationClip;
    public AnimationClip ReloadAnimationClip { get { return reloadAnimationClip; } }

    private GunAnimations gunAnimations;

    #endregion

    #region Logic Variables

    private float LastshootTime;

    private int currentClipAmmo;
    public int CurrentClipAmmo { get { return currentClipAmmo; } }

    private int currentStockedAmmo;
    public int CurrentStockedAmmo { get { return currentStockedAmmo; } }

    #endregion

    #region GameObject References

    private ParticleSystem ShootParticleSystem;
    private Transform bulletTrailContainer;
    private ObjectPool<TrailRenderer> TrailPool;

    private Transform crossHairTarget;

    private ThirdPersonShooterController playerThirdPersonShooterController;

    private AmmoDisplayUI ammoDisplayUI;
    
    #endregion

    private void Awake()
    {
        ShootParticleSystem = GetComponentInChildren<ParticleSystem>();
        bulletTrailContainer = GameObject.FindGameObjectWithTag(Config.PROYECTILE_CONTAINER_TAG).transform;
        crossHairTarget = GameObject.FindGameObjectWithTag(Config.CROSSHAIR_TAG).transform;
        gunAnimations = GetComponentInChildren<GunAnimations>();

        LastshootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

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

    public void Shoot()
    {
        if (Time.time > gunConfiguration.ShootConfig.FireRate + LastshootTime)
        {
            LastshootTime = Time.time;

            ShootParticleSystem.Play();
            gunConfiguration.AudioConfig.PlayShootingClip();
            gunAnimations.PlayShootAnimation(gunConfiguration.AmmoConfig.ShootAnimationDelay);
            GameManager.instance.GetCinemachineShake().ShakeCamera(gunConfiguration.ShootConfig.CameraShakeAmplitude, gunConfiguration.ShootConfig.CameraShakeDuration);

            SubstractClipAmmo();

            for(int i = 0; i < gunConfiguration.ShootConfig.PelletsPerBullet; i++)
            {
                Vector3 shootDirection = crossHairTarget.position - ShootParticleSystem.transform.position +
                new Vector3(
                    Random.Range(-gunConfiguration.ShootConfig.Spread.x, gunConfiguration.ShootConfig.Spread.x),
                    Random.Range(-gunConfiguration.ShootConfig.Spread.y, gunConfiguration.ShootConfig.Spread.y),
                    Random.Range(-gunConfiguration.ShootConfig.Spread.z, gunConfiguration.ShootConfig.Spread.z)
                    );

                shootDirection.Normalize();

                if (Physics.Raycast(ShootParticleSystem.transform.position, shootDirection,
                                    out RaycastHit hit, float.MaxValue, gunConfiguration.ShootConfig.HitMask))
                {
                    StartCoroutine(PlayTrail(ShootParticleSystem.transform.position, hit.point, hit));
                }
                else
                {
                    StartCoroutine(
                        PlayTrail(
                                  ShootParticleSystem.transform.position,
                                  ShootParticleSystem.transform.position + (shootDirection * gunConfiguration.TrailConfig.MaxDistance),
                                  new RaycastHit()
                                 )
                        );
                }
            }
        }
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        instance.transform.parent = bulletTrailContainer;

        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = gunConfiguration.TrailConfig.Color;
        trail.material = gunConfiguration.TrailConfig.Material;
        trail.widthCurve = gunConfiguration.TrailConfig.WidthCurve;
        trail.time = gunConfiguration.TrailConfig.Duration;
        trail.minVertexDistance = gunConfiguration.TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    private IEnumerator PlayTrail(Vector3 startPosition, Vector3 endPosition, RaycastHit hit)
    {
        TrailRenderer instance = TrailPool.Get();

        instance.gameObject.SetActive(true);
        instance.transform.position = startPosition;

        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPosition, endPosition);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01(1 - (remainingDistance / distance)));

            remainingDistance -= gunConfiguration.TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPosition;

        
        if (hit.collider != null)
        {
            GameManager.instance.GetSurfaceManager().HandleImpact(hit.transform.gameObject, endPosition, hit.normal, impactType, 0);

            if (hit.collider.TryGetComponent(out Damageable damageable))
                damageable.ReceiveDamage(gunConfiguration.DamageConfig.GetDamage(distance));
        }

        yield return new WaitForSeconds(gunConfiguration.TrailConfig.Duration);

        yield return null;

        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
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

    private void SubstractClipAmmo()
    {
        currentClipAmmo--;
        UpdateAmmoDisplayCounters();
    }

    private void UpdateAmmoDisplayCounters()
    {
        ammoDisplayUI.UpdateCounters(CurrentClipAmmo, CurrentStockedAmmo);
    }
}
