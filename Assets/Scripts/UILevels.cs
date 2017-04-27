using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LevelState : byte
{
	 Closed = 0
	,Current = 1
	,Opened = 2
}

[System.Serializable]
public class StateSet
{
	public Sprite sprite;
	public LevelState state;
}

public class UILevels : MonoBehaviour 
{
	public StateSet[] states;
	int curPage = 0;
	/// <summary>
	/// The pool.
	/// </summary>
	public GroupLayoutPool pool;
	/// <summary>
	/// Items per page
	/// </summary>
	public int IPP = 8;
	public Button btnLeft;
	public Button btnRight;

	/// <summary>
	/// Режим
	/// </summary>
	public Text caption;
	static UILevels instance;
	public static bool editMode = false;

	public AudioClip clip;

	void Awake()
	{
		instance = this;
		Save.RefreshLevels ();
		curPage = (int)(Save.currentLevel / IPP);

		btnLeft.onClick.RemoveAllListeners ();
		btnRight.onClick.RemoveAllListeners ();

		btnLeft.onClick.AddListener (btnLeftClick);
		btnRight.onClick.AddListener (btnRightClick);

		Save.firftPlay = true;
	}

	void btnLeftClick()
	{
		if (curPage > 0) 
		{
			curPage--;
			Show ();
		} 
		else
			gameObject.SetActive (false);
		if (clip != null)
			UILoaderSoundPlayer.Source.PlayOneShot (clip);
	}

	void btnRightClick()
	{
		if (curPage * IPP + IPP< Save.maxLevel)
			curPage++;
		Show ();
		if (clip != null)
			UILoaderSoundPlayer.Source.PlayOneShot (clip);
	}

	void OnEnable()
	{
		Show ();
		if (editMode)
			caption.text = "E d i t   L e v e l";
		else
			caption.text = "S e l e c t   L e v e l";
	}

	int curLev
	{
		get
		{
			if (Save.currentLevel < 1)
				return 1;
			else
				return Save.currentLevel;
		}
	}

	/// <summary>
	/// Проиграть уровень
	/// </summary>
	public static void PlayStage(int pLev)
	{
#if !UNITY_EDITOR
		if (instance.curLev >= pLev)
#endif
			instance.doPlay (pLev);
	}

	/// <summary>
	/// Запустить игру
	/// </summary>
	void doPlay(int pLev)
	{
		if (clip != null)
			UILoaderSoundPlayer.Source.PlayOneShot (clip);
		Debug.Log ("play level: " + pLev);
		Save.playingLevel = pLev;
		Save.bonus = 3 * 60; // 3 минуты
		StartCoroutine (lateRun());
	}

	IEnumerator lateRun()
	{
		yield return new WaitForSeconds(0.5f);
		Save.lives = 2;
		UnityEngine.SceneManagement.SceneManager.LoadScene ("main");
	}

	StateSet getState(int pLev)
	{
		if (pLev < curLev)
			return states[0];
		if (pLev == curLev)
			return states [1];

		return states [2];
	}

	void Show()
	{
		btnRight.interactable = true;
		pool.Clear ();
		for (int i = 0; i < IPP; i++)
		{
			int lNo = i + curPage * IPP + 1;
			if (lNo <= Save.maxLevel) 
			{
				UILevel lev = pool.InstantiateElement ().GetComponent<UILevel> ();
				lev.Set (lNo, getState (lNo));
			} 
			else
				btnRight.interactable = false;
		}
	}
}

