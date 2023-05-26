using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartProjectile : Projectile
{
    [SerializeField] private GunDamageConfigurationScriptableObject damageConfig;
    public GunDamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } set { damageConfig = value; } }

    protected override void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided on collision enter");

        base.OnCollisionEnter(collision);

        transform.SetParent(collision.collider.transform);
        Disable();

        Vector3 forceDirection = -collision.contacts[0].normal;

        if (collision.collider.TryGetComponent(out Damageable damageable))
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled), forceDirection * 40f);
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collided on trigger enter");

        base.OnTriggerEnter(collider);

        transform.SetParent(collider.transform, true);
        Disable();

        Vector3 forceDirection = (collider.transform.position - GameManager.instance.GetPlayer().transform.position).normalized;

        if (collider.TryGetComponent(out Damageable damageable))
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled), forceDirection * 40f);
    }
}
