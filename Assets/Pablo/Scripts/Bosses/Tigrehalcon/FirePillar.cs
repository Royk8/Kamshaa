using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirePillar : MonoBehaviour
{
    public MeshRenderer material;
    public Material warning, danger;
    public Transform initPoint, finalPoint;
    public LayerMask mask;
    public float damageFrecuence;
    public float damage;
    public float warningTime;

    private Coroutine damageTickCoroutine;

    private void OnEnable()
    {
        damageTickCoroutine = StartCoroutine(DamageTick());
    }

    private IEnumerator DamageTick()
    {
        material.material = warning;
        yield return new WaitForSeconds(warningTime);
        material.material = danger;

        while (true)
        {
            Collider[] collidersAffected = Physics.OverlapCapsule(initPoint.position, finalPoint.position, transform.localScale.x / 2, mask);
            Debug.Log(collidersAffected.Length);

            for (int i = 0; i < collidersAffected.Length; i++)
            {
                if (collidersAffected[i].TryGetComponent(out IDamageable creature))
                {
                    creature.ReceiveDamage(damage);
                    Debug.Log(collidersAffected[i].name + " dañado por fire pillar de: " + transform.name);
                }
            }
            yield return new WaitForSeconds(damageFrecuence);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(damageTickCoroutine);
    }
}
