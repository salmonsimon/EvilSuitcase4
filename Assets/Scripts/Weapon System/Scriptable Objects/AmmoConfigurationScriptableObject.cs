using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Guns/Ammo Configuration", fileName = "AmmoConfiguration", order = 6)]
public class AmmoConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private Sprite bulletSprite;
    public Sprite BulletSprite { get { return bulletSprite; } }

    [SerializeField] private int clipSize = 30;
    public int ClipSize { get { return clipSize; } private set { ClipSize = value; } }

    [SerializeField] private float shootAnimationDelay = 0f;
    public float ShootAnimationDelay { get { return shootAnimationDelay; } }
}
