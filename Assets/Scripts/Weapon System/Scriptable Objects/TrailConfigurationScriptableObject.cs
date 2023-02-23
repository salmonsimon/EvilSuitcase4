using UnityEngine;

[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Gun Trail Configuration", order = 4)]
public class TrailConfigurationScriptableObject : ScriptableObject

{
    #region Trail Configuration

    [Header("Trail Configuration")]
    [SerializeField] private Material material;
    public Material Material { get { return material; } }

    [SerializeField] private AnimationCurve widthCurve;
    public AnimationCurve WidthCurve { get { return widthCurve; } }

    [SerializeField] private float duration = .1f;
    public float Duration { get { return duration; } }

    [SerializeField] private float minVertexDistance = .1f;
    public float MinVertexDistance { get { return minVertexDistance; } }

    [SerializeField] private Gradient color;
    public Gradient Color { get { return color; } }

    #endregion

    #region Hitscan Shoot Configuration

    [Header("Hitscan Shoot Configuration")]
    [SerializeField] private float maxDistance = 100f;
    public float MaxDistance { get { return maxDistance; } }

    [SerializeField] private float simulationSpeed = 100f;
    public float SimulationSpeed { get { return simulationSpeed; } }

    #endregion
}
