using UnityEngine;

public class ReduceAnimationFramerate : MonoBehaviour
{
    public float x, y, z, w;
    void Start()
    {
		x = transform.localRotation.x;
		y = transform.localRotation.y;
		z = transform.localRotation.z;
		w = transform.localRotation.w;
	}

    void Update()
    {
        Quaternion q = new Quaternion(x, y, z, w);
        transform.localRotation = q;
    }
}
