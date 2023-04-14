using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(SFX))]
public class Projectile : MonoBehaviour
{
    #region GameObject References

    protected Rigidbody rigidBody;
    protected BoxCollider boxCollider;
    protected SFX sfx;

    #endregion

    #region Projectile Configuration

    [Header("Projectile Configuration")]
    [SerializeField] protected float maxDistance;

    [SerializeField] protected bool poolableProjectile = false;
    public bool PoolableProjectile { get { return poolableProjectile; } }

    protected ObjectPool<Projectile> projectilePool;
    public ObjectPool<Projectile> ProjectilePool { get { return projectilePool; } set { projectilePool = value; } }

    [SerializeField] protected AudioClip hitAudioClip;

    #endregion

    #region Logic Variables

    protected bool isDisabled = false;

    protected Vector3 startPosition;
    protected float distanceTraveled;

    #endregion

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        sfx = GetComponent<SFX>();
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

        if (rigidBody.velocity.magnitude > 0)
            transform.rotation.SetLookRotation(rigidBody.velocity);
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

            if (rigidBody)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
        }
    }

    protected virtual void OnDisable()
    {
        if (rigidBody)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        sfx.PlayAudioClip(hitAudioClip);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        sfx.PlayAudioClip(hitAudioClip);
    }
}
