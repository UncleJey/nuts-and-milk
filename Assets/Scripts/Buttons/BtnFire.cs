using UnityEngine.UI;
using UnityEngine;

public class BtnFire : Button
{
	void Update () 
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (IsPressed ())
			Player.firePressed = true;
		else
			Player.firePressed = false;
	}
}
