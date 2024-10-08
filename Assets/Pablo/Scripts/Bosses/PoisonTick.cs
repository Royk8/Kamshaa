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

    private Coroutine dotCoroutine;

    private void OnEnable()
    {
        dotCoroutine = StartCoroutine(DamageOverTime());
        meshRenderer.material = warning;
    }

    private void OnDisable()
    {
        StopCoroutine(dotCoroutine);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator DamageOverTime()
    {
        yield return new WaitForSeconds(poisonWarning);
        meshRenderer.material = danger;
        //Invoke(nameof(Disable), poisonDuration);

        while (true)
        {
            Collider[] collidersAffected = Physics.OverlapSphere(transform.position, transform.localScale.x / 2, mask);
            Debug.Log(collidersAffected.Length);

            for (int i = 0; i < collidersAffected.Length; i++)
            {
                if (collidersAffected[i] != creator) continue;
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
