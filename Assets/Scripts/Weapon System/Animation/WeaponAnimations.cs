using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    private Animation anim;

    protected virtual void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void PlayShootAnimation(float delay)
    {
        StartCoroutine(PlayShootAnimationCoroutine(delay));
    }

    private IEnumerator PlayShootAnimationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        anim.Play("Shoot");
    }

    public void PlayReloadAnimation(float delay)
    {
        StartCoroutine(PlayReloadAnimationCoroutine(delay));
    }

    private IEnumerator PlayReloadAnimationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        anim.Play("Reload");
    }
}
