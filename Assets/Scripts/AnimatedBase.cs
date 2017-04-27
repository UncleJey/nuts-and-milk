using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Типы состояний
/// </summary>
public enum AnimState : int
{
	 Stay  = 0
	,WalkL = 1
	,WalkR = 2
	,Jump  = 4
	,WalkU = 8
	,WalkD = 16
	,Die   = 32
	,Born  = 64
	,FalL  = 128
	,FalR  = 256
	,Eat   = 512
	,Jump2 = 1024
	,Done  = 2048
	,Falling = 4096
}

public class AnimatedBase : MonoBehaviour 
{
	/// <summary>
	/// Текущее состояние
	/// </summary>
	protected AnimState curState = AnimState.Stay;
	public RectTransform rt;
	/// <summary>
	/// Точка спавна
	/// </summary>
	public Point startPos = Point.Zero;
	public bool iAmEnemy = false;

	/// <summary>
	/// Приращение скорости бега
	/// </summary>
	public int SpeedRunInc = 1;
	/// <summary>
	/// Максимальная скорость бега
	/// </summary>
	public int SpeedRunMax = 2;

	/// <summary>
	/// Начальная скорость прыжка
	/// </summary>
	public int SpeedJumpStart = 8;

	/// <summary>
	/// Гравитация
	/// </summary>
	public int FallSpeedInc = -1;
	/// <summary>
	/// Максимальная скорость падения
	/// </summary>
	public int FallSpeedMax;

	/// <summary>
	/// Текущая скорость Y
	/// </summary>
	public Point speed = Point.Zero;

	/// <summary>
	/// Через сколько проснёмся когда упали
	/// </summary>
	float awakeTimer = 0;

	/// <summary>
	/// Активен
	/// </summary>
	public bool Active = false;
	public bool CheckGround = true;

	Animator animator;
	protected virtual void Awake()
	{
		animator = GetComponent<Animator> ();
		rt = GetComponent<RectTransform> ();
	}

	/// <summary>
	/// Состояние поменялось
	/// </summary>
	public bool PlaySound = false;

	/// <summary>
	/// Переключение состояний
	/// </summary>
	public void ChangeState(AnimState pState)
	{
		if (curState != pState) 
		{
			//animator.ResetTrigger (curState.ToString());
			//Debug.Log (pState.ToString ());
			curState = pState;

			if (curState == AnimState.Jump) 
			{
				animator.SetTime (0);
				animator.enabled = false;

			}
			else 
			{
				animator.Play (pState.ToString ());
				//animator.SetTrigger (pState.ToString ());
				animator.enabled = true;
				if ((pState == AnimState.FalL || pState == AnimState.FalR) && PlaySound)
					TheGame.PlaySound (AnimState.Falling);
			}
		}
	}

	/// <summary>
	/// Переключиться обратно из анимации прыжка
	/// </summary>
	void UnJump()
	{
		speed.Y = 0;
		flying = false;
		animator.enabled = true;
	}
#region directions
	/// <summary>
	/// Какие кнопки нажаты
	/// </summary>
	protected int Keys { get; set;}

	/// <summary>
	/// Поднять флаг для кнопки
	/// </summary>
	public void SetKey(AnimState pState)
	{
		int st = (int)pState;

		if ((Keys & st) == 0)
			Keys += st;
	}

	/// <summary>
	/// Опустить флаг для кнопки
	/// </summary>
	public void UnSetKey(AnimState pState)
	{
		int st = (int)pState;

		if ((Keys & st) != 0)
			Keys -= st;
	}

	/// <summary>
	/// Поднят ли флаг для кнопки
	/// </summary>
	protected bool IsSetKey(AnimState pState)
	{
		return ((Keys & ((int)pState)) != 0);
	}
#endregion directions

#region move
	/// <summary>
	/// Падение
	/// </summary>
	bool _flying  = false;
	/// <summary>
	/// Прыжок
	/// </summary>
	bool _jumping = false;

	/// <summary>
	/// Моб находится в полёте
	/// </summary>
	protected bool flying
	{
		get
		{
			return _flying || _jumping;
		}
		set
		{
			//if (iAmEnemy)
			//	Debug.Log ("Set flying " + value);
			_flying = value;
			_jumping = false;
		}
	}

	/// <summary>
	/// Аналоговое смещение моба относительно его текущей дискретной позиции
	/// </summary>
	public Point offset  = Point.Zero;
	[SerializeField]
	Point _position = Point.Zero;
	/// <summary>
	/// Координаты
	/// </summary>
	public Point position
	{
		get 
		{
			return _position;
		}
		set 
		{
			_position = value;
		}
	}
	/// <summary>
	/// Может ли прыгнуть
	/// </summary>
	protected bool canJump
	{
		get
		{
			if (awakeTimer > 0)
				return false;
			
			EntityType t = TheField.getType (position + Point.Down);
			return t == EntityType.Wall || t == EntityType.Jumper || t == EntityType.UnvisWall;
		}
	}

