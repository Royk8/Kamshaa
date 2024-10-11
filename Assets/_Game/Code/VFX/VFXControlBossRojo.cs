using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXControlBossRojo : MonoBehaviour
{
    public GameObject particulasRojas;
    public void VerParticulas( bool visible)
	{
		particulasRojas.SetActive(visible);
	}
}
