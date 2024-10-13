using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderLento : MonoBehaviour
{
	public Slider slImitar;
	public Slider miSlider;



    IEnumerator Start()
    {
		while (true)
		{
			if (slImitar.maxValue != miSlider.maxValue)
			{
				miSlider.maxValue = slImitar.maxValue;
			}
			if (slImitar.value != miSlider.value)
			{
				while (Mathf.Abs(slImitar.value-miSlider.value)>0.01f)
				{
					miSlider.value = Mathf.Lerp(miSlider.value, slImitar.value, 0.1f);
					yield return new WaitForSeconds(0.01f);
				}
				slImitar.value = miSlider.value;
			}
			yield return new WaitForSeconds(0.5f);
		}
    }
}
