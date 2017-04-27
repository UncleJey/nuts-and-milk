using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonMode : MonoBehaviour 
{
	public bool editMode;
	public UILevels levels;
	Button button;
	public AudioClip clip;

	void Start () 
	{
		button = GetComponent<Button> ();
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener (btnClick);

		if (editMode && Save.currentLevel < 1) 
		{
			button.interactable = false;
			GetComponentInChildren<Text> ().color = Color.gray;
		}
	}
	
	void btnClick () 
	{
		UILevels.editMode = editMode;
		levels.gameObject.SetActive (true);
		if (clip != null)
			UILoaderSoundPlayer.Source.PlayOneShot (clip);
	}
}
