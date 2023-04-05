using UnityEngine;

[RequireComponent(typeof(SFX))]
public abstract class Weapon : MonoBehaviour
{
    protected SFX sfx;

    public virtual void Attack()
    {

    }

    protected virtual void Start()
    {
        sfx = GetComponent<SFX>();
    }
}
