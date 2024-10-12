using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float timeBeforeDestroy = 5f;
    public float speed = 10f;
    public float damage = 1f;
    public float castingTime;
    public float cooldown;
    public float energyCost;
    public LayerMask piso;

    private void Start()
    {
        Destroy(gameObject, timeBeforeDestroy);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == piso)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.ReceiveDamage(damage);
            Destroy(gameObject);
        }
    }



}
