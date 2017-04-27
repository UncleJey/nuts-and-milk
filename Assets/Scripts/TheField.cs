using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public enum EntityType : byte
{
		 Empty		= 0
		,Wall		= 1
		,Water		= 2
		,Player		= 3
		,Enemy		= 4
		,Fruit		= 5
		,Girl		= 6
		,Stairway	= 7
		,Decor		= 8
		,Jumper		= 9
		,UnkNown	= 10
		,UnvisWall	= 11 // Незримая стена
}

[System.Serializable]
public class SpriteSet
{
	public string name;
	public EntityType type;
	public Sprite sprite;
}

//[System.Serializable]
public class Step
{
	public TheCell cell;
	public int stepType;

	public override string ToString ()
	{
		return string.Format (cell.position.ToString()+" : "+stepType.ToString());
	}
}

public class TheField : MonoBehaviour 
{
	public static TheField instance;
	public static Point Gaborites = new Point(16, 14);
	public static Point CellSize  = new Point(64, 50);
	public static bool isBonusLevel = false;

	public BonusPanel bonusPanel;
	public GameObject EditButtonsPanel;

	public Button loadButton;
	public Button saveButton;
	public Button resetButton;
	public Button buttonMenu;

	public TheCell imgPrefab;
	public SpriteSet[] spriteSet;
	TheCell[,] field = null;

	public TheCursor cursor;
	public static int fruitCount = 0;

	/// <summary>
	/// Индикатор позиции Х
	/// </summary>
	public Numbr XX;
	/// <summary>
	/// Индикатор позиции Y
	/// </summary>
	public Numbr YY;
	/// <summary>
	/// Индикатор уровня
	/// </summary>
	public Numbr LL;
	/// <summary>
	/// Индикатор номера спрайта
	/// </summary>
	public Numbr SS;

	public GameObject catchEffect;

	void Awake()
	{
		instance = this;
		loadButton.onClick.AddListener (()=>{Load (Save.playingLevel);});
		saveButton.onClick.AddListener (()=>{SaveLevel();});
		resetButton.onClick.AddListener (()=>{Load (Save.playingLevel, true);});
		buttonMenu.onClick.AddListener (()=>{SceneManager.LoadScene("loader");});

		EditButtonsPanel.SetActive (UILevels.editMode);

	}

	void Start()
	{
		instance.MoveCursor (0, 0);
		instance.cursor.gameObject.SetActive (UILevels.editMode);
	}

	float tm = 0.5f;
	void Update () 
	{
		if (UILevels.editMode)
		{
			tm -= Time.deltaTime;
			if (tm > 0)
				return;
			tm = 0.1f;

			if (Input.GetKeyDown (KeyCode.DownArrow) || Player.downPressed)
				MoveCursor (0, -1);
			if (Input.GetKeyDown (KeyCode.UpArrow) || Player.upPressed)
				MoveCursor (0, 1);

			if (Input.GetKeyDown (KeyCode.LeftArrow) || Player.leftPressed)
				MoveCursor (-1, 0);
			if (Input.GetKeyDown (KeyCode.RightArrow) || Player.rightPressed)
				MoveCursor (1, 0);

			// Спрайт
			if (Input.GetKeyDown (KeyCode.Minus) || Player.firePressed)
				ChangeSprite (-1);
			if (Input.GetKeyDown (KeyCode.Equals) || Player.fire2Pressed)
				ChangeSprite (1);

			// Уровень
			if (Input.GetKeyDown (KeyCode.KeypadMinus))
				ChangeLevel (-1);
			if (Input.GetKeyDown (KeyCode.KeypadPlus))
				ChangeLevel (1);

			if (Input.GetKeyDown (KeyCode.S))
				SaveLevel ();

			if (Input.GetKeyDown (KeyCode.L))
				Load (Save.playingLevel);
		}
	}

#region Game
	/// <summary>
	/// Слопать яблоко
	/// </summary>
	public static void Catch(Point pPoint)
	{
		TheCell cell = instance.field[pPoint.X,pPoint.Y];
		if (cell.sprite != 0) 
		{
			fruitCount--;
			Debug.Log ("Fruits: " + fruitCount.ToString ());
			cell.sprite = 0;
			RectTransform ert = Instantiate (instance.catchEffect).GetComponent<RectTransform> ();

			ert.transform.parent = cell.transform;
			ert.anchoredPosition = Vector2.zero;

			if (fruitCount <= 0)
			{
				TheGame.theGirl.ChangeState (GirlMode.Ready);
				TheGame.PlaySound (AnimState.Done);
			}
			else
				TheGame.PlaySound (AnimState.Eat);
		}
	}

