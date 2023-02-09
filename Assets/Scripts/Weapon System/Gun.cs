using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : Weapon
{
    [SerializeField] private GunScriptableObject gunConfiguration;
    [SerializeField] private ImpactType impactType;

    private float LastshootTime;
    private ParticleSystem ShootParticleSystem;
    private Transform bulletTrailContainer;
    private ObjectPool<TrailRenderer> TrailPool;

    private Transform crossHairTarget;

    private void Awake()
    {
        ShootParticleSystem = GetComponentInChildren<ParticleSystem>();
        bulletTrailContainer = GameObject.FindGameObjectWithTag(Config.PROYECTILE_CONTAINER_TAG).transform;
        crossHairTarget = GameObject.FindGameObjectWithTag(Config.CROSSHAIR_TAG).transform;

        LastshootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
    }

    public override void Attack()
    {
        Shoot();
    }

    public void Shoot()
    {
        if (Time.time > gunConfiguration.ShootConfig.FireRate + LastshootTime)
        {
            LastshootTime = Time.time;
            ShootParticleSystem.Play();
            GameManager.instance.GetCinemachineShake().ShakeCamera(gunConfiguration.ShootConfig.CameraShakeAmplitude, gunConfiguration.ShootConfig.CameraShakeDuration);

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
}
