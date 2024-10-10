using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public Collider creator;
    public float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other == creator) return;
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.ReceiveDamage(damage);
        }
    }
}
