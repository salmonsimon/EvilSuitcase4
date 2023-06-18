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

    private bool initialized = false;

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

        GameManager.instance.GetPauseMenuUI().OnPause += OnPause;
        GameManager.instance.GetPauseMenuUI().OnResume += OnResume;

        initialized = true;
    }

    private void OnEnable()
    {
        modelHealthManager.OnDeath += Explode;

        if (initialized)
        {
            GameManager.instance.GetPauseMenuUI().OnPause += OnPause;
            GameManager.instance.GetPauseMenuUI().OnResume += OnResume;
        }
        
    }

    private void OnDisable()
    {
        modelHealthManager.OnDeath -= Explode;

        if (initialized)
        {
            GameManager.instance.GetPauseMenuUI().OnPause -= OnPause;
            GameManager.instance.GetPauseMenuUI().OnResume -= OnResume;
        }
    }

    private void OnPause()
    {
        audioSource.Pause();
    }

    private void OnResume()
    {
        audioSource.UnPause();
    }

    private void Explode()
    {
        audioSource.minDistance = 10;
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

        explosionObject.gameObject.SetActive(false);

        yield return new WaitForSeconds(delay * 2);

        Destroy(gameObject);
    }
}
