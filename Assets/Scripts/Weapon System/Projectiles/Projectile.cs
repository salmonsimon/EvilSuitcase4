using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    #region GameObject References

    private Rigidbody rigidBody;
    private BoxCollider boxCollider;

    #endregion

    #region Projectile Configuration

    [Header("Projectile Configuration")]
    [SerializeField] private float maxDistance;

    [SerializeField] private DamageConfigurationScriptableObject damageConfig;
    public DamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } set { damageConfig = value; } }

    [SerializeField] private bool poolableProjectile = false;
    public bool PoolableProjectile { get { return poolableProjectile; } }

    private ObjectPool<Projectile> projectilePool;
    public ObjectPool<Projectile> ProjectilePool { get { return projectilePool; } set { projectilePool = value; } }

    #endregion

    #region Logic Variables

    private bool isDisabled = false;

    private Vector3 startPosition;
    private float distanceTraveled;

    #endregion

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    protected virtual void Update()
    {
        if (isDisabled) 
            return;

        distanceTraveled = Vector3.Distance(startPosition, transform.position);

        if (distanceTraveled > maxDistance)
        {
            Disable();
        }
    }

    public virtual void LaunchProjectile(Vector3 launchForce)
    {
        startPosition = transform.position;

        transform.forward = launchForce.normalized;

        rigidBody.AddForce(launchForce);
    }

    protected virtual void Disable()
    {
        if (poolableProjectile)
        {
            gameObject.SetActive(false);
            projectilePool.Release(this);
        }
        else
        {
            isDisabled = true;
            Destroy(boxCollider);
            Destroy(rigidBody);
        }
    }

    protected virtual void OnDisable()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Damageable damageable))
            damageable.Damage(damageConfig.GetDamage(distanceTraveled));
    }
}
