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
    public float radio = 3;
    public float damageFrecuence;
    public float damage;
    public float warningTime;
    public bool doingDamage;

    private Coroutine damageTickCoroutine;

    private void OnEnable()
    {
        damageTickCoroutine = StartCoroutine(DamageTick());
    }

    private IEnumerator DamageTick()
    {
        material.material = warning;
        yield return new WaitForSeconds(warningTime);
        ControladorCamara.singleton.IniciarTemblor(5, 0.06f);
        material.material = danger;
        doingDamage = true;

        while (doingDamage)
        {
            Collider[] collidersAffected = Physics.OverlapCapsule(initPoint.position, finalPoint.position, radio, mask);
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
