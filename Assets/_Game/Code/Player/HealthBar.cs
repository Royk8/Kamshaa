using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public RawImage indicator;
    public float maxHealth = 100f;
    [Range(0,100)]
    public float currentHealth;
    PlayerHealth playerHealth;

    private void Start()
    {
        indicator = GetComponent<RawImage>();
    }

    public void SetHealth(float value)
    {
        indicator.rectTransform.localScale = new Vector3(value/maxHealth, 1, 1);
    }

    public void ModifyHealth(float value)
    {
        indicator.rectTransform.localScale += new Vector3(value/maxHealth, 0, 0);
    }

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
            currentHealth = playerHealth.currentHealth;
        }
        playerHealth.OnHealthChanged += ModifyHealth;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= ModifyHealth;
    }
}
