using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TheCursor : MonoBehaviour 
{
	Image img;
	public Color baseColor;

	void Awake () 
	{
		img = GetComponent<Image> ();
	}

	float val = 1f;
	float step = -1f;
	void Update () 
	{
		val += step * Time.deltaTime;
		if (val < 0.3f)
			step = 1;
		else if (val > 1f)
			step = -1;

		img.color = new Color (baseColor.r, baseColor.g, baseColor.b, val);
	}
}
