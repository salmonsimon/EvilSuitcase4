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
    public int CurrentHitPoints { get { return currentHitPoints; } 
        set 
        {
            if (value == currentHitPoints)
                return;

            currentHitPoints = value; 

            if (OnCurrentHealthChange != null)
                OnCurrentHealthChange();
        } 
    }

    public void SetHealth(int hitPoints)
    {
        maxHitPoints = hitPoints;
        currentHitPoints = hitPoints;
    }

    private void Damage(int damage)
    {
        int damageTaken = Mathf.Clamp(damage, 0, CurrentHitPoints);
        CurrentHitPoints -= damageTaken;

        if (OnDamaged != null && currentHitPoints > 0)
            OnDamaged();
    }

    private void Recover(int recoverAmount)
    {
        int clampedRecoverAmount = Mathf.Clamp(recoverAmount, 0, maxHitPoints - currentHitPoints);
        CurrentHitPoints += clampedRecoverAmount;

        if (OnRecover != null)
            OnRecover();
    }

    public delegate void OnCurrentHealthChangeDelegate();
    public event OnCurrentHealthChangeDelegate OnCurrentHealthChange;

    public delegate void OnDamagedDelegate();
    public event OnDamagedDelegate OnDamaged;

    public delegate void OnRecoverDelegate();
    public event OnRecoverDelegate OnRecover;

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

            if (IsAlive && OnRevival != null)
                OnRevival();
            else if (!IsAlive && OnDeath != null)
                OnDeath();
        }
    }

    public delegate void OnRevivalDelegate();
    public event OnRevivalDelegate OnRevival;

    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeath;

    public delegate void OnResurrectDelegate();
    public event OnResurrectDelegate OnResurrect;

    #endregion

    #region Parameters

    [SerializeField] private bool shakeWhenDamaged = false;

    #endregion

    #region Variables

    private int lastReceivedAttackID;
    public int LastReceivedAttackID { get { return lastReceivedAttackID; } set { lastReceivedAttackID = value; } }

    #endregion

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
                GameManager.instance.GetCinemachineShake().ShakeCameras(Config.CAMERASHAKE_HIT_AMPLITUDE, Config.CAMERASHAKE_HIT_DURATION);

            if (currentHitPoints < 1)
            {
                Death();
            }
        }
    }

    public virtual void RecoverHealth(int recoverAmount)
    {
        if (IsAlive)
            Recover(recoverAmount);
    }

    protected virtual void Death()
    {
        IsAlive = false;
    }

    private IEnumerator WaitAndDestroy(float delay = 0f)
    {
        yield return null;

        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    public virtual void Resurrect()
    {
        CurrentHitPoints = maxHitPoints;

        IsAlive = true;

        if (OnResurrect != null)
            OnResurrect();
    }
}
