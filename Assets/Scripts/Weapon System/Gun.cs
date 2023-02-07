using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    private float LastshootTime;
    private ParticleSystem ShootParticleSystem;
    private ObjectPool<TrailRenderer> TrailPool;

    /*
    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour)
    {
        LastshootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        Model = Instantiate(ModelPrefab);
        Model.transform.SetParent(parent, false);
        Model.transform.localPosition = SpawnPoint;
        Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

        ShootParticleSystem = Model.GetComponentInChildren<ParticleSystem>();
    }

    public void Shoot()
    {
        if (Time.time > ShootConfig.FireRate + LastshootTime)
        {
            LastshootTime = Time.time;
            //ShootParticleSystem.Play();

            Vector3 shootDirection = ShootParticleSystem.transform.forward +
                new Vector3(
                    Random.Range(-ShootConfig.Spread.x, ShootConfig.Spread.x),
                    Random.Range(-ShootConfig.Spread.y, ShootConfig.Spread.y),
                    Random.Range(-ShootConfig.Spread.z, ShootConfig.Spread.z)
                    );

            shootDirection.Normalize();

            if (Physics.Raycast(ShootParticleSystem.transform.position, shootDirection,
                                out RaycastHit hit, float.MaxValue, ShootConfig.HitMask))
            {
                ActiveMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        ShootParticleSystem.transform.position,
                        hit.point,
                        hit
                        )
                    );
            }
            else
            {
                ActiveMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        ShootParticleSystem.transform.position,
                        ShootParticleSystem.transform.position + (shootDirection * TrailConfig.MaxDistance),
                        new RaycastHit()
                        )
                    );
            }
        }
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

            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPosition;

        
        if (hit.collider != null)
        {
            SurfaceManager.Instance.HandleImpact(hit.transform.gameObject, endPosition, hit.normal, ImpactType, 0);
        }
        

        yield return new WaitForSeconds(TrailConfig.Duration);

        yield return null;

        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");

        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = trail.material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    */
}
