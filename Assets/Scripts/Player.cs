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

	public static bool rightPressed = false;
	public static bool leftPressed = false;
	public static bool upPressed = false;
	public static bool downPressed = false;
	public static bool firePressed = false;
	public static bool fire2Pressed = false;

	float scale = 400;

	float h,v;
	int hh=0;
	int vv=0;
	bool space = false;

	public static KeyCode[] Joystics = new KeyCode[] 
	{
		KeyCode.JoystickButton0	// треугольник
		,KeyCode.JoystickButton1	// круг
		,KeyCode.JoystickButton2	// квадрат
		,KeyCode.JoystickButton3	// крестик
		,KeyCode.JoystickButton4	// LD
		,KeyCode.JoystickButton5	// RD
		,KeyCode.JoystickButton6	// LU
		,KeyCode.JoystickButton7	// RU
		,KeyCode.JoystickButton8	// select
		,KeyCode.JoystickButton9	// start
		,KeyCode.JoystickButton10	// Left pad Click
		,KeyCode.JoystickButton11	// Right pad Click
		,KeyCode.Space				// Пробел
	};

	Vector3 mousePos = Vector3.zero;
	float distance = 0;
	void checkMove()
	{
		if (Input.GetMouseButtonDown(0))
		{
			mousePos = Input.mousePosition;
			distance = 0;
		}

		if (Input.GetMouseButtonUp (0)) 
		{
			distance = 0;
			hh = 0;
			vv = 0;
		}

		if (Input.GetMouseButton(0))
		{
			float dx = Input.mousePosition.x - mousePos.x;
			float dy = Input.mousePosition.y - mousePos.y;

			float adx = dx<0?0-dx:dx;
			float ady = dy<0?0-dy:dy;

			if (adx + ady > 25)
			{
				//Debug.Log(adx + ady);
				distance = 45;
				mousePos = Input.mousePosition;
				if (adx > ady)
				{
					if (dx > 0)
						hh = 1 ;//moveRight();
					else
						hh = -1;//moveLeft();
				}
				else
				{
					if (dy < 0)
						vv = -1;//moveBack();
					else
						vv = 1;//	boost();
				}
			}
			//mousePos = Vector3.zero;
		}
	}

	void Update () 
	{
		h = 0; v = 0;
		bool space = false;
		if (Active && CheckGround)
		{

			foreach (KeyCode c in Joystics)
				if (Input.GetKeyDown (c))
					space = true;

			h = Input.GetAxisRaw("Horizontal");	// left -1 right +1
			v = Input.GetAxisRaw("Vertical");	// up + 1 down -1
			if (v == 0 && h == 0)
			{
				checkMove();
				h = hh; v = vv;
			}

			float delta = Time.deltaTime * scale;

			if (h > 0 || rightPressed)
				h = 1;
			else if (h < 0 || leftPressed)
				h = -1;
			else if (v > 0 || upPressed)
				v = 1;
			else if (v < 0 || downPressed)
				v = -1;
		}

		if (space)
			SetKey (AnimState.Jump);
		else
			UnSetKey (AnimState.Jump);

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
