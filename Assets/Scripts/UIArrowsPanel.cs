using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArrowsPanel : MonoBehaviour 
{
	void Awake () 
	{
		if (!UILevels.editMode)
			gameObject.SetActive (false);
	}
}
