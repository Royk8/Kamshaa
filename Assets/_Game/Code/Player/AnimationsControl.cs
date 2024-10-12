using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimationsControl : MonoBehaviour
{
    public Animator animator;
    [SerializeField]
    private bool enPiso;
    public bool autoCalcularVelocidad;
    public VFXControlPersonaje vfx;
    public UnityEvent dobleSalto;
    public UnityEvent dash;
    public UnityEvent stun;
    private FMOD.Studio.EventInstance Stun;

    Vector3 bPos;

	private void Start()
	{
        bPos = transform.position;
		if (autoCalcularVelocidad)
		{
            StartCoroutine(RecalcularMovimiento());
		}
	}

    IEnumerator RecalcularMovimiento()
	{
        Vector3 aPos;
		while (autoCalcularVelocidad)
		{
            yield return new WaitForSeconds(0.1f);
            aPos = transform.position;
            aPos.y = 0;
            CambiarVelocidad((aPos - bPos).sqrMagnitude*3);
            bPos = transform.position;
            bPos.y = 0;
		}
	}

	public void Dash()
	{
        animator.SetTrigger("dash");
        dash.Invoke();
        if (vfx!=null)
		{
            vfx.Dash();
		}
    }
    public void DobleSalto()
    {
        dobleSalto.Invoke();
        animator.SetTrigger("resalto");
        if (vfx != null)
        {
            vfx.DobleSalto();
        }
    }
    public void Disparar()
    {
        animator.SetTrigger("poder");
    }
    [ContextMenu("Marear")]
    public void IniciarMareo()
    {
        stun.Invoke();
        animator.SetBool("mareado", true);
        vfx.SetMareado(true);
        Stun = AudioManager.Instance.NuevaInstancia(EventsManager.Instance.StunSnapshot);
        Stun.start();
    }
    [ContextMenu("Desmarear")]
    public void FinalizarMareo()
    {
        
        animator.SetBool("mareado", false);
        vfx.SetMareado(false);

        Stun.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        
        
    }
    public void CambiarVelocidad(float v)
	{
        animator.SetFloat("velocidad", v);
    }
    public void CambiarEnPiso(bool _enPiso)
    {
        enPiso = _enPiso;
        animator.SetBool("piso", enPiso);
    }
}
