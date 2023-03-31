using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class HealthManager : MonoBehaviour
{
    #region MaxHitPoints

    [SerializeField] private int maxHitPoints = 1;
    public int MaxHitPoints { get { return maxHitPoints; } }

    #endregion

    #region CurrentHitPoints

    [SerializeField] private int currentHitPoints = 1;
    public int CurrentHitPoints { get { return currentHitPoints; } set { currentHitPoints = value; } }

    public void SetHealth(int hitPoints)
    {
        maxHitPoints = hitPoints;
        currentHitPoints = hitPoints;
    }

    private void Damage(int damage)
    {
        int damageTaken = Mathf.Clamp(damage, 0, currentHitPoints);
        currentHitPoints -= damageTaken;

        if (OnDamaged != null && currentHitPoints > 0)
            OnDamaged();
    }

    public delegate void OnDamagedDelegate();
    public event OnDamagedDelegate OnDamaged;

    #endregion

    #region IsAlive

    [SerializeField] private bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
        private set
        {
            if (isAlive == value) return;

            isAlive = value;
            if (OnAliveStatusChange != null)
                OnAliveStatusChange();
        }
    }

    public delegate void OnAliveStatusChangeDelegate();
    public event OnAliveStatusChangeDelegate OnAliveStatusChange;

    #endregion

    [SerializeField] private bool shakeWhenDamaged = false;

    private void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    public virtual void ReceiveDamage(int damage)
    {
        if (IsAlive)
        {
            Damage(damage);

            if (shakeWhenDamaged)
                GameManager.instance.GetCinemachineShake().ShakeCamera(Config.CAMERASHAKE_HIT_AMPLITUDE, Config.CAMERASHAKE_HIT_DURATION);

            if (currentHitPoints < 1)
            {
                Death();
            }
        }
    }

    protected virtual void Death()
    {
        IsAlive = false;

        /*
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<ParticleSystem>(out ParticleSystem particleSystem))
                GameManager.instance.GetSurfaceManager().DisableEffect(particleSystem.gameObject);
        }
        */

        // TO DO: DELETE THIS LATER, THIS IS FOR TESTING
        //StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy(float delay = 0f)
    {
        yield return null;

        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    public virtual void Resurrect()
    {
        currentHitPoints = maxHitPoints;
        IsAlive = true;
    }
}
