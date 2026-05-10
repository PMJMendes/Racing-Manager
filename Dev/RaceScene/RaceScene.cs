using System;
using System.Collections.Generic;
using Godot;

public partial class RaceScene : Node
{
	private Dictionary<Car, List<CarTelemetry>> telemetry = [];

	public void AttachCar(Car car, int lane, string name)
	{
		telemetry[car] = [];
		car.TelemetryUpdated += OnTelemetryUpdated;
		car.Position = new Vector2(68.55f, lane * 9.4f - 4.3f);
		GetNode<Node2D>("World").AddChild(car);
		car.Name = name;
		car.AttachTrack(GetNode<Track>("World/Track"));
	}

	public void Start()
	{
		GetNode<Track>("World/Track").StartRace();
	}

	private void OnTelemetryUpdated(object sender, CarTelemetry e)
	{
		if (sender is Car car)
		{
			telemetry[car].Add(e);
		}
	}

	private void OnCarCrossedFinishLine(Car car, double officialTime)
	{
		GD.Print($"{car.Name} finished in {officialTime:F4}s");
	}
}
