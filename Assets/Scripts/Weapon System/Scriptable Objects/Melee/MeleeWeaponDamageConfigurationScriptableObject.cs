using UnityEngine;

[CreateAssetMenu(menuName = "Melee Weapons/Damage Configuration", fileName = "DamageConfiguration", order = 2)]
public class MeleeWeaponDamageConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private Vector2Int damageRange;
    public Vector2Int DamageRange { get { return damageRange; } }

    [SerializeField] private int force;
    public int Force { get { return force; } }

    public int GetDamage()
    {
        return Random.Range(damageRange.x, damageRange.y);
    }
}
