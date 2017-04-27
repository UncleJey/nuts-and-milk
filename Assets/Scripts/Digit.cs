using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Digit : MonoBehaviour 
{
	Image img;
	public Sprite[] sprites;

	void Awake()
	{
		img = GetComponent<Image> ();
	}

	public void Set(string pNum)
	{
		_digit = 0;
		int.TryParse (pNum, out _digit);
		if (_digit > 9 || _digit < 0)
			_digit = 0;

		digit = _digit;
	}

	int _digit = 0;
	public int digit
	{
		set 
		{
			_digit = value;
			if (_digit > 9 || _digit < 0)
				_digit = 0;

			img.sprite = sprites [_digit];
		}
		get 
		{
			return _digit;
		}
	}

}