	public static void ShowCatch(Point pPoint)
	{
		RectTransform ert = Instantiate (instance.catchEffect).GetComponent<RectTransform> ();

		ert.transform.parent = instance.transform;
		ert.anchoredPosition = (pPoint * CellSize).ToVector2();
	}

#endregion Game
#region editor
	int x=10,y = 10;

	/// <summary>
	/// Подвинуть курсор
	/// </summary>
	void MoveCursor(int dX, int dY)
	{
		x += dX;
		y += dY;

		if (x < 0)
			x = Gaborites.X - 1;
		if (x >= Gaborites.X)
			x = 0;

		if (y < 0)
			y = Gaborites.Y - 1;
		if (y >= Gaborites.Y)
			y = 0;

		XX.Value = x + 1;
		YY.Value = y + 1;

		TheCell cell = getCell (x, y);
		cursor.transform.parent = cell.transform;
		RectTransform rt = cursor.GetComponent<RectTransform> ();
		rt.anchoredPosition = Vector2.zero;
		rt.sizeDelta = Vector2.zero;// cell.GetComponent<RectTransform> ().sizeDelta;
		mooved = true;
	}

	int sprNo = 0;
	bool mooved = false;
	/// <summary>
	/// Поменять спрайт
	/// </summary>
	void ChangeSprite(int pMoveTo)
	{
		if (!mooved) 
		{
			sprNo += pMoveTo;
			if (sprNo >= spriteSet.Length)
				sprNo = 0;
			if (sprNo < 0)
				sprNo = spriteSet.Length - 1;
		}
		TheCell cell = getCell (x, y);
		cell.sprite = sprNo;
		mooved = false;

		SS.Value = sprNo;
	}

	/// <summary>
	/// Поменять номер уровня
	/// </summary>
	public void ChangeLevel(int pMoveTo)
	{
		Save.playingLevel += pMoveTo;
		if (Save.playingLevel < 1)
			Save.playingLevel = 50;
		if (Save.playingLevel > 50)
			Save.playingLevel = 1;

		LL.Value = Save.playingLevel;
	}

	/// <summary>
	/// Сохранить уровень
	/// </summary>
	void SaveLevel()
	{
		string val = "";
		for (int j = 0; j < Gaborites.Y; j++) 
			for (int i = 0; i < Gaborites.X; i++) 
				val += field [i, j].sprite.ToString () + ",";

		System.IO.File.WriteAllText(levPath(Save.playingLevel),val);
	}

	string levPath(int pLevNo, string bd = "")
	{
		string pth = /*Application.dataPath +*/ "/Resources/Levels/" + pLevNo.ToString ("X2") + bd;// + ".txt";
		Debug.Log (pth);
		return pth;
	}

	/// <summary>
	/// Загрузить уровень
	/// </summary>
	public static void LoadLevel(int pLevelNo)
	{
		instance.Load (pLevelNo);

		// Подготавливаем к игре
		if (!UILevels.editMode)
		{
			if (isBonusLevel)
			{
				instance.bonusPanel.gameObject.SetActive (true);
			}
			else
			{
				instance.bonusPanel.gameObject.SetActive (false);
			}
		}
	}

