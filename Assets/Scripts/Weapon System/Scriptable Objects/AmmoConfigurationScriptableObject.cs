using UnityEngine;

[CreateAssetMenu(menuName = "Guns/Ammo Configuration", fileName = "AmmoConfiguration", order = 6)]
public class AmmoConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private int clipSize = 30;
    public int ClipSize { get { return clipSize; } private set { ClipSize = value; } }

    [SerializeField] private float shootAnimationDelay = 0f;
    public float ShootAnimationDelay { get { return shootAnimationDelay; } }
}
