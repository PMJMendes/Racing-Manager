using Godot;

public partial class RaceWeekendManager : Node
{
	public static RaceWeekendManager Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

	public void StartWeekend()
	{
		var preRaceScene = SceneChanger.Instance
			.ChangeTo<PreRaceScene>("res://PreRaceScene/pre_race_scene.tscn");

		preRaceScene.Start();
	}

	public void StartRace()
	{
		var raceScene = SceneChanger.Instance
			.ChangeTo<RaceScene>("res://RaceScene/race_scene.tscn");
		
		var carTemplate = GD.Load<PackedScene>("res://RaceScene/car.tscn");
		
		var car1 = carTemplate.Instantiate<Car>();
		car1.Name = "PlayerCar";
		car1.Color = Colors.Blue;
		raceScene.AttachCar(car1, 1);
		
		var car2 = carTemplate.Instantiate<Car>();
		car2.Name = "AICar";
		car2.Color = Colors.Red;
		raceScene.AttachCar(car2, 2);
		
		raceScene.Start();
	}
}
