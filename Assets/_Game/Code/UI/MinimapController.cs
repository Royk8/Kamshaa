using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinimapController : MonoBehaviour
{
    public InputAdapter inputAdapter;
    public Animator map;

    public void Start()
    {
        inputAdapter.OnMap += ToggleMinimap;
    }

    private void ToggleMinimap(InputAction.CallbackContext context)
    {
        map.SetBool("Mapa", !map.GetBool("Mapa"));
    }
}
