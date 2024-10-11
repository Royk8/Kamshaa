using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodGlobalParameter : MonoBehaviour
{
    public string nombreParametroGlobal = "Batalla";

    // Update is called once per frame
    public void EnviarParametro(float valor)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(nombreParametroGlobal, valor);
    }
}
