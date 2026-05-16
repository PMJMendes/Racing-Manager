using System.Linq;
using Godot;

public partial class ChristmasTree : Node2D
{
    public enum StagingState
    {
        Clear,
        PreStage,
        Stage
    }

    public enum CountdownState
    {
        Clear,
        Amber1,
        Amber2,
        Amber3,
        Green
    }

    public void SetStagingState(int lane, StagingState state)
    {
        var tree = GetNode<Node2D>(lane == 1 ? "LeftTree" : "RightTree");
        tree.GetNode<Sprite2D>("PreStage").Visible = state == StagingState.PreStage;
        tree.GetNode<Sprite2D>("Stage").Visible = state == StagingState.Stage;
    }

    public void SetCountdownState(CountdownState state)
    {
        foreach(var tree in new[] { "LeftTree", "RightTree" }.Select(name => GetNode<Node2D>(name)))
        {
            tree.GetNode<Sprite2D>("Amber1").Visible = state > CountdownState.Clear;
            tree.GetNode<Sprite2D>("Amber2").Visible = state > CountdownState.Amber1;
            tree.GetNode<Sprite2D>("Amber3").Visible = state > CountdownState.Amber2;
            tree.GetNode<Sprite2D>("Green").Visible = state == CountdownState.Green;
        }
    }

    public void SetDisqualifiedState(int lane, bool dq)
    {
        var tree = GetNode<Node2D>(lane == 1 ? "LeftTree" : "RightTree");
        tree.GetNode<Sprite2D>("Red").Visible = dq;
    }

    public void ResetTree()
    {
        SetStagingState(1, StagingState.Clear);
        SetCountdownState(CountdownState.Clear);
        SetDisqualifiedState(1, false);
        SetStagingState(2, StagingState.Clear);
        SetCountdownState(CountdownState.Clear);
        SetDisqualifiedState(2, false);
    }
}
