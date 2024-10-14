using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sting : MonoBehaviour
{
    public float velocity = 5;
    public Rigidbody rb;
    public float damage = 1;
    public float disableDelay;
    public GameObject creator;
    public TrailRenderer trail;
    bool yaDan = false;

    private void Start()
    {
        SetUpVelocity();
    }

    private void OnEnable()
    {
        trail.Clear();
        SetUpVelocity();
    }

    private void SetUpVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.velocity = transform.forward * velocity;
        Invoke(nameof(DisableObject), disableDelay);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == creator) return;
        if (!yaDan && other.gameObject.TryGetComponent(out IDamageable entity))
        {
            entity.ReceiveDamage(damage);
        }
        Invoke("Desactivar", 1);
        yaDan = true;
    }

    void Desactivar()
	{
        gameObject.SetActive(false);
        yaDan = false;
    }
}
