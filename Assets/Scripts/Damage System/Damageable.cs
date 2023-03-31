using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] protected HealthManager healthManager;

    public virtual void ReceiveDamage(int damage)
    {
        healthManager.ReceiveDamage(damage);
    }
}
