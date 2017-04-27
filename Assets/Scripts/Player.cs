using UnityEngine;
using System.Collections;

public class Player : AnimatedBase 
{
	protected override void Awake()
	{
		base.PlaySound = true;
		base.iAmEnemy = false;
		base.Awake();
	}

	static KeyCode[] Joystics = new KeyCode[]
	{
		 KeyCode.JoystickButton0        // треугольник
		,KeyCode.JoystickButton1        // круг
		,KeyCode.JoystickButton2        // квадрат
		,KeyCode.JoystickButton3        // крестик
		,KeyCode.JoystickButton4        // LD
		,KeyCode.JoystickButton5        // RD
		,KeyCode.JoystickButton6        // LU
		,KeyCode.JoystickButton7        // RU
		,KeyCode.JoystickButton8        // select
		,KeyCode.JoystickButton9        // start
		,KeyCode.JoystickButton10       // Left pad Click
		,KeyCode.JoystickButton11       // Right pad Click
		,KeyCode.Space                  // Пробел
	};

	public static bool rightPressed = false;
	public static bool leftPressed = false;
	public static bool upPressed = false;
	public static bool downPressed = false;
	public static bool firePressed = false;
	public static bool fire2Pressed = false;

	void Update()
	{
		if (Active && CheckGround)
		{
			float h = Input.GetAxis ("Horizontal");     // left -1 right +1
			float v = Input.GetAxis ("Vertical");       // up + 1 down -1

			bool space = false;
/*
			foreach (KeyCode c in System.Enum.GetValues(typeof(KeyCode)))
				if (Input.GetKey (c))
					Debug.Log(c.ToString());
*/
			foreach (KeyCode c in Joystics)
			{
				if (Input.GetKey (c))
				{
					space = true;
					break;
				}
			}

			if (Input.GetKeyDown (KeyCode.DownArrow) || downPressed)
				v = -1f;
			else if (Input.GetKeyDown (KeyCode.UpArrow) || upPressed)
				v = 1f;
			else if (Input.GetKeyDown (KeyCode.LeftArrow) || leftPressed)
				h = -1f;
			else if (Input.GetKeyDown (KeyCode.RightArrow) || rightPressed)
				h = 1f;
			
			if (v > 0.5f)
			{
				SetKey (AnimState.WalkU);
				UnSetKey (AnimState.WalkD);
			}
			else
			if (v < -0.5f)
				{
					UnSetKey (AnimState.WalkU);
					SetKey (AnimState.WalkD);
				}
				else
				{
					UnSetKey (AnimState.WalkU);
					UnSetKey (AnimState.WalkD);
				}

			if (h > 0.5f)
			{
				SetKey (AnimState.WalkR);
				UnSetKey (AnimState.WalkL);
			}
			else
			if (h < -0.5f)
				{
					UnSetKey (AnimState.WalkR);
					SetKey (AnimState.WalkL);
				}
				else
				{
					UnSetKey (AnimState.WalkR);
					UnSetKey (AnimState.WalkL);
				}

			if (space || firePressed || fire2Pressed)
				SetKey (AnimState.Jump);
			else
				UnSetKey (AnimState.Jump);
		}
		AfterUpdate();
	}

	void LateUpdate()
	{
		if (!Active)
			return;
		
		switch (TheField.getType(position))
		{
			case EntityType.Girl:
				TheGame.Friend (position);
			break;
			case EntityType.Fruit:
			    TheField.Catch(position);
			break;
			case EntityType.Water:
				TheGame.Sink(this, true);
			break;
			default:
			break;
		}
	}

}
