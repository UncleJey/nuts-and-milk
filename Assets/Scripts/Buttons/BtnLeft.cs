using UnityEngine.UI;
using UnityEngine;

public class BtnLeft : Button
{
	void Update () 
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (IsPressed ())
			Player.leftPressed = true;
		else
			Player.leftPressed = false;
	}
}
