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
		car.Position = new Vector2(270, lane*200 - 40);
		AddChild(car);
		car.Name = name;
		car.AttachTrack(GetNode<Track>("Track"));
	}

	public void Start()
	{
		GetNode<Track>("Track").StartRace();
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
