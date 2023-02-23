using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    [SerializeField] private float gravityMultiplier = 1.0f;

    private float globalGravity = Physics.gravity.y;

    Rigidbody rigidBody;

    void OnEnable()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
    }

    void FixedUpdate()
    {
        Vector3 gravity = globalGravity * gravityMultiplier * Vector3.up;
        rigidBody.AddForce(gravity, ForceMode.Acceleration);
    }
}