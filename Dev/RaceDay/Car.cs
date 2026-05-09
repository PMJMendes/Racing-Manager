using Godot;
using System;

public partial class Car : RigidBody2D
{
	[Export] public Color Color = Colors.White;
	[Export] public float EngineForce = 55820f;

	private bool _racing = false;

	public void StartRace()
	{
		_racing = true;
	}

	public override void _Ready()
	{
		GetNode<Sprite2D>("Sprite2D").Modulate = Color;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_racing)
		{
			return;
		}

		ApplyCentralForce(Vector2.Right * EngineForce);
	}
}
