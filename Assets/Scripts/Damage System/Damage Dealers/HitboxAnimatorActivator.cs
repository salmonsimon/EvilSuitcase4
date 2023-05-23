using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAnimatorActivator : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> hitBoxes;

    public void EnableHitbox(int hitboxIndex)
    {
        if (hitBoxes.Count > hitboxIndex)
            hitBoxes[hitboxIndex].enabled = true;
    }

    public void DisableHitbox(int hitboxIndex)
    {
        if (hitBoxes.Count > hitboxIndex && hitBoxes[hitboxIndex].enabled)
            hitBoxes[hitboxIndex].enabled = false;
    }
}
