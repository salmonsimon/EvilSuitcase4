using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Weapons/CrossHair Configuration", fileName = "CrossHairConfiguration", order = 0)]
public class CrosshairConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get { return weaponType; } }

    [SerializeField] private bool hasDotImage = true;
    [SerializeField] private bool hasInnerImage = true;

    [SerializeField] private bool hasExpandingImage = true;
    public bool HasExpandingImage { get { return hasExpandingImage; } }

    [SerializeField, DrawIf("hasDotImage", true)] private Sprite dot;
    public Sprite Dot { get { return dot; } }

    [DrawIf("weaponType", WeaponType.Gun), DrawIf("hasInnerImage", true), SerializeField] private Sprite inner;
    public Sprite Inner { get { return inner; } }

    [DrawIf("weaponType", WeaponType.Gun), DrawIf("hasExpandingImage", true), SerializeField] private Sprite expanding;
    public Sprite Expanding { get { return expanding; } }

    [DrawIf("weaponType", WeaponType.Gun), DrawIf("hasExpandingImage", true), SerializeField] private float crosshairMaxScale = 2f;
    public float CrosshairMaxScale { get { return crosshairMaxScale; } }

    [DrawIf("weaponType", WeaponType.Gun), DrawIf("hasExpandingImage", true), SerializeField] private float expandingValue = .5f;
    public float ExpandingValue { get { return expandingValue; } }

    [DrawIf("weaponType", WeaponType.Gun), DrawIf("hasExpandingImage", true), SerializeField] private float shrinkingSpeed = .5f;
    public float ShrinkingSpeed { get { return shrinkingSpeed; } }

    [DrawIf("weaponType", WeaponType.Gun), SerializeField] private AnimationClip reloadAnimationClip;
    public AnimationClip ReloadAnimationClip { get { return reloadAnimationClip; } }
}
