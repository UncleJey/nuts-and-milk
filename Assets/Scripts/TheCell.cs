using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TheCell : MonoBehaviour 
{
	public Step firstStep;
	public int stepIttNo = 0;
	public RectTransform rt;

	Image image;
	void Awake()
	{
		rt = GetComponent<RectTransform> ();
		if (image == null)
			image = GetComponent<Image> ();
	}

	public bool invisible = false;
	public bool fruit = false;
	/// <summary>
	/// Позиция ячейки на карте
	/// </summary>
	public Point position;

	int _sprite=0;
	public int sprite
	{
		get 
		{
			return _sprite;
		}
		set 
		{
			_sprite = value;

			if (!UILevels.editMode && invisible)
				image.enabled = false;
			else if (_sprite == 0)
				image.enabled = false;
			else 
			{
				image.enabled = true;
				image.sprite = TheField.getSprite(value);
			}
		}
	}
		
#region Neighbors

	bool haveDownGround(int pX, int pY)
	{
		Point p = new Point (pX, pY);
		while (p.Y > 0)
		{
			EntityType rt = TheField.getType (p);
			EntityType et = TheField.WalkType (p);

			if (et == EntityType.Stairway || et == EntityType.UnvisWall || et == EntityType.Wall)
				return rt != EntityType.Jumper;

			if (et == EntityType.Water)
				return false;

			p.Y--;
		}
		return false;
	}

	List<Step> _neighbors = null;
	/// <summary>
	/// Получить список соседей
	/// </summary>
	public List<Step> Neighbors
	{
		get 
		{
			if (_neighbors == null) 
			{
				_neighbors = new List<Step> ();

				EntityType[,] st = new EntityType[5,5];
				for (int i = 0; i < 5; i++) 
				{
					for (int j = 0; j < 5; j++) 
					{
						st [i, j] = TheField.WalkType (position + new Point (i - 2, j - 2));
					}
				}

				// на лестнице
				if (st [2, 2] == EntityType.Stairway) 
				{
					//верх
					if (st [2, 3] == EntityType.Stairway || st [2, 3] == EntityType.Empty)
						AddNeighbor (2, 3, (int)AnimState.WalkU);
					//лево
					if (st [1, 2] == EntityType.Stairway || (st [1, 2] == EntityType.Empty && (st [1, 1] == EntityType.Wall || st [1, 1] == EntityType.Stairway)))
						AddNeighbor (1, 2, (int)AnimState.WalkL);
					//право
					if (st [3, 2] == EntityType.Stairway || (st [3, 2] == EntityType.Empty && (st [3, 1] == EntityType.Wall || st [3, 1] == EntityType.Stairway)))
						AddNeighbor (3, 2, (int)AnimState.WalkR);
					//низ
					if (st [2, 1] == EntityType.Stairway || (st [2, 1] == EntityType.Empty && st [2, 0] == EntityType.Wall ))
						AddNeighbor (2, 1, (int)AnimState.WalkD);
					else
					// падение вниз
						if (st [2, 1] == EntityType.Empty && haveDownGround(position.X, position.Y-1))
							AddDownNeighbor(2, 1, (int)AnimState.WalkD);

				}
				else if (st [2, 2] == EntityType.Empty && (st [2, 1] == EntityType.Wall ||  st [2, 1] == EntityType.Stairway))
				{
					//низ
					if (st [2, 1] == EntityType.Stairway || (st [2, 1] == EntityType.Empty && st [2, 0] == EntityType.Wall ))
						AddNeighbor (2, 1, (int)AnimState.WalkD);

					//право
					if (st [3, 2] == EntityType.Stairway || (st [3, 2] == EntityType.Empty && (st [3, 1] == EntityType.Wall || st [3, 1] == EntityType.Stairway )))
						AddNeighbor (3, 2, (int)AnimState.WalkR);
					//лево
					if (st [1, 2] == EntityType.Stairway || (st [1, 2] == EntityType.Empty && (st [1, 1] == EntityType.Wall || st [1, 1] == EntityType.Stairway)))
						AddNeighbor (1, 2, (int)AnimState.WalkL);

					/// Прыжки
					if (st [2, 1] == EntityType.Wall) 
					{
						// верх+1
						if (st [2, 3] == EntityType.Stairway)
							AddNeighbor (2, 3, (int)AnimState.Jump);

						//право - верх
						if (st [2, 3] == EntityType.Empty && 
							st [3, 3] == EntityType.Empty && 
							st [4, 2] != EntityType.Empty && 
							st [4, 3] == EntityType.Empty)
							AddNeighbor (4, 3, (int)AnimState.Jump + (int)AnimState.WalkR);
						//лево - верх 
						if (st [2, 3] == EntityType.Empty && 
							st [1, 3] == EntityType.Empty && 
							st [0, 2] != EntityType.Empty && 
							st [0, 3] == EntityType.Empty)
							AddNeighbor (0, 3, (int)AnimState.Jump + (int)AnimState.WalkL);

						//право
						if (st [4, 2] == EntityType.Empty &&
						    st [2, 3] == EntityType.Empty &&
						    st [3, 2] == EntityType.Empty &&
						    st [3, 3] == EntityType.Empty &&
						    st [3, 1] == EntityType.Empty) 
						{
							if (st [4, 1] != EntityType.Empty)
								AddNeighbor (4, 2, (int)AnimState.Jump + (int)AnimState.WalkR);
							else if (st [4, 0] != EntityType.Empty)
								AddNeighbor (4, 1, (int)AnimState.Jump + (int)AnimState.WalkR);
						}

						//лево
						if (st [0, 2] == EntityType.Empty &&
						    st [2, 3] == EntityType.Empty &&
						    st [1, 2] == EntityType.Empty &&
						    st [1, 3] == EntityType.Empty &&
							st [1, 1] == EntityType.Empty 
						) 
						{
							if (st [0, 1] != EntityType.Empty)
								AddNeighbor (0, 2, (int)AnimState.Jump + (int)AnimState.WalkL);
							else if (st [0, 0] != EntityType.Empty)
								AddNeighbor (0, 1, (int)AnimState.Jump + (int)AnimState.WalkL);
						}
					}
					// Падения
					// лево
					if (st[1,1] == EntityType.Empty && st[1,2] == EntityType.Empty && st[1,0] == EntityType.Empty && 
						(TheField.WalkType (position + new Point (-1, -3)) == EntityType.Wall || TheField.WalkType (position + new Point (-1, -3)) == EntityType.Stairway ))
						AddNeighbor (1, 0, (int)AnimState.WalkL);
					else 
						if (
							   /*st[0,1] == EntityType.Empty 
							&& */st[0,0] == EntityType.Empty 
							&& st[1,1] == EntityType.Empty 
							&& st[1,0] == EntityType.Empty 
							&& st[1,2] == EntityType.Empty 
							&& haveDownGround(position.X-2, position.Y-2)
						)
							AddDownNeighbor (0, 0, (int)AnimState.WalkL);

					//право
					if (st[3,1] == EntityType.Empty && st[3,2] == EntityType.Empty && st[3,0] == EntityType.Empty && 
						(TheField.WalkType (position + new Point (1, -3)) == EntityType.Wall || TheField.WalkType (position + new Point (1, -3)) == EntityType.Stairway))
						AddNeighbor (3, 0, (int)AnimState.WalkR);
					else 
						if (	/*st[4,1] == EntityType.Empty 
							 && */st[3,1] == EntityType.Empty
							 && st[3,2] == EntityType.Empty
							 && st[3,0] == EntityType.Empty
							 && st[4,0] == EntityType.Empty
							 && haveDownGround(position.X+2, position.Y+2))
							AddDownNeighbor (4, 0, (int)AnimState.WalkL);


				}
			}
			///  .
			/// 04 14 24 34 44
			/// 03 13 23 33 43
			/// 02 12.22.32 42
			/// 01 11 21 31 41
			/// 00 10 20 30 40
			return _neighbors;
		}
	}

	/// <summary>
	/// Очистить список соседей
	/// </summary>
	public void ClearNeighbors()
	{
		if (_neighbors != null) 
		{
			_neighbors.Clear ();
			_neighbors = null;
		}
	}

	void AddNeighbor(int dx, int dy, int pStepType)
	{
		Step step = new Step ();
		step.cell = TheField.getCell(position.X + dx - 2, position.Y + dy - 2);
		step.stepType = pStepType;
		_neighbors.Add (step);
	}

	void AddDownNeighbor(int dx, int dy, int pStepType)
	{
		int xx = position.X + dx - 2;
		int yy = position.Y + dy - 2;

		while (yy > 0)
		{
			EntityType et = TheField.WalkType (xx, yy);
			if (et == EntityType.Wall || et == EntityType.Stairway || et == EntityType.UnvisWall)
			{
				Step step = new Step ();
				step.cell = TheField.getCell (xx, yy+1);
				step.stepType = pStepType;
				_neighbors.Add (step);
				return;
			}
			yy--;
		}
	}

	static Vector3 sw = new Vector3(32,0,25);
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawCube (transform.position + sw, Vector3.one*10);
		if (_neighbors == null && Application.isPlaying)
			_neighbors = Neighbors;
		if (_neighbors != null) 
		{
			foreach (Step s in _neighbors) 
			{
				//Gizmos.DrawCube (s.cell.transform.position, Vector3.one*5);
				Gizmos.DrawLine(transform.position + sw, s.cell.transform.position + sw);
			}
		}
	}

	public override string ToString ()
	{
		return string.Format ("[TheCell: sprite={0}, Position={1}]", sprite, position.ToString());
	}
#endregion Neighbors
}

