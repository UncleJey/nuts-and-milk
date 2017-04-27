using UnityEngine.UI;
using UnityEngine;

public class BtnRight : Button
{
	void Update () 
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (IsPressed ())
			Player.rightPressed = true;
		else
			Player.rightPressed = false;
	}
}