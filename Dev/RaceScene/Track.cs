using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class Track : Node2D
{
	[Signal]
	public delegate void RaceStartedEventHandler();
	[Signal]
	public delegate void CarCrossedFinishLineEventHandler(Car car, double officialTime);
	[Signal]
	public delegate void CarFinishedRolloutEventHandler(Car car, double officialTime);

	[Export]
	public float Grip = 1.0f;

	public int CarCount { get; set; }
	public float StartLineX { get; private set; } = 0f;
	public float FinishLineX { get; private set; } = 0f;

	private ChristmasTree _christmasTree;
	private bool _isRunning = false;
	private double _raceElapsed = 0.0;
	private readonly Dictionary<Car, double> _reactionTimes = [];

	public float GetGripAt(Vector2 _) => Grip;

	public override void _Ready()
	{
		_christmasTree = GetNode<ChristmasTree>("ChristmasTree");

		var startLine = GetNode<Area2D>("StartLine");
		var rectangleShape = startLine.GetNode<CollisionShape2D>("CollisionShape2D")
			.Shape as RectangleShape2D;
		StartLineX = Position.X + startLine.Position.X + (rectangleShape.Size.X / 2f);

		var finishLine = GetNode<Area2D>("FinishLine");
		rectangleShape = finishLine.GetNode<CollisionShape2D>("CollisionShape2D")
			.Shape as RectangleShape2D;
		FinishLineX = Position.X + finishLine.Position.X - (rectangleShape.Size.X / 2f);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isRunning)
		{
			_raceElapsed += delta;
		}
	}

	private async Task StartRace()
	{
		await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
		_christmasTree.SetCountdownState(ChristmasTree.CountdownState.Amber3);

		await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);
		_christmasTree.SetCountdownState(ChristmasTree.CountdownState.Green);

		_isRunning = true;
		EmitSignal(SignalName.RaceStarted);
	}

	private void OnPreStagingAreaEntered(Area2D area)
	{
		if (area.GetParent() is Car car)
		{
			_christmasTree.SetStagingState(car.Position.Y < 12.5 ? 1 : 2, ChristmasTree.StagingState.PreStage);
			car.OnPreStaged();
		}
	}

	private async void OnStartLineAreaEntered(Area2D area)
	{
		if (area.GetParent() is Car car)
		{
			_christmasTree.SetStagingState(car.Position.Y < 12.5 ? 1 : 2, ChristmasTree.StagingState.Stage);
			car.OnStaged();
			CarCount--;
		}
		if (CarCount <= 0)
		{
			await StartRace();
		}
	}

	private void OnStartLineAreaExited(Area2D area)
	{
		if (area.GetParent() is Car car)
		{
			_reactionTimes[car] = InterpolateFinishTime(car, StartLineX, car => car.Position.X + car.TireTrail);
			EmitSignal(SignalName.CarFinishedRollout, car, _reactionTimes[car]);
		}
	}

	private void OnFinishLineBodyEntered(Node2D body)
	{
		if (body is Car car)
		{
            double elapsed = InterpolateFinishTime(car, FinishLineX, car => car.Position.X + car.BodyLead) - _reactionTimes[car];
            EmitSignal(SignalName.CarCrossedFinishLine, car, elapsed);
			car.OnRaceFinished();
			_christmasTree.ResetTree();
		}
	}

	private double InterpolateFinishTime(Car car, float referenceX, Func<Car, float> edgeCalc)
	{
		float carLeadingEdge = edgeCalc(car);
		
		float distancePastReference = carLeadingEdge - referenceX;
		
		float velocity = car.LinearVelocity.X;
		if (velocity <= 0)
		{
			return _raceElapsed;
		}
		
		double timeToReachFinish = distancePastReference / velocity;
		return _raceElapsed - timeToReachFinish;
	}
}
