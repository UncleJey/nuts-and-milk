using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MusicType : byte
{
	 None
	,MainTheme
	,MainTheme2
	,Intro
	,Complete
	,Loved
	,Prepare
	,Loose
	,Prepare2
}

[System.Serializable]
public class Musics
{
	public string Name;
	public AudioClip clip;
	public MusicType state;
	public int length;
}

[System.Serializable]
public class Sounds
{
	public string Name;
	public AudioClip clip;
	public AnimState state;
}

public class TheGame : MonoBehaviour 
{
	static TheGame Instance;
	public Animation shakeAnim;
	public Sounds[] sounds;
	public Musics[] music;

	public Digit milkCount;
	public Numbr levelNo;
	public Numbr Bonus;

	public Player playerPrefab;
	public Enemy enemyPrefab;
	public Girl girlPrefab;

	public static Player thePlayer;
	//public static Enemy theEnemy;
	public static Girl theGirl;

	public static List<Point> enemyPoints = new List<Point>();
	List<Enemy> enemies = new List<Enemy> ();

	AudioSource audio;

	void Awake ()
	{
		if (Save.lives < 1)
			Save.lives = 1;
		Save.bonus = Save.lives * 60;
		Instance = this;
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;
		audio = GetComponent<AudioSource> ();
	}

	void Start()
	{
		Save.RefreshLevels ();
		if (Save.playingLevel < 1)
			Save.playingLevel = 1;

		milkCount.digit = Save.lives;
		levelNo.Value = Save.playingLevel;


		thePlayer = Instantiate (playerPrefab.gameObject).GetComponent<Player> ();
		thePlayer.gameObject.transform.SetParent (TheField.instance.gameObject.transform);
		thePlayer.gameObject.transform.localScale = Vector3.one;
		thePlayer.gameObject.SetActive (false);

		theGirl   = Instantiate (girlPrefab.gameObject).GetComponent<Girl> ();
		theGirl.gameObject.transform.SetParent (TheField.instance.gameObject.transform);
		theGirl.gameObject.transform.localScale = Vector3.one;
		theGirl.gameObject.SetActive (false);

		enemyPoints.Clear ();
		enemies.Clear ();
		TheField.Generate ();
		TheField.LoadLevel (Save.playingLevel);

		for (int i=0; i<enemyPoints.Count; i++)
		{
			Enemy theEnemy = Instantiate (enemyPrefab.gameObject).GetComponent<Enemy> ();
			theEnemy.gameObject.transform.SetParent (TheField.instance.gameObject.transform);
			theEnemy.gameObject.transform.localScale = Vector3.one;
			theEnemy.gameObject.SetActive (false);
			theEnemy.startPos = enemyPoints [i];
			enemies.Add (theEnemy);
		}

		if (UILevels.editMode)
			StartEdit ();
		else
			StartCoroutine(StartGame ());
	}

	public static void ShakeCam()
	{
		Instance.shakeAnim.Play ();
	}

	static float lastTime;
	static AnimState lastState;
	public static void PlaySound(AnimState pState)
	{
		Debug.Log ("<color=green>"+pState.ToString ()+"</color>");
		foreach (Sounds sound in Instance.sounds) 
		{
			if (sound.state == pState)
			{
				if (Time.fixedTime - lastTime > 0.5f || lastState != pState) 
				{
					lastTime = Time.fixedTime;
					lastState = pState;
					Instance.audio.PlayOneShot (sound.clip);
				}
				return;
			}
		}
	}

	void Update()
	{
		if (UILevels.editMode)
			return;
		
		if (thePlayer.Active)
			Save.bonus -= Time.deltaTime;

		Bonus.Value = (int)(Save.bonus * 10);
	}

#region Edit
	void StartEdit()
	{
	}
#endregion Edit

#region Music
	/// <summary>
	/// Запустить проигрывание музыки.
	/// </summary>
	int PlayMusic(MusicType pType)
	{
		foreach (Musics m in music)
		{
			if (m.state == pType)
			{
				audio.PlayOneShot (m.clip);
				return m.length;
			}
		}
		return 0;
	}
#endregion Music

#region Gameplay
	IEnumerator StartGame()
	{
		theGirl.Spawn ();
		foreach (Enemy e in enemies)
			e.Spawn (true);
		thePlayer.Spawn (true);
		int length = 0;
		if (Save.firftPlay)
		{
			length = PlayMusic (MusicType.Intro);
			Save.firftPlay = false;
		}
		else
			length = PlayMusic (MusicType.Prepare2);
		
		yield return new WaitForSeconds (length);
		PlayMusic (MusicType.MainTheme);
		yield return null;
		thePlayer.Active = true;

		foreach (Enemy e in enemies)
		{
			e.Active = true;
			yield return null;
			yield return null;
		}
	}

