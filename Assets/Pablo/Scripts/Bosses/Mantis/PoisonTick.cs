using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTick : MonoBehaviour
{
    public float damageTick = 0.1f;
    public float damageFrecuency = 0.3f;
    public float poisonWarning = 1.5f;
    public float poisonDuration = 3;
    public Material warning, danger;
    public MeshRenderer meshRenderer;
    public LayerMask mask;
    public GameObject creator;
    public ParticleSystem particles;

    private Coroutine dotCoroutine;

    private void OnEnable()
    {
        particles.Play();
        dotCoroutine = StartCoroutine(DamageOverTime());
        meshRenderer.material = warning;
    }

    private void OnDisable()
    {
        particles.Stop();
        StopCoroutine(dotCoroutine);
    }

    private IEnumerator DamageOverTime()
    {
        yield return new WaitForSeconds(poisonWarning);
        meshRenderer.material = danger;

        while (true)
        {
            Collider[] collidersAffected = Physics.OverlapSphere(transform.position, transform.localScale.x / 2, mask);
            Debug.Log(collidersAffected.Length);

            for (int i = 0; i < collidersAffected.Length; i++)
            {
                if (collidersAffected[i] == creator.GetComponent<Collider>())
                {
                    creator.GetComponent<Mantis>().Heal(damageTick);
                    continue;
                }
                Debug.Log(collidersAffected[i].name);
                if (collidersAffected[i].TryGetComponent(out IDamageable creature))
                {
                    creature.ReceiveDamage(damageTick);
                    Debug.Log(collidersAffected[i].name + " dañado por poison de: " + creator.name);
                }
            }
            yield return new WaitForSeconds(damageFrecuency);
        }
    }
}
