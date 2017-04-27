using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GirlMode : byte
{
	 Passive	= 0
	,Ready		= 1
	,Done		= 2
}

public class Girl : MonoBehaviour 
{
	public Point startPos;

	Animator animator;
	RectTransform rt;
	public GirlMode mode;

	protected virtual void Awake()
	{
		animator = GetComponent<Animator> ();
		rt = GetComponent<RectTransform> ();
		mode = GirlMode.Passive;
	}

	public void Spawn()
	{
		gameObject.SetActive (true);
		UpdatePos ();
		if (TheField.isBonusLevel)
		{
			mode = GirlMode.Ready;
			animator.Play ("Ready2");
		}
	}

	protected void UpdatePos()
	{
		rt.anchoredPosition = (startPos * TheField.CellSize).ToVector2();
		rt.SetAsLastSibling ();
	}

	public void ChangeState(GirlMode pMode)
	{
		mode = pMode;
		Debug.Log ("new mode " + pMode.ToString ());
		animator.Play (pMode.ToString ());
	}

	void Update()
	{
		if (mode == GirlMode.Ready && (rt.anchoredPosition - TheGame.thePlayer.rt.anchoredPosition).magnitude < 55)
		{
			ChangeState (GirlMode.Done);
			TheGame.DoneLevel ();
			//Debug.Log ((rt.anchoredPosition - Player.instance.rt.anchoredPosition).magnitude);
		}
	}
}
