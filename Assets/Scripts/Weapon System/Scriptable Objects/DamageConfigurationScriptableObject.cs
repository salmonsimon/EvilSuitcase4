using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(menuName = "Guns/Damage Configuration", fileName = "DamageConfiguration", order = 5)]
public class DamageConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private MinMaxCurve damageCurve;
    public MinMaxCurve DamageCurve { get { return damageCurve; } }

    private void Reset()
    {
        damageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float distance = 0)
    {
        return Mathf.CeilToInt(DamageCurve.Evaluate(distance, Random.value));
    }
}