	/// <summary>
	/// Стоит ли на платформе
	/// </summary>
	protected bool platformed
	{
		get
		{
			if (!CheckGround)
				return false;
			EntityType t = TheField.getType (position + Point.Down);
			return (
				   t == EntityType.Wall 
				|| t == EntityType.Stairway
				|| t == EntityType.Jumper
				|| t == EntityType.UnvisWall
				|| TheField.getType (position) == EntityType.Stairway);
		}
	}

	/// <summary>
	/// На ступеньках
	/// </summary>
	protected bool OnStairs(bool pIsUp)
	{
		if (!CheckGround)
			return false;

		if (TheField.getType (position) == EntityType.Stairway)
			return true;

		if (pIsUp)
			return false;
		else
			return TheField.getType (position + Point.Down) == EntityType.Stairway;
	}

	/// <summary>
	/// На прыгуне
	/// </summary>
	protected bool OnJumper
	{
		get 
		{
			return TheField.getType (position + Point.Down) == EntityType.Jumper;
		}
	}

	/// <summary>
	/// Телепортироваться в точку
	/// </summary>
	public void Teleport(Point pPoint)
	{
		Stop ();
		Debug.Log ("teleport "+name+" to " + pPoint.ToString ());
		position = pPoint.Clone();
		offset = Point.Zero;
		UpdatePos();
	}

	/// <summary>
	/// Остановиться
	/// </summary>
	public void Stop()
	{
		Debug.Log ("Stop");
		speed = Point.Zero;
		_jumping = false;
		_flying = false;
		Keys = 0;
		ChangeState (AnimState.Stay);
		Move (Point.Zero);
	}

	/// <summary>
	/// Обновить позицию
	/// </summary>
	protected void UpdatePos()
	{
		rt.anchoredPosition = (_position * TheField.CellSize + offset).ToVector2();
	}

	/// <summary>
	/// Подвинуться на плавающую величину
	/// </summary>
	protected void Move(Point pDelta)
	{
		offset += pDelta;
		Point delta = NextStep (pDelta);
		offset -= delta * TheField.CellSize;
		_position += delta;
		if (_position.X < 0)
			_position.X = TheField.Gaborites.X - 1;
		else if (_position.X >= TheField.Gaborites.X)
			_position.X = 0;
		
		UpdatePos();
	}

	/// <summary>
	/// На сколько клеток сдвинется персонаж при перемещении
	/// </summary>
	protected Point NextStep(Point pDelta)
	{
		Point vPosition = Point.Zero;
		Point vOffset = pDelta + offset;

		if (vOffset.X >= TheField.CellSize.X)
			vPosition.X++;
		else if (vOffset.X < 0)
			vPosition.X--;

		if (vOffset.Y >= TheField.CellSize.Y )
			vPosition.Y++;
		else if (vOffset.Y < 0)
			vPosition.Y--;

		return vPosition;
	}

	/// <summary>
	/// Может ли перс переместиться
	/// </summary>
	bool CanMove(Point pDelta,  bool pIsWalk = false)
	{
		if (!CheckGround)
			return true;
		EntityType t = TheField.getType (position + NextStep (pDelta));
		return (t != EntityType.Wall && t != EntityType.Jumper && t != EntityType.UnvisWall);
	}

	/// <summary>
	/// Лежит
	/// </summary>
	bool lying
	{
		get 
		{
			if (iAmEnemy)
				return false;
			return (curState == AnimState.FalL) || (curState == AnimState.FalR);
		}
		set 
		{
			awakeTimer = 0;
			ChangeState (AnimState.Stay);
		}
	}

