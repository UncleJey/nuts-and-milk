using UnityEngine.UI;
using UnityEngine;

public class BtnFire2 : Button
{
	void Update () 
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (IsPressed ())
			Player.fire2Pressed = true;
		else
			Player.fire2Pressed = false;
	}
}
