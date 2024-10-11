using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePillarRail : MonoBehaviour
{
    public List<FirePillar> firePillars = new();
    public float afertFinishDelay;

    public void StartDeactivate()
    {
        for (int i = 0; i < firePillars.Count; i++)
        {
            firePillars[i].doingDamage = false;
        }
        Invoke(nameof(DeactivatePillars), afertFinishDelay);
    }

    private void DeactivatePillars()
    {
        gameObject.SetActive(false);
    }
}
