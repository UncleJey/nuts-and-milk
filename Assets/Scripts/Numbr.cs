using UnityEngine;
using System.Collections;

public class Numbr : MonoBehaviour 
{
	public Digit[] digits;

	int _val = 0;

	public int Value
	{
		get 
		{
			return _val;
		}
		set 
		{
			if (_val < 0 || _val == value)
				return;
			
			_val = value;
			string v = _val.ToString ();

			int i = v.Length - 1;
			int j = digits.Length-1;
			int k = 0;

			while (j >= 0) 
			{
				k = 0;
				if (i >= 0)
					int.TryParse (v [i].ToString(), out k);
				digits [j].digit = k;
				j--;
				i--;
			}
		}
	}

}
