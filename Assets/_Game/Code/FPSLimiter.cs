using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int TargetFPS = 60;
    void Awake()
    {
        Application.targetFrameRate = TargetFPS;
    }


}
