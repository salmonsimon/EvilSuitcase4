using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartProjectile : Projectile
{
    [SerializeField] private GunDamageConfigurationScriptableObject damageConfig;
    public GunDamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } set { damageConfig = value; } }

    [SerializeField] protected ImpactType impactType;

    protected void FixedUpdate()
    {
        if (!isDisabled)
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, launchForce.z);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        transform.SetParent(collision.collider.transform);
        Disable();

        Vector3 forceDirection = -collision.contacts[0].normal;

        if (collision.collider.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled), forceDirection * 40f);

            if (damageable.TryGetComponent(out HumanoidHurtGeometry humanoidHurtGeometry))
            {
                GameManager.instance.GetBloodManager().SpawnBloodOnHit(humanoidHurtGeometry.transform, collision.contacts[0].point, forceDirection);
                GameManager.instance.GetSurfaceManager().HandleFleshImpact(damageable.transform.gameObject, collision.contacts[0].point, collision.contacts[0].normal, impactType, 0);
            }
        }
        else if (collision.collider.TryGetComponent(out Rigidbody rigidbody))
            rigidbody.AddForce(forceDirection * 5f, ForceMode.Impulse);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        transform.SetParent(other.transform, true);

        Disable();

        Vector3 forceDirection = (other.transform.position - transform.position).normalized;

        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled), forceDirection * 40f);

            if (damageable.TryGetComponent(out HumanoidHurtGeometry humanoidHurtGeometry))
            {
                Vector3 closestPosition = other.ClosestPoint(transform.position);

                GameManager.instance.GetBloodManager().SpawnBloodOnHit(humanoidHurtGeometry.transform, closestPosition, -forceDirection);
                GameManager.instance.GetSurfaceManager().HandleFleshImpact(damageable.transform.gameObject, closestPosition, -forceDirection, impactType, 0);
            }
        }
        else if (other.TryGetComponent(out Rigidbody rigidbody))
            rigidbody.AddForce(forceDirection * 5f, ForceMode.Impulse);
    }

    protected override void Disable()
    {
        Destroy(gameObject.GetComponent<CustomGravity>());

        base.Disable();
    }
}
