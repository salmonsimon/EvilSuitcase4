using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolableObject : MonoBehaviour
{
    public ObjectPool<GameObject> ObjectPool { get; set; }
}
