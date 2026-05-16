using System;
using Godot;

public partial class Car : RigidBody2D
{
	private enum CarState
	{
		Staging,
		Staged,
		Racing,
		Braking,
		Finished
	}

	public event EventHandler<CarTelemetry> TelemetryUpdated;

	[Export] public Color Color = Colors.White;
	[Export] public float EngineForce = 18700.0f;
	[Export] public float BrakeForce = 170000.0f;

	public float BodyLead => _body.RegionRect.Size.X / 2.0f;
	public float TireLead => _frontTireBase.Position.X + _frontTireShape.Size.X / 2.0f;
	public float TireTrail => _frontTireBase.Position.X - _frontTireShape.Size.X / 2.0f;

	private Sprite2D _body;
	private Area2D _frontTireBase;
	private RectangleShape2D _frontTireShape;

	private CarState _state = CarState.Staging;

	private float _force = 0.0f;
	private double _time;

	public void AttachTrack(Track track)
	{
		track.RaceStarted += OnRaceStarted;
	}

	public void OnPreStaged()
	{
		LinearVelocity = new Vector2(0.12f, 0.0f);
	}

	public void OnStaged()
	{
		LinearVelocity = new Vector2(0.0f, 0.0f);
	}

	public void OnRaceFinished()
	{
		_state = CarState.Braking;
		_force = -BrakeForce;
	}

	public override void _Ready()
	{
		_body = GetNode<Sprite2D>("Body");
		_frontTireBase = GetNode<Area2D>("FrontTireBase");
		_frontTireShape = GetNode<CollisionShape2D>("FrontTireBase/CollisionShape2D")
			.Shape as RectangleShape2D;

		_body.SelfModulate = Color;

		LinearVelocity = new Vector2(2.0f, 0.0f);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_state == CarState.Finished)
		{
			return;
		}
		
		if (LinearVelocity.X < 0)
		{
			LinearVelocity = Vector2.Zero;
			_force = 0;
			_state = _state == CarState.Braking ? CarState.Finished : CarState.Staged;
			return;
		}
		
		_time += delta;
		ApplyCentralForce(Vector2.Right * _force);

		if (_state == CarState.Racing)
		{
			TelemetryUpdated?.Invoke(this, new CarTelemetry(
				_time,
				LinearVelocity.X,
				Position.X
			));
		}
	}

	private void OnRaceStarted()
	{
		_state = CarState.Racing;
		_time = 0;
		_force = EngineForce;
	}
}
