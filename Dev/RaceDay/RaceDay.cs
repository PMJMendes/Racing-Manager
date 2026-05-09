using Godot;
using System;

public partial class RaceDay : Node
{
	public override void _Ready()
	{
		GetNode<Track>("World/Track").StartRace();
	}
}
