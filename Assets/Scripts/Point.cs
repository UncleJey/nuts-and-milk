using UnityEngine;
using System;

[System.Serializable]
public struct Point
{
	public int X;
	public int Y;

	/// <summary>
	/// Создание точки
	/// </summary>
	public Point(int pX, int pY)
	{
		X = pX;
		Y = pY;          
	}

	/// <summary>
	/// Для загрузки из сейва
	/// </summary>
	public Point(string pPoint)
	{
		if (pPoint != null && pPoint != string.Empty)
		{
			string[] d = pPoint.Split(',');
			if (d.Length == 2)
			{
				X = int.Parse(d[0]);
				Y = int.Parse(d[1]);
			}
			else
			{
				X = 0;
				Y = 0;
			}
		}
		else
		{
			X = 0;
			Y = 0;
		}
	}

	/// <summary>
	/// Дистанция (для оптимизации поиска пути)
	/// </summary>
	public Point(int pX1, int pX2, int pY1, int pY2)
	{
		X = pX1 - pX2;
		if (X<0)
			X = -X;
		Y = pY1-pY2;
		if (Y<0)
			Y = -Y;
	}

	public Point Clone()
	{
		return new Point (X, Y);
	}

	public static Point Zero  = new Point(0, 0); 
	public static Point One   = new Point(1, 1);
	public static Point Right = new Point(1, 0);
	public static Point Left  = new Point(-1,0);
	public static Point Up	  = new Point(0, 1);
	/// <summary>
	/// Down. x=0 y=-1
	/// </summary>
	public static Point Down  = new Point(0,-1);

	public bool Empty
	{
		get
		{
			return (X==0 && Y==0);
		}
	}

	/// <summary>
	/// Нормализованный единичный вектор
	/// </summary>
	public Point Normalized
	{
		get 
		{
			return new Point (X > 0 ? 1 : X < 0 ? -1 : 0, Y > 0 ? 1 : Y < 0 ? -1 : 0);
		}
	}

	/// <summary>
	/// Квадрат вектора с учётом знака
	/// </summary>
	public Point SignedSQR
	{
		get 
		{
			return new Point (X * (X > 0 ? X : -X), Y * (Y > 0 ? Y : -Y));
		}
	}

	public static Point ClipNegativeValues(Point p)
	{
		return new Point(p.X < 0 ? 0 : p.X, p.Y < 0 ? 0 : p.Y); 
	}

	public override int GetHashCode ()
	{
		return this.X.GetHashCode () ^ this.Y.GetHashCode () << 2;
	}

	public override string ToString()
	{
		return X.ToString()+","+Y.ToString();
	}

	public static bool operator == (Point lhs, Point rhs)
	{
		return lhs.X == rhs.X && lhs.Y==rhs.Y;
	}

	public static bool operator != (Point lhs, Point rhs)
	{
		return lhs.X != rhs.X || lhs.Y!=rhs.Y;
	}

	public static int SqrMagnitude(Point p1, Point p2)
	{
		return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
	}

	public static float Distance(Point p1, Point p2) {
		return Mathf.Sqrt(SqrMagnitude(p1, p2));
	}

	public override bool Equals (object other)
	{
		if (!(other is Point))
		{
			return false;
		}
		Point point = (Point)other;
		return this.X.Equals (point.X) && this.Y.Equals (point.Y);
	}

	public int this [int index]
	{
		get
		{
			switch (index)
			{
			case 0:
				return this.X;
			case 1:
				return this.Y;
			default:
				throw new IndexOutOfRangeException ("Invalid Vector3 index!");
			}
		}
		set
		{
			switch (index)
			{
			case 0:
				this.X = value;
				break;
			case 1:
				this.Y = value;
				break;
			default:
				throw new IndexOutOfRangeException ("Invalid Vector3 index!");
			}
		}
	}

	public int Magnit
	{
		get 
		{
			return (this.X > 0 ? this.X : -this.X) + (this.Y > 0 ? this.Y : -this.Y);
		}
	}

	public static Point operator +(Point p1, Point p2)
	{
		return new Point(p1.X + p2.X, p1.Y + p2.Y);
	}

	public static Point operator -(Point p1, Point p2)
	{
		return new Point(p1.X - p2.X, p1.Y - p2.Y);
	} 

	public static Point operator *(Point p1, float f)
	{
		return new Point ((int)(p1.X * f), (int)(p1.Y * f));
	} 

	public static Point operator *(Point p1, Point p2)
	{
		return new Point(p1.X * p2.X, p1.Y * p2.Y);
	} 

	public Vector2 ToVector2 ()
	{
		return new Vector2(X, Y);
	}
}
