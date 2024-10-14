using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player received damage. Current health: " + currentHealth);
		if (ControladorCamara.singleton != null)
		{
            ControladorCamara.singleton.IniciarTemblor(1, 0.2f);
		}
		if (MarcoSangre.singleton != null)
		{
            MarcoSangre.singleton.IniciarEfecto(1);
		}
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //Just like me working in this game.
    public void SpendHealth(float value)
    {
        currentHealth -= value;
        currentHealth = Mathf.Clamp(currentHealth, 1, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);

    }

    public void Heal (float value)
    {
        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void Die()
    {
        currentHealth = 0;
        SceneManager.LoadScene("GameOver");
    }
}
