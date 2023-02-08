using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Configuration", order = 2)]
public class ShootConfigurationScriptableObject : ScriptableObject
{
    public LayerMask HitMask;
    public int PelletsPerBullet = 1;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float FireRate = .25f;
}
