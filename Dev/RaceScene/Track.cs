using System.Collections.Generic;
using Godot;

public partial class Track : Node2D
{
	[Signal]
	public delegate void RaceStartedEventHandler();
	[Signal]
	public delegate void CarCrossedFinishLineEventHandler(Car car, double officialTime);

	private float _finishLineX = 0f;
	private bool _isRunning = false;
	private double _raceElapsed = 0.0;

	public void StartRace()
	{
		_isRunning = true;
		EmitSignal(SignalName.RaceStarted);
	}

	public override void _Ready()
	{
		var finishLine = GetNode<Area2D>("FinishLine");
		var collisionShape = finishLine.GetNode<CollisionShape2D>("CollisionShape2D");
		var rectangleShape = collisionShape.Shape as RectangleShape2D;
		_finishLineX = Position.X + finishLine.Position.X - (rectangleShape.Size.X / 2f);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isRunning)
		{
			_raceElapsed += delta;
		}
	}

	private void OnStartLineBodyEntered(Node2D body)
	{
	}

	private void OnFinishLineBodyEntered(Node2D body)
	{
		if (body is Car car)
		{
			EmitSignal(SignalName.CarCrossedFinishLine, car, InterpolateFinishTime(car));
			car.OnRaceFinished();
		}
	}

	private double InterpolateFinishTime(Car car)
	{
		var collisionShape = car.GetNode<CollisionShape2D>("CollisionShape2D");
		var rectangleShape = collisionShape.Shape as RectangleShape2D;
		float carLeadingEdge = car.Position.X + rectangleShape.Size.X / 2f;
		
		float distancePastFinish = carLeadingEdge - _finishLineX;
		
		float velocity = car.LinearVelocity.X;
		if (velocity <= 0)
		{
			return _raceElapsed;
		}
		
		double timeToReachFinish = distancePastFinish / velocity;
		return _raceElapsed - timeToReachFinish;
	}
}
