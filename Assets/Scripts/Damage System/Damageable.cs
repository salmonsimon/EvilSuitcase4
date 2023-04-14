using UnityEngine;

public class Damageable : MonoBehaviour
{
    protected HealthManager healthManager;
    public HealthManager HealthManager { get { return healthManager; } }

    protected virtual void Awake()
    {
        healthManager = GetComponent<HealthManager>();
    }

    public virtual void ReceiveDamage(int damage, Vector3 Force)
    {
        healthManager.ReceiveDamage(damage);

        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.AddForce(Force);
        }
    }

    public virtual void ReceiveDamage(int damage, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        healthManager.ReceiveDamage(damage);

        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        }
    }
}
