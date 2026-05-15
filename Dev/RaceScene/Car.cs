using System;
using Godot;

public partial class Car : RigidBody2D
{
	public event EventHandler<CarTelemetry> TelemetryUpdated;

	[Export] public Color Color = Colors.White;
	[Export] public float EngineForce = 18700f;

	private bool _racing = false;
	private bool _braking = false;

	private double _time;

	public void AttachTrack(Track track)
	{
		track.RaceStarted += OnRaceStarted;
	}

	public void OnRaceFinished()
	{
		_braking = true;
		EngineForce = -170000f;
	}

	public override void _Ready()
	{
		GetNode<Sprite2D>("Sprite2D").SelfModulate = Color;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_racing)
		{
			return;
		}
		
		if (_braking && LinearVelocity.X < 0)
		{
			EngineForce = 0;
			LinearVelocity = Vector2.Zero;
			_racing = false;
			return;
		}
		
		_time += delta;
		ApplyCentralForce(Vector2.Right * EngineForce);
		
		TelemetryUpdated?.Invoke(this, new CarTelemetry(
			_time,
			LinearVelocity.X,
			Position.X
		));
	}

	private void OnRaceStarted()
	{
		_racing = true;
		_time = 0;
	}
}
