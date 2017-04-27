using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoaderSoundPlayer : MonoBehaviour 
{
	static UILoaderSoundPlayer instance;
	AudioSource source;

	void Awake()
	{
		instance = this;
		source = GetComponent<AudioSource> ();
	}

	public static AudioSource Source
	{
		get 
		{
			return instance.source;
		}
	}
}
