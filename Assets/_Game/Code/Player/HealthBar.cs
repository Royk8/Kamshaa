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
    float sizeDeltaX;

    public void SetHealth(float value)
    {
        indicator.rectTransform.sizeDelta = new Vector3((value/maxHealth) * sizeDeltaX, indicator.rectTransform.sizeDelta.y);
    }

    private void OnEnable()
    {
        sizeDeltaX = indicator.rectTransform.sizeDelta.x;
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
            currentHealth = playerHealth.currentHealth;
            SetHealth(currentHealth);
        }
        playerHealth.OnHealthChanged += SetHealth;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= SetHealth;
    }
}
