using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Fuego : MonoBehaviour
{
    ParticleSystem particulas;
    public Transform target;
    public float velocidad= 5;
    float velTiempo = 3f;
    public GameObject tomado;
    public float rango;

    Vector3 posInicial;
    // Start is called before the first frame update
    void Start()
    {
        particulas = GetComponent<ParticleSystem>();
        StartCoroutine(Buscar());
        posInicial = transform.position;
    }

    public IEnumerator Buscar()
	{
		while (target == null)
		{
            yield return new WaitForSeconds(0.3f);
            Collider[] cols = Physics.OverlapSphere(transform.position, rango);
			for (int i = 0; i < cols.Length; i++)
			{
				if (cols[i].CompareTag("Player"))
				{
                    target = cols[i].transform;
                    break;
                }
			}

        }
	}

    // Update is called once per frame
    void Update()
    {
		if (target != null)
		{
            Quaternion brot = transform.rotation;
            transform.LookAt(target.position + Vector3.up);
            transform.rotation = Quaternion.Lerp(brot, transform.rotation, Time.deltaTime * velTiempo);
            velTiempo += Time.deltaTime;
            velocidad += Time.deltaTime/1.5f;
            transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
			if ((transform.position - (target.position + Vector3.up)).sqrMagnitude < 0.2f)
			{
                particulas.enableEmission = false;
                GameObject gt = Instantiate(tomado, transform.position, Quaternion.identity);
                gt.transform.localScale  = Vector3.one*2;
                gt = Instantiate(tomado, transform.position, Quaternion.identity);
                gt.transform.localScale = Vector3.one * 2;
                gt = Instantiate(tomado, transform.position, Quaternion.identity);
                gt.transform.localScale = Vector3.one * 2;
                this.enabled = false;
                Destroy(gameObject, 3);
			}
		}
		else
		{
            transform.position = posInicial + Vector3.up * Mathf.Sin(Time.time);
		}
    }

	private void OnDrawGizmosSelected()
	{
        Gizmos.DrawWireSphere(transform.position, rango);
	}
}
