using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GunProjectile : Gun
{
    [Header("Projectile Configuration")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float projectileLaunchForce = 1000f;

    private bool poolableProjectiles = false;
    private ObjectPool<Projectile> projectilePool;

    private Transform projectileContainer;

    protected override void Awake()
    {
        base.Awake();

        projectileContainer = GameObject.FindGameObjectWithTag(Config.PROYECTILE_CONTAINER_TAG).transform;

        poolableProjectiles = projectilePrefab.PoolableProjectile;

        if (poolableProjectiles)
            projectilePool = new ObjectPool<Projectile>(CreateBullet);
    }

    public override void Shoot()
    {
        if (Time.time > gunConfiguration.ShootConfig.FireRate + LastshootTime)
        {
            LastshootTime = Time.time;

            ShootParticleSystem.Play();
            sfx.PlayAudioClip(gunConfiguration.AudioConfig.GetRandomShootingClip());

            if(weaponAnimations)
                weaponAnimations.PlayShootAnimation(gunConfiguration.AmmoConfig.ShootAnimationDelay);

            GameManager.instance.GetCinemachineShake().ShakeCamera(gunConfiguration.ShootConfig.CameraShakeAmplitude, gunConfiguration.ShootConfig.CameraShakeDuration);

            SubstractClipAmmo();
            crossHair.ExpandCrosshair();

            float crosshairDistance = Vector3.Distance(crossHairTarget.position, ShootParticleSystem.transform.position);

            Vector3 shootDirection = crossHairTarget.position - ShootParticleSystem.transform.position + CalculateSpread(crosshairDistance);

            shootDirection.Normalize();

            ShootProjectile(shootDirection);
        }
    }

    private void ShootProjectile(Vector3 shootDirection)
    {
        if (poolableProjectiles)
        {
            Projectile projectile = projectilePool.Get();
            projectile.gameObject.SetActive(true);

            projectile.transform.position = ShootParticleSystem.transform.position;
            projectile.LaunchProjectile(shootDirection * projectileLaunchForce);
        }
        else
        {
            Projectile newProjectile = Instantiate(projectilePrefab, projectileContainer, true);

            newProjectile.transform.position = ShootParticleSystem.transform.position;
            newProjectile.LaunchProjectile(shootDirection * projectileLaunchForce);
        }
    }

    private Projectile CreateBullet()
    {
        Projectile newProjectile = Instantiate(projectilePrefab, projectileContainer, true);

        if (poolableProjectiles)
            newProjectile.ProjectilePool = projectilePool;

        return newProjectile;
    }
}
