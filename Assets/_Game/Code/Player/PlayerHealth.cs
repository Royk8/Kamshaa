using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;
    public event Action<float> OnHealthChanged;

    /**
     * 
     * To be called by enemies or other objects that can damage the player
     */
    public void ReceiveDamage(float value)
    {
        currentHealth -= value;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //Just like me working in this game.
    public void SpendHealth(float value)
    {
        currentHealth -= value;
    }

    private void Die()
    {
        Debug.Log("Player died");
    }
}
