using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXControlMantis : MonoBehaviour
{
    public Animator animator;
    public TrailRenderer tRendMano1, tRendMano2;

    [ContextMenu("IniciarAtaqueLargo")]
    public void IniciarAtaqueLargo()
    {
        animator.SetBool("dashing", true);
        tRendMano1.emitting = true;
        tRendMano2.emitting = true;
    }

    [ContextMenu("TerminarAtaqueLargo")]
    public void TerminarAtaqueLargo()
    {
        animator.SetBool("dashing", false);
        tRendMano1.emitting = false;
        tRendMano2.emitting = false;
    }

    [ContextMenu("Golpear")]
    public void Golpear()
    {
        animator.SetTrigger("atacar");
    }

    private void SetVisible(bool visible)
    {
        animator.SetBool("visible", visible);
    }

    [ContextMenu("Visibilizar")]
    public void Visibilizar()
    {
        SetVisible(true);
    }

    [ContextMenu("Invisibilizar")]
    public void Invisibilizar()
    {
        SetVisible(false);
    }
}
