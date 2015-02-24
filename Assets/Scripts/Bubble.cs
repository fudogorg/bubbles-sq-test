
using System.Linq.Expressions;
using UnityEngine;

internal class Bubble
{
	private static readonly Color[] Colors = { Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.cyan };
	public float Size { get; private set; }
	public float PositionY { get; private set; }
	public float PositionX { get; private set; }
	public Color Color { get; private set; }

	public int Scores { get { return 10 * (int)(10 / Size); } }

	private readonly float _speed;

	public Bubble(float xBorders)
	{
		Size = Random.Range(0.3f, 1.0f);
		PositionX = Random.Range(xBorders, 1.0f - xBorders);
		PositionY = 1.0f;
		Color = Colors[Random.Range(0, Colors.Length)];
		_speed = Size * 10.0f;
	}

	public void UpdatePos(float dt)
	{
		PositionY -= dt / _speed;
	}
}

