using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartProjectile : Projectile
{
    [SerializeField] private GunDamageConfigurationScriptableObject damageConfig;
    public GunDamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } set { damageConfig = value; } }

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
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled), forceDirection * 40f);
        else if (collision.collider.TryGetComponent(out Rigidbody rigidbody))
            rigidbody.AddForce(forceDirection * 5f, ForceMode.Impulse);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        transform.SetParent(other.transform, true);

        Disable();

        Vector3 forceDirection = (other.transform.position - GameManager.instance.GetPlayer().transform.position).normalized;

        if (other.TryGetComponent(out Damageable damageable))
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled), forceDirection * 40f);
    }

    protected override void Disable()
    {
        Destroy(gameObject.GetComponent<CustomGravity>());

        base.Disable();
    }
}
