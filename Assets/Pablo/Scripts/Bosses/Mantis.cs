using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Mantis : MonoBehaviour
{
    private bool actualMechanicIsFinished = false;
    public Transform player;
    [Space(2)]
    [Header("Slash")]
    [Space(1)]
    public List<Transform> slashUbications = new();
    private List<Transform> slashUbicationsRemaining = new();
    public List<GameObject> hitBoxIndicators;
    public Material warningMaterial, hitMaterial;
    public LayerMask hitBoxMask;
    public string actualMechanic;
    public float slashDamage, slashWait, slashWarningDelay, slashFinishDelay;
    [Space(1)]
    [Header("Poison")]
    [Space(2)]
    public NavMeshAgent agent;
    public List<Transform> posiblePositions = new();
    private List<Transform> posiblePositionsRemaining = new();
    public PoisonTick poisonTick;

    private void Start()
    {
        StartCoroutine(MechanicsController());
    }

    private IEnumerator MechanicsController()
    {
        while (true)
        {
            actualMechanicIsFinished = false;
            StartCoroutine(Slash());
            actualMechanic = nameof(Slash);
            yield return new WaitUntil(() => actualMechanicIsFinished);
            actualMechanicIsFinished = false;
            StartCoroutine(Poison());
            actualMechanic = nameof(Poison);
            yield return new WaitUntil(() => actualMechanicIsFinished);
        }
    }

    private IEnumerator Slash()
    {
        if (slashUbicationsRemaining.Count == 0)
            slashUbicationsRemaining = slashUbications.Where(x => x).ToList();

        // se posiciona la mantis en el lugar de donde va a atacar
        Transform selectedPosition = slashUbicationsRemaining[Random.Range(0, slashUbicationsRemaining.Count)];
        slashUbicationsRemaining.Remove(selectedPosition);
        transform.SetPositionAndRotation(selectedPosition.position, selectedPosition.rotation);
        yield return new WaitForSeconds(slashWait);

        // mostramos el area donde va a golpear
        int randomArea = Random.Range(0, hitBoxIndicators.Count);
        hitBoxIndicators[randomArea].GetComponent<MeshRenderer>().material = warningMaterial;
        hitBoxIndicators[randomArea].SetActive(true);
        yield return new WaitForSeconds(slashWarningDelay);

        // crea las hitbox donde va a golpear
        hitBoxIndicators[randomArea].GetComponent<MeshRenderer>().material = hitMaterial;
        Collider[] collidersAffected = Physics.OverlapBox(hitBoxIndicators[randomArea].transform.position, hitBoxIndicators[randomArea].transform.localScale / 2, hitBoxIndicators[randomArea].transform.rotation, hitBoxMask);
        for (int i = 0; i < collidersAffected.Length; i++)
        {
            if (collidersAffected[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.ReceiveDamage(slashDamage);
                Debug.Log(collidersAffected[i].name + " dañado por slash de: " + transform.name);
            }
            else
                Debug.Log(collidersAffected[i].name);

        }
        yield return new WaitForSeconds(slashFinishDelay);

        hitBoxIndicators[randomArea].SetActive(false);
        actualMechanicIsFinished = true;
    }

    private IEnumerator Poison()
    {
        if (posiblePositionsRemaining.Count == 0)
            posiblePositionsRemaining = posiblePositions.Where(x => x).ToList();

        // posiciona a la mantis en una de las posibles posiciones de spawn para ir a atacar al jugador
        Transform spawnPosition = posiblePositionsRemaining[Random.Range(0, posiblePositionsRemaining.Count)];
        posiblePositionsRemaining.Remove(spawnPosition);
        transform.SetLocalPositionAndRotation(spawnPosition.position, spawnPosition.rotation);

        // hace que la mantis persiga al jugador
        agent.enabled = true;
        agent.isStopped = false;
        Coroutine destinationCo = StartCoroutine(SetMantisDestination(player.transform));

        // ubica y activa la nube de veneno
        poisonTick.transform.position = new Vector3 (player.transform.position.x, poisonTick.transform.position.y, player.transform.position.z);
        poisonTick.gameObject.SetActive(true);
        yield return new WaitForSeconds(poisonTick.poisonWarning + poisonTick.poisonDuration);
        poisonTick.gameObject.SetActive(false);
        StopCoroutine(destinationCo);
        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;
        actualMechanicIsFinished = true;
    }

    private IEnumerator SetMantisDestination(Transform destination)
    {
        while (!agent.isStopped)
        {
            agent.SetDestination(destination.position);
            yield return new WaitForSeconds(0.3f);
        }
        Debug.Log("sale");
    }
}