using UnityEngine.UI;
using UnityEngine;

public class BtnDown : Button
{

	void Update () 
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (IsPressed ())
			Player.downPressed = true;
		else
			Player.downPressed = false;
	}
}
