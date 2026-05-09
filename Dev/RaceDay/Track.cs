using Godot;
using System;

public partial class Track : Node2D
{
	[Signal]
	public delegate void RaceStartedEventHandler();
	
	private bool _isRunning = false;
	
	public void StartRace()
	{
		EmitSignal(SignalName.RaceStarted);
		GD.Print("Race started");
	}
	
	
}
