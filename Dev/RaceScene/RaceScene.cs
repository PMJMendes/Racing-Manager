using System;
using System.Collections.Generic;
using Godot;

public partial class RaceScene : Node
{
	private readonly Dictionary<Car, List<CarTelemetry>> _telemetry = [];
	private readonly Dictionary<Car, CarTelemetryPanel> _telemetryPanels = [];
	private float _distance;
	private float _progression;
	private CameraRig _rig;

	public void AttachCar(Car car, int lane)
	{
		_telemetry[car] = [];
		car.TelemetryUpdated += OnTelemetryUpdated;
		GetNode<Node2D>("World").AddChild(car);
		car.Position = new Vector2(60.0f - car.BodyLead, lane * 14.0f - 8.5f);
		car.AttachTrack(GetNode<Track>("World/Track"));

		var panel = GD.Load<PackedScene>("res://RaceScene/car_telemetry_panel.tscn")
			.Instantiate<CarTelemetryPanel>();
		_telemetryPanels[car] = panel;
		panel.Position = new Vector2(lane * 480.0f + 240.0f, 800.0f);
		GetNode<CanvasLayer>("UI").AddChild(panel);
		panel.SetCarName(car.Name);
	}

	public void Start()
	{
		_rig = GetNode<CameraRig>("World/CameraRig");
		var track = GetNode<Track>("World/Track");
		_distance = track.FinishLineX - track.StartLineX;
		_progression = 0.0f;
		track.CarCount = _telemetry.Count;
	}

	private void OnTelemetryUpdated(object sender, CarTelemetry e)
	{
		if (sender is Car car)
		{
			_telemetry[car].Add(e);
			_progression = MathF.Max(_progression, e.Position);
			_rig.SweepCamera(_progression, _progression / _distance);
			_telemetryPanels[car].UpdateTelemetry(e);
		}
	}

	private void OnCarCrossedFinishLine(Car car, double officialTime)
	{
		GD.Print($"{car.Name} finished in {officialTime:F4}s");
	}

	private void OnCarFinishedRollout(Car car, double officialTime)
	{
		GD.Print($"{car.Name} reaction time: {officialTime:F4}s");
	}
}
