using Godot;
using System;

public partial class SceneChanger : Node
{
	public static SceneChanger Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

	public T ChangeTo<T>(string path) where T : Node
	{
		GetTree().CurrentScene.QueueFree();

		var result = GD.Load<PackedScene>(path).Instantiate<T>();

		GetTree().Root.AddChild(result);
		GetTree().CurrentScene = result;

		return result;
	}
}
