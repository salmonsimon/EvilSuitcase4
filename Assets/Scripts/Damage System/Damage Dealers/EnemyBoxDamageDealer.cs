using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBoxDamageDealer : MonoBehaviour
{
    [SerializeField] DamageDealerConfigScriptableObject damageDealerConfig;

    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(GetDamage(), Vector3.zero);

            boxCollider.enabled = false;
        }
    }

    private int GetDamage()
    {
        return Random.Range(damageDealerConfig.MinMaxDamage.x, damageDealerConfig.MinMaxDamage.y);
    }
}
