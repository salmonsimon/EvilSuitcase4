using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GunHitscan : Gun
{
    #region Configuration

    [Header("Impact Type")]
    [SerializeField] private ImpactType impactType;

    #endregion

    #region GameObject References

    private Transform bulletTrailContainer;
    private ObjectPool<TrailRenderer> TrailPool;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        bulletTrailContainer = GameObject.FindGameObjectWithTag(Config.PROYECTILE_CONTAINER_TAG).transform;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
    }

    public override void Shoot()
    {
        if (Time.time > gunConfiguration.ShootConfig.FireRate + LastshootTime)
        {
            LastshootTime = Time.time;

            ShootParticleSystem.Play();
            gunConfiguration.AudioConfig.PlayShootingClip();
            weaponAnimations.PlayShootAnimation(gunConfiguration.AmmoConfig.ShootAnimationDelay);

            if (string.Equals(GunConfiguration.GunName, "Shotgun"))
                playerThirdPersonShooterController.PlayShotgunShootAnimation(gunConfiguration.AmmoConfig.ShootAnimationDelay);

            GameManager.instance.GetCinemachineShake().ShakeCamera(gunConfiguration.ShootConfig.CameraShakeAmplitude, gunConfiguration.ShootConfig.CameraShakeDuration);

            SubstractClipAmmo();

            for (int i = 0; i < gunConfiguration.ShootConfig.PelletsPerBullet; i++)
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
            {
                damageable.ReceiveDamage(gunConfiguration.DamageConfig.GetDamage(distance));

                if (damageable.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.AddForce(-hit.normal.normalized * GunConfiguration.TrailConfig.HitForce);
            }
        }

        yield return new WaitForSeconds(gunConfiguration.TrailConfig.Duration);

        yield return null;

        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }
}
