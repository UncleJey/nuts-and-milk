using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class Save 
{
	public static string CalculateMD5Hash(string input)
	{
		System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

		byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
		byte[] hash = md5.ComputeHash(inputBytes);

		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < hash.Length; i++)
			sb.Append(hash[i].ToString("X2"));

		return sb.ToString();
	}

	static string MD5(string pKey)
	{
		return CalculateMD5Hash("\\dev\\temp\\"+pKey + NativeTools.IMEI).Substring(2,10);
	}
	public static float bonus;
	public static int lives = 0;

	/// <summary>
	/// Первое вступление проиграть
	/// </summary>
	public static bool firftPlay = true;

	public static int maxLevel = 12;
	/// <summary>
	/// Кеш уровней
	/// </summary>
	public static int[] levels = new int[100];
	/// <summary>
	/// Самый высокий пройденный уровень
	/// </summary>
	public static int currentLevel = 0;
	/// <summary>
	/// Уровень в который пользователь играет сейчас
	/// </summary>
	public static int playingLevel = 0;
	/// <summary>
	/// Была ли инициализация
	/// </summary>
	static bool was = true;
	/// <summary>
	/// Загрузка уровней в кеш
	/// </summary>
	public static void RefreshLevels()
	{
		if (!was)
			return;
		
		for (int l = 1; l <= 50; l++)
		{
			int strs = GetHearts (l);
			if (was && strs >= 0)
			{
				currentLevel = l;
				levels [l] = strs;
			}
			else
			{
				was = false;
				levels [l] = 0;
			}
		}
	}

	public static int GetHearts(int pLevNum)
	{
		string k = "h" + pLevNum.ToString();
		if (PlayerPrefs.HasKey (k))
		{
			string s = PlayerPrefs.GetString (k);

			if (s == MD5 (k + "_5"))
				return 3;
			else if (s == MD5 (k + "_7"))
				return 2;
			else if (s == MD5 (k + "_12"))
				return 1;
			else if (s == MD5 (k + "_9"))
				return 0;
		}
		return -1;
	}

	public static void SetHearts(int pLevNum, int pNum)
	{
		string k = "h" + pLevNum.ToString();
		switch (pNum)
		{
			case 0:
				k += "_9";
			break;
			case 1:
				k += "_12";
			break;
			case 2:
				k += "_7";
			break;
			case 3:
				k += "_5";
			break;
			default:
			break;
				
		}
		PlayerPrefs.SetString ("h"+pLevNum.ToString(),MD5(k));
		PlayerPrefs.Save ();
		levels [pLevNum] = pNum;
		if (currentLevel < pLevNum)
			currentLevel = pLevNum;
	}
}
