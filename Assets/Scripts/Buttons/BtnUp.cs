using UnityEngine.UI;
using UnityEngine;

public class BtnUp : Button
{

	void Update () 
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (IsPressed ())
			Player.upPressed = true;
		else
			Player.upPressed = false;
	}
}
