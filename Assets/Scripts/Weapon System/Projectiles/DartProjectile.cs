using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartProjectile : Projectile
{
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        transform.SetParent(collision.collider.transform);
    }
}
