using Godot;

public partial class GameManager : Node
{
	public override void _Ready()
	{
		CallDeferred(nameof(SetupRace));
	}

	private void SetupRace()
	{
		Node currentScene = GetTree().CurrentScene;
		currentScene.QueueFree();
		
		var raceScene = GD.Load<PackedScene>("res://RaceScene/race_scene.tscn")
			.Instantiate<RaceScene>();
		
		GetTree().Root.AddChild(raceScene);
		GetTree().CurrentScene = raceScene;
		
		var carTemplate = GD.Load<PackedScene>("res://RaceScene/car.tscn");
		
		var car1 = carTemplate.Instantiate<Car>();
		car1.Color = Colors.Blue;
		car1.Mass = 1105;
		raceScene.AttachCar(car1, 1, "PlayerCar");
		
		var car2 = carTemplate.Instantiate<Car>();
		car2.Color = Colors.Red;
		car2.Mass = 1095;
		raceScene.AttachCar(car2, 2, "AICar");
		
		raceScene.Start();
	}
}