	/// <summary>
	/// Игрок погиб
	/// </summary>
	IEnumerator DiePlayer(bool byWater)
	{
		thePlayer.CheckGround = true;

		foreach (Enemy e in enemies)
			e.Stop ();

		thePlayer.Stop ();

		foreach (Enemy e in enemies)
			e.Active = false;

		if (!byWater)
		{
			yield return null;

			thePlayer.CheckGround = false;
			thePlayer.speed.Y = thePlayer.SpeedJumpStart;
			thePlayer.PlaySound = false;
			PlaySound (AnimState.Falling);
			yield return null;
		}
		else
		{
			thePlayer.Active = false;
			thePlayer.ChangeState (AnimState.Die);
			PlaySound (AnimState.Die);
			yield return new WaitForSeconds (0.5f);
			thePlayer.gameObject.SetActive (false);

			audio.Stop ();

			int lenght = PlayMusic (MusicType.Loose);
			yield return new WaitForSeconds (lenght);
			Save.lives--;
			if (Save.lives > 0)
				UnityEngine.SceneManagement.SceneManager.LoadScene ("main");
			else
				UnityEngine.SceneManagement.SceneManager.LoadScene ("loader");

			Save.bonus -= 60;
		}
	}

	IEnumerator WinGame()
	{
		yield return null;
		audio.Stop ();
		yield return null;
		int lenght = PlayMusic (MusicType.Loved);
		yield return new WaitForSeconds (lenght);
		audio.Stop ();

		int stars = 3;
		int h = Save.GetHearts (Save.playingLevel);
		if (h<stars)
			Save.SetHearts (Save.playingLevel, stars);
		Save.playingLevel++;
		if (Save.GetHearts (Save.playingLevel) < 1)
			Save.SetHearts (Save.playingLevel, 0);
		
		UnityEngine.SceneManagement.SceneManager.LoadScene ("main");
	}

	/// <summary>
	/// Уровень закончился
	/// </summary>
	public static void DoneLevel()
	{
		foreach (Enemy e in Instance.enemies)
			e.gameObject.SetActive (false);
		thePlayer.gameObject.SetActive (false);
		thePlayer.Active = false;
		foreach (Enemy e in Instance.enemies)
			e.Active = false;
		Instance.StartCoroutine (Instance.WinGame ());
	}

	/// <summary>
	/// Подружиться
	/// </summary>
	public static void Friend(Point pPoint)
	{
		if (theGirl.mode == GirlMode.Ready) 
		{
			theGirl.ChangeState (GirlMode.Done);
		}
	}

	/// <summary>
	/// Убиение
	/// </summary>
	public static void Sink(AnimatedBase pPlayer, bool byWater)
	{
		if (pPlayer.iAmEnemy)
			Instance.StartCoroutine (Instance.EnemyDie (pPlayer));
		else
			Instance.StartCoroutine (Instance.DiePlayer (byWater));
	}

	/// <summary>
	/// Слопали суперфрукт
	/// </summary>
	public static void SuperEat(Vector2 pPosition)
	{
		PlaySound (AnimState.Eat);
		TheField.ShowCatch (thePlayer.position);
	}

	IEnumerator EnemyDie(AnimatedBase pPlayer)
	{
		yield return null;
		PlaySound (AnimState.Die);
		pPlayer.Active = false;
		pPlayer.ChangeState (AnimState.Die);
		yield return new WaitForSeconds (0.5f);
		StartCoroutine (EnemyBorn (pPlayer));
	}

	IEnumerator EnemyBorn(AnimatedBase pPlayer)
	{
		pPlayer.gameObject.SetActive (false);
		pPlayer.Active = false;
		yield return new WaitForSeconds (1f);
		if (thePlayer.Active)
		{
			pPlayer.gameObject.SetActive (true);
			pPlayer.Spawn (false);
		}
	}
#endregion Gameplay
}