	/// <summary>
	/// Эмуляция движения и физики
	/// </summary>
	protected void AfterUpdate()
	{
		if (awakeTimer > 0)
			awakeTimer -= Time.deltaTime;

		// Если не летим, то проверям кнопки
		if (!flying) 
		{
			if (IsSetKey (AnimState.WalkL) && !lying)
			{
				speed.Y = 0;
				if (speed.X > -SpeedRunMax) 
				{
					speed.X -= SpeedRunInc;
					ChangeState (AnimState.WalkL);
				}
			} 
			else if (IsSetKey (AnimState.WalkR) && !lying) 
			{
				speed.Y = 0;
				if (speed.X < SpeedRunMax) 
				{
					speed.X += SpeedRunInc;
					ChangeState (AnimState.WalkR);
				}
			} 
			else if (speed.X > 0) 
			{
				if (iAmEnemy)
					speed.X = 0;
				else 
					speed.X -= SpeedRunInc;
			}
			else if (speed.X < 0) 
			{
				if (iAmEnemy)
					speed.X = 0;
				else 
					speed.X += SpeedRunInc;
			}

			if (IsSetKey (AnimState.WalkD) && OnStairs (false) && (offset.Y > 0 || !canJump))
			{
				speed.X = 0;

				if (speed.Y > -SpeedRunMax) 
				{
					speed.Y -= SpeedRunInc;
					ChangeState (AnimState.WalkD);
				}
				if (offset.X > TheField.CellSize.X >> 1)
					offset.X--;
				else if (offset.X < TheField.CellSize.X >> 1)
					offset.X++;
			} 
			else if (IsSetKey (AnimState.WalkU) && OnStairs (true)) 
			{
				speed.X = 0;
				if (speed.Y < SpeedRunMax) 
				{
					speed.Y += SpeedRunInc;
					ChangeState (AnimState.WalkU);
				}
			//	if (iAmEnemy) 
				{
					if (offset.X > TheField.CellSize.X >> 1)
						offset.X--;
					else if (offset.X < TheField.CellSize.X >> 1)
						offset.X++;
				}
			} 
			else if (curState == AnimState.WalkU || curState == AnimState.WalkD) 
			{
				speed.Y = 0;
				ChangeState (AnimState.Stay);
			}

			if (IsSetKey (AnimState.Jump) && canJump) 
			{
				//Debug.Log ("J");
				if (curState == AnimState.FalL || curState == AnimState.FalR)
					ChangeState (AnimState.Stay);

				_jumping = true;
				speed.Y += SpeedJumpStart;

				if (iAmEnemy) 
				{
					if (IsSetKey (AnimState.WalkR))
						speed.X = SpeedRunMax;
					else if (IsSetKey (AnimState.WalkL))
						speed.X = -SpeedRunMax;
					else
						speed.X = 0;
				}
				else
				{
					if (Mathf.Abs(speed.X) > (SpeedRunInc << 1))
					{
						speed.X = SpeedRunMax * (int)Mathf.Sign(speed.X);
						ChangeState (AnimState.Jump);
					}
					else 
					{
						speed.X = 0;
						ChangeState (AnimState.Stay);
					}
				}

				if (PlaySound)
					TheGame.PlaySound (AnimState.Jump);
			}

			if (!platformed) 
			{
				_flying = true;
				offset.Y -= 1;
			}
		}
		else if (OnStairs(true))// Если уже летим куда-то и на ступеньках оказались
		{
			UnJump();
			if (offset.Y < 0)
				offset.Y = 0;
			Move (Point.Zero);
		}

		// Гравитация
		if (offset.Y != 0 && !OnStairs (true)) 
		{
			speed.Y -= FallSpeedInc;
			flying = true;
		}

		// Смещение к новой позиции
		Point dblSpeed = speed * Time.deltaTime;

		//Debug.Log (speed.Y);
		// -- Теперь проверяем ограничения
		// Проверяем ось Y
		if (speed.Y > 0) // Летим вверх
		{
			if (!CanMove(dblSpeed + new Point(0,TheField.CellSize.Y)))
			{
				if (CanMove (new Point (0, dblSpeed.Y + TheField.CellSize.Y)))
					dblSpeed.X = 0;
				else
				if (speed.Y > 200)
					{
						Debug.Log (speed.Y);
						speed.Y = speed.Y / 2;
					}
			}
		}
		else if (speed.Y < 0) // Летим вниз
		{
			if (speed.Y < -800 && !OnStairs(false) && !iAmEnemy)
			{
				if (!lying)
				{
					if (curState == AnimState.WalkR)
						ChangeState (AnimState.FalR);
					else if (curState == AnimState.WalkL)
						ChangeState (AnimState.FalL);
					else
						ChangeState (Random.value>0.5f?AnimState.FalL:AnimState.FalR);
				}
				awakeTimer = 1f;
			}

			if (!CanMove(dblSpeed))
			{
				if (CanMove (new Point (0, dblSpeed.Y))) 
				{
					dblSpeed.X = 0;
				}
				else
				{
					Move (Point.Zero);
					offset.Y = 0;
					dblSpeed.Y = 0;
					Move (Point.Zero);
					UnJump();

					if (OnJumper)
					{
						TheGame.PlaySound (AnimState.Jump2);
						flying = true;
						speed.Y = SpeedJumpStart;

						if (IsSetKey (AnimState.Jump) && !lying)
							speed.Y += SpeedJumpStart >> 1;

						if (IsSetKey (AnimState.WalkL))
							speed.X = -SpeedRunMax;
						else if (IsSetKey (AnimState.WalkR)) 
							speed.X = SpeedRunMax;
					}
					else
					{
						if (lying) 
						{
							if (OnStairs (true))
								lying = false;
							else
								TheGame.ShakeCam ();
						}

						if (!OnStairs(true) && PlaySound)
							TheGame.PlaySound (AnimState.FalL);
					}
				}
			}
		}

		if (speed.Empty) 
		{
			if (!lying)
				ChangeState (AnimState.Stay);
		} 
		else 
		{
			if (!flying && speed.Y == 0 && offset.Y != 0 &&  //Чит - если ползли по лестнице и не доползли до конца, но повернули то помогаем доползти
				TheField.getType(position) == EntityType.Stairway &&
				TheField.getType(position + speed.Normalized) == EntityType.Wall
			) 
			{
				if (offset.Y > (TheField.CellSize.Y >> 1))
					offset.Y += 5;
			}
			if (dblSpeed.X != 0 && !CanMove(dblSpeed))
				dblSpeed.X = 0;

			Move (dblSpeed);
		}
	}
#endregion move

	public virtual void Spawn(bool pEmmediate)
	{
		gameObject.SetActive (true);
		Teleport(startPos);
		rt.SetAsLastSibling();
	}
}
