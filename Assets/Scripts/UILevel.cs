using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour 
{
	int myNum = 0;

	public Digit digitL,digitC,digitR;
	public Image imgButotn;
	public Button button;
	public Image[] hearts;

	public void Set(int pNum, StateSet pState)
	{
		myNum = pNum;

		imgButotn.sprite = pState.sprite;

		string n = pNum.ToString ();

		digitL.gameObject.SetActive (pNum > 9);
		digitR.gameObject.SetActive (pNum > 9);
		digitC.gameObject.SetActive (pNum < 10);

		if (pNum < 10)
			digitC.digit = pNum;
		else 
		{
			digitL.Set (n [0].ToString());
			digitR.Set (n [1].ToString());
		}

		for (int i = 0; i < hearts.Length; i++)
			hearts [i].gameObject.SetActive (Save.levels [pNum] > i);

	}

	void Awake()
	{
		button.onClick.AddListener (clicked);
	}

	void clicked()
	{
		UILevels.PlayStage (myNum);
	}

}
