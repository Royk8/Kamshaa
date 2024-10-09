using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttack : MonoBehaviour
{
    Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }
    public void Attack()
    {
        collider.enabled = true;
    }

    internal void StopAttack()
    {
        collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Print enemy hit when collide with them

        IDamageable enemy = other.GetComponent<IDamageable>();
        if (enemy != null)
        {
            Debug.Log("Enemy Hit");
            enemy.ReceiveDamage(1);
        }

    }
}
