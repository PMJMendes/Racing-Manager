using Godot;

public partial class PreRaceScene : Node
{

	public void Start()
	{
        RaceWeekendManager.Instance.StartRace();
	}
}
