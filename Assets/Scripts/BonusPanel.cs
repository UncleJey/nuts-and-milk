using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPanel : MonoBehaviour 
{
	public Numbr bonus;

	void Start()
	{
		GetComponent<RectTransform> ().SetAsLastSibling ();
	}

	void Update ()
	{
		bonus.Value = (int) (Save.bonus * 10);
	}
}
