using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperFruit : MonoBehaviour 
{
	public int speed = 20;
	int direction = 1;

	public int minY = 200;
	public int maxY = 400;

	RectTransform rt;

	void Start()
	{
		rt = GetComponent<RectTransform> ();
		rt.SetAsLastSibling ();
	}

	void Update()
	{
		if (!TheGame.thePlayer.Active)
			return;
		
		float spd = Time.deltaTime * speed;
		if (direction > 0)
		{
			if (rt.anchoredPosition.y + spd < maxY)
				rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + spd);
			else
				direction = -1;
		}
		else
		{
			if (rt.anchoredPosition.y - spd > minY)
				rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - spd);
			else
				direction = 1;
		}

		if ((rt.anchoredPosition - TheGame.thePlayer.rt.anchoredPosition).magnitude < 55) 
		{
			TheGame.SuperEat (rt.anchoredPosition);
			Destroy (gameObject);
		}

	}

}
