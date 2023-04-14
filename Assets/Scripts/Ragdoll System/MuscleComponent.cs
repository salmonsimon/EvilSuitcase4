using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MuscleComponent : MonoBehaviour
{
    public Transform Transform;
    public Rigidbody Rigidbody;
    public Collider Collider;
    public HumanoidBodyPart BodyPart;
    public ConfigurableJoint ConfigurableJoint;

    public Vector3 StoredLocalPosition;
    public Quaternion StoredLocalRotation;

    public void StoreLocalPositionAndRotation()
    {
        StoredLocalPosition = transform.localPosition;
        StoredLocalRotation = transform.localRotation;
    }
}
