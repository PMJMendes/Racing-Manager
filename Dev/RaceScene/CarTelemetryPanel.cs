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
		_teamDriverData = GetNode<Label>("GridContainer/TeamDriverData");
		_timerData = GetNode<Label>("GridContainer/TimerData");
		_positionData = GetNode<Label>("GridContainer/PositionData");
		_speedData = GetNode<Label>("GridContainer/SpeedData");
		_rearLoadData = GetNode<Label>("GridContainer/RearLoadData");
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
