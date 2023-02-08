using UnityEngine;

[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Gun Trail Configuration", order = 4)]
public class TrailConfigurationScriptableObject : ScriptableObject

{
    public Material Material;
    public AnimationCurve WidthCurve;
    public float Duration = .5f;
    public float MinVertexDistance = .1f;
    public Gradient Color;

    public float MaxDistance = 100f;
    public float SimulationSpeed = 100f;
}
