using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MuscleComponent
{
    public Transform Transform;
    public Rigidbody Rigidbody;
    public Collider Collider;

    public MuscleComponent(Transform transform)
    {
        Transform = transform;
        Rigidbody = transform.GetComponent<Rigidbody>();
        Collider = transform.GetComponent<Collider>();
    }
}
