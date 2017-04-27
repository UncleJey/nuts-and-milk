using UnityEngine;
using System;

public static class NativeTools
{
	static string _imei = null;
	/// <summary>
	/// Уникальный код устройства
	/// </summary>
	public static string IMEI
	{
		get
		{
			if (_imei == null)
			{
				#if UNITY_WEBGL
				_imei = Foranj.SDK.SocialPlatforms.SocialManager.id;
				#elif UNITY_EDITOR
				_imei = SystemInfo.deviceUniqueIdentifier;
				#elif UNITY_ANDROID

				//return getVal("android.telephony.TelephonyManager", "getDeviceId");
				AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
				AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
				AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");  
				AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
				_imei = secure.CallStatic<string> ("getString", contentResolver, "android_id");
				#endif
			}
			return _imei;
		}
	}

	/// <summary>
	/// Открыть ссылку
	/// </summary>
	public static void OpenURL(string pUrl)
	{
#if UNITY_WEBGL
		Application.ExternalCall("OpenURL",pUrl);
#else
		Application.OpenURL(pUrl);
#endif
	}



	/// <summary>
	/// Нативный выход из приложения
	/// </summary>
	public static void QuitApplication()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL

		//Application.Quit(); // No Exit muahaha
#elif UNITY_IPHONE
		Application.Quit();
#elif UNITY_ANDROID
		AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		activity.Call<bool>("moveTaskToBack", true);
#endif
	}
		/*
	static string _installerid = "-";
	/// <summary>
	/// Возвращает имя установщика (магазин)
	/// google        should decode at runtime to "com.android.vending";
	/// amazon        should decode at runtime to "com.amazon.venezia"; 
	/// Null - левая установка
	/// </summary>
	public static string InstallerId
	{
		get
		{
			if (_installerid == "-")
			{
	#if UNITY_EDITOR
	#elif UNITY_ANDROID
				try 
				{
					AndroidJNI.AttachCurrentThread();
					using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
					using (var curActivity = jc.GetStatic<AndroidJavaObject>("currentActivity")) 
					using (var pm = curActivity.Call<AndroidJavaObject>("getPackageManager")) 
					_installerid = pm.Call<string>("getInstallerPackageName", new object[] { CurrentBundle.identifier });

					if (string.IsNullOrEmpty(_installerid)) // левый инсталл
						_installerid = "!";
				}
				catch (Exception ex)
				{
					_installerid = "err";
					Debug.LogError(ex.Message);
				}
	#endif
			}
		return _installerid;
		}
	}
		*/
}