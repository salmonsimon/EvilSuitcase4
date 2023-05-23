using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage System/Damage Configuration", fileName = "DamageConfiguration", order = 0)]
public class DamageDealerConfigScriptableObject : ScriptableObject
{
    [SerializeField] private Vector2Int minMaxDamage;
    public Vector2Int MinMaxDamage { get { return minMaxDamage; } }
}