	public static string LoadFromFile(string filePath)
	{
		TextAsset xFile;
		string t;

		if(filePath.StartsWith("/Resources"))
		{
			filePath = filePath.Substring(11);
			xFile = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
			if (xFile == null)
				return string.Empty;
			t = xFile.text;
		}
		else
		{	
			StreamReader sr = new StreamReader(Application.dataPath + filePath + ".txt");
			t = sr.ReadToEnd();
			sr.Close();
		}
		return t;
	}


	/// <summary>
	/// Загрузить уровень
	/// </summary>
	public void Load(int pLevNo, bool defaultOnly = false)
	{
		bool defLevel = false;
		bool userLevel = false;
		string txt = "";
		fruitCount = 0;
		isBonusLevel = false;

		//user created
		if (!defaultOnly)
		{
			txt = LoadFromFile(levPath (pLevNo));

			// user & bonus
			if (string.IsNullOrEmpty (txt))
			{
				txt = LoadFromFile(levPath (pLevNo, "b"));
				if (!string.IsNullOrEmpty (txt))
				{
					userLevel = true;
					isBonusLevel = true;
				}
			}
		}

		// default
		if (string.IsNullOrEmpty (txt))
		{
			txt = LoadFromFile(levPath (pLevNo, "d"));
			if (!string.IsNullOrEmpty (txt))
			{
				defLevel = true;
			}
		}


		// default & bonus
		if (string.IsNullOrEmpty (txt))
		{
			txt = LoadFromFile(levPath (pLevNo, "db"));
			if (!string.IsNullOrEmpty (txt))
			{
				defLevel = true;
				isBonusLevel = true;
			}
		}

		string[] cls = txt.Split (',');
		int nr = 0;
		int t = 0;

		for (int j = 0; j < Gaborites.Y; j++)
		{
			for (int i = 0; i < Gaborites.X; i++)
			{
				t = 0;
				if (cls.Length > nr)
					int.TryParse (cls [nr++], out t);
				else
					t = 0;
				SpriteSet spr = spriteSet [0];
				if (t < spriteSet.Length)
					spr = spriteSet [t];
				else
					t = 0;

				if (!UILevels.editMode)
				{
					switch (spr.type) // Определяем позиици ключевых элементов
					{
						case EntityType.Player:
							TheGame.thePlayer.startPos = new Point (i, j);
							t = 0;
						break;
						case EntityType.Enemy:
							TheGame.enemyPoints.Add(new Point (i, j));
							t = 0;
						break;
						case EntityType.Girl:
							TheGame.theGirl.startPos = new Point (i, j);
							t = 0;
						break;
					}
				}
				field [i, j].fruit = spr.type == EntityType.Fruit;
				field [i, j].invisible = spr.type == EntityType.UnvisWall;
				field [i, j].sprite = t;

				if (field [i, j].fruit)
					fruitCount++;
				field [i, j].ClearNeighbors ();
			}
		}
	}

#endregion editor

#region generation
	public static TheCell getCell(Point pPoint)
	{
		return getCell (pPoint.X, pPoint.Y);
	}

	public static TheCell getCell(int pX, int pY, bool pCorrect = false)
	{
		if (pCorrect) 
		{
			if (pX < 0)
				pX = Gaborites.X + pX;
			else if (pX >= Gaborites.X)
				pX -= Gaborites.X;
		}
		else 
		{
			if (pX < 0 || pX >= Gaborites.X)
				return null;
		}

		if (pY < 0 || pY >= Gaborites.Y)
			return null;

		return instance.field[pX,pY];
	}

	public static void Generate()
	{
		instance.generate ();
	}

