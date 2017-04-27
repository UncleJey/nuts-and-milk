using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : AnimatedBase 
{
	Step cell;

	void Update ()
	{
		if (!Active)
			return;
		
		if ((rt.anchoredPosition - TheGame.thePlayer.rt.anchoredPosition).magnitude < 55) 
		{
			Debug.Log ((rt.anchoredPosition - TheGame.thePlayer.rt.anchoredPosition).magnitude);
			TheGame.Sink (TheGame.thePlayer, false);
		}

		float dist = cell == null ? 555 : (rt.anchoredPosition - cell.cell.rt.anchoredPosition).magnitude;
		if (dist < 5 || dist > 90) 
			CalcMove ();
		else if (base.speed.Empty)// && !base.flying)
			CalcMove ();

		AfterUpdate ();
		if (flying && offset.Y == 0) // костыль
			offset.Y = -1;
	}

	void Awake()
	{
		base.iAmEnemy = true;
		base.Awake ();
	}

	AnimState curMoveState = AnimState.Stay;
	/// <summary>
	/// Рассчитать ход
	/// </summary>
	void CalcMove()
	{
		cell = TheField.FindNextCell (position, TheGame.thePlayer.position, (int)curMoveState, (int)AnimState.WalkR);
		if (cell != null) 
			base.Keys = cell.stepType;
	}

	void LateUpdate()
	{
		if (!Active)
			return;
		
		switch (TheField.getType(position))
		{
			case EntityType.Water:
				TheGame.Sink(this, true);
			break;
			default:
			break;
		}
	}

	public override void Spawn (bool pEmmediate)
	{
		if (pEmmediate)
			base.Spawn (pEmmediate);
		else
			StartCoroutine (Spawner ());
	}

	IEnumerator Spawner()
	{
		Teleport (startPos);
		ChangeState (AnimState.Born);
		yield return new WaitForSeconds (0.5f);
		if (TheGame.thePlayer.Active)
			Active = true;
	}
}
