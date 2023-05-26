using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Explosive : MonoBehaviour
{
    #region Game Object Components

    private AudioSource audioSource;

    #endregion

    #region References

    [SerializeField] private GameObject model;
    [SerializeField] private Explosion explosionObject;

    private HealthManager modelHealthManager;
    private Rigidbody modelRigidBody;
    private Collider modelCollider;

    #endregion

    #region Parameters

    private float disableTime = Config.EXPLOSIVE_DISABLE_TIME;

    #endregion

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        modelHealthManager = model.GetComponent<HealthManager>();
        modelRigidBody = model.GetComponent<Rigidbody>();
        modelCollider = model.GetComponent<Collider>();
    }

    private void Start()
    {
        audioSource.volume = GameManager.instance.GetSFXManager().GetSFXVolume();
    }

    private void OnEnable()
    {
        modelHealthManager.OnDeath += Explode;
    }

    private void OnDisable()
    {
        modelHealthManager.OnDeath -= Explode;
    }

    private void Explode()
    {
        audioSource.Play();

        explosionObject.transform.SetParent(transform);

        foreach (Transform particleEffect in model.transform)
            Destroy(particleEffect.gameObject);

        model.SetActive(false);
        modelRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        modelCollider.enabled = false;

        explosionObject.gameObject.SetActive(true);

        StartCoroutine(WaitToDisable(disableTime));
    }

    private IEnumerator WaitToDisable(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }
}
