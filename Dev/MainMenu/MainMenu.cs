using Godot;
using System;

public partial class MainMenu : Node
{
	public override void _Ready()
	{
		RaceWeekendManager.Instance.CallDeferred(nameof(RaceWeekendManager.StartWeekend));
	}
}
