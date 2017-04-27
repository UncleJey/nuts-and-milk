using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonClickSound : MonoBehaviour 
{
	Button button;
	public AudioClip clip;

	void OnEnable () 
	{
		if (button == null)
			button = GetComponent<Button> ();
		button.onClick.AddListener (click);
	}
	
	void click()
	{
		UILoaderSoundPlayer.Source.PlayOneShot (clip);
	}

	void OnDisable () 
	{
		button.onClick.RemoveListener (click);
	}
}
