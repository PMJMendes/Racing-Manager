using Godot;

public partial class CarTelemetryPanel : Control
{
	private Label _teamDriverData;
	private Label _timerData;
	private Label _positionData;
	private Label _speedData;
	private Label _rearLoadData;

	public override void _Ready()
	{
		_teamDriverData = GetNode<Label>("TeamDriverData");
		_timerData = GetNode<Label>("TimerData");
		_positionData = GetNode<Label>("PositionData");
		_speedData = GetNode<Label>("SpeedData");
		_rearLoadData = GetNode<Label>("RearLoadData");
	}

	public void SetCarName(string name)
	{
		_teamDriverData.Text = name;
	}

	public void UpdateTelemetry(CarTelemetry telemetry)
	{
		_timerData.Text = $"{telemetry.Time:F3} s";
		_positionData.Text = $"{telemetry.Position:F2} m";
		_speedData.Text = $"{telemetry.Speed:F2} m/s";
		_rearLoadData.Text = $"{telemetry.RearLoad:F2} N";
	}
}