	void generate()
	{
		if (field == null) 
		{
			TheCell img = null;
			field = new TheCell[Gaborites.X, Gaborites.Y];
			for (int j = 0; j < Gaborites.Y; j++) 
			{
				for (int i = 0; i < Gaborites.X; i++) 
				{
					if (img == null)
						img = imgPrefab;
					else
						img = Instantiate (imgPrefab.gameObject).GetComponent<TheCell> ();

					RectTransform irt = img.GetComponent<RectTransform> ();
					irt.parent = GetComponent<RectTransform> ();
					img.transform.localScale = Vector3.one;
					img.sprite = 0;
					img.position = new Point (i, j);
					irt.anchoredPosition = (img.position * CellSize).ToVector2();
					img.name = "Cell_" + i.ToString () + "_" + j.ToString ();
					field [i, j] = img;
				}
			}
		}
	}

	/// <summary>
	/// Получить спрайт по номеру
	/// </summary>
	public static Sprite getSprite(int pNo)
	{
		if (pNo <= 0 || pNo >= instance.spriteSet.Length)
			return instance.spriteSet [0].sprite;

		return instance.spriteSet [pNo].sprite;
	}

	/// <summary>
	/// Получить тип ячейки по координатам
	/// </summary>
	public static EntityType getType(Point pPoint, bool pCorrect = false)
	{
		/*
		if (pPoint.X < 0)
			pPoint.X = Gaborites.X - pPoint.X;
		else if (pPoint.X >= Gaborites.X)
			pPoint.X -= Gaborites.X;
*/
		TheCell cell = getCell(pPoint.X, pPoint.Y, pCorrect);
		if (cell == null)
			return EntityType.UnkNown;
		
		return instance.spriteSet [cell.sprite].type;
	}

	public static EntityType WalkType(int pX, int pY)
	{
		return WalkType (new Point (pX, pY));
	}
	/// <summary>
	/// Что можно делать с точкой
	/// </summary>
	public static EntityType WalkType(Point pPoint)
	{
		EntityType t = getType (pPoint);
		switch (t) 
		{
			case EntityType.Decor:
			case EntityType.Fruit:
			case EntityType.Girl:
			case EntityType.Water:
			case EntityType.Empty:
			case EntityType.Player:
			case EntityType.UnkNown:
			case EntityType.Enemy:
				return EntityType.Empty; // Пусто
			break;
			case EntityType.Stairway:
				return EntityType.Stairway; // Лестница
			break;
			case EntityType.Jumper:
			case EntityType.Wall:
			case EntityType.UnvisWall:
				return EntityType.Wall; // Стена
			break;
			default:
				Debug.LogError ("Unknown type " + t.ToString ());
				return EntityType.UnkNown;
			break;
		}
	}
#endregion generation

#region pathfind

	/// <summary>
	/// Номер иттерации
	/// </summary>
	static int ittNo = 0;
	static Queue<TheCell> heap = new Queue<TheCell>();
	/// <summary>
	/// Очистить шаги
	/// </summary>
	public static Step FindNextCell(Point pFrom, Point pTo, int pActionPrime, int pActionSec)
	{
		TheCell cellFrom = getCell (pFrom); 

		cellFrom.stepIttNo = ++ittNo;
		heap.Clear ();
		heap.Enqueue (cellFrom);
		bool first = true;

		while (heap.Count > 0) 
		{
			TheCell cell = heap.Dequeue ();

			foreach (Step stp in cell.Neighbors) 
			{
				if (stp.cell.stepIttNo != ittNo) 
				{
					stp.cell.stepIttNo = ittNo;
					stp.cell.firstStep = first ? stp:cell.firstStep;
					if (pTo == stp.cell.position)
						return stp.cell.firstStep;
					heap.Enqueue (stp.cell);
				}
			}
			first = false;
		}

		/*if (cell == null) 
		{
			foreach (Step s in cellFrom.Neighbors) 
			{
				if ((s.stepType & pActionPrime) == pActionPrime)
					return s.cell;
			}
			foreach (Step s in cellFrom.Neighbors) 
			{
				if ((s.stepType & pActionSec) == pActionSec)
					return s.cell;
			}
		}
		*/

		return null;
	}

#endregion pathfind
}
