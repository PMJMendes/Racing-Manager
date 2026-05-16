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
	[Export] public float TireGrip = 2.0f;
	[Export] public float RearLoad = 0.55f;
	[Export] public float CgHeight = 0.5f;
	[Export] public float Wheelbase = 2.5f;

	public float BodyLead => _body.RegionRect.Size.X / 2.0f;
	public float TireLead => _frontTireBase.Position.X + _frontTireShape.Size.X / 2.0f;
	public float TireTrail => _frontTireBase.Position.X - _frontTireShape.Size.X / 2.0f;

	private Sprite2D _body;
	private Area2D _frontTireBase;
	private RectangleShape2D _frontTireShape;

	private CarState _state = CarState.Staging;

	private Track _track;

	private float _previousForce = 0.0f;
	private double _time;

	public void AttachTrack(Track track)
	{
		_track = track;
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
			_state = _state == CarState.Braking ? CarState.Finished : CarState.Staged;
			return;
		}

		var effectiveForce = 0.0f;
		var dynamicRearLoad = RearLoad;

		if (_state == CarState.Racing)
		{
			effectiveForce = EngineForce;
		    var currentAccel = _previousForce / Mass;
		    var weightTransfer = (CgHeight / Wheelbase) * (currentAccel / PhysicsSettings.s_Gravity);
    		dynamicRearLoad = Mathf.Clamp(RearLoad + weightTransfer, RearLoad, 1.0f);
			var tractionLimit = Mass * dynamicRearLoad * PhysicsSettings.s_Gravity * TireGrip * _track.GetGripAt(Position);
			if (effectiveForce > tractionLimit)
			{
				var slip = effectiveForce - tractionLimit;
				var slipRatio = slip / tractionLimit;
				effectiveForce = tractionLimit;
			}
		}

		if (_state == CarState.Braking)
		{
			effectiveForce = -BrakeForce;
		}

		_time += delta;
		ApplyCentralForce(Vector2.Right * effectiveForce);

		if (_state == CarState.Racing)
		{
			TelemetryUpdated?.Invoke(this, new CarTelemetry(
				_time,
				LinearVelocity.X,
				Position.X,
				dynamicRearLoad
			));
		}

		_previousForce = effectiveForce;
	}

	private void OnRaceStarted()
	{
		_state = CarState.Racing;
		_time = 0;
	}
}
